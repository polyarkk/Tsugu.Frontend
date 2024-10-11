using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Timers;
using Tsugu.Api;
using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;
using Tsugu.Lagrange.Util;
using Timer = System.Timers.Timer;

namespace Tsugu.Lagrange.Command;

public class BindPlayerVerificationTimer : Timer {
    private readonly static ILogger<BindPlayerVerificationTimer> Logger =
        LoggerUtil.GetLogger<BindPlayerVerificationTimer>();

    private readonly static ConcurrentDictionary<string, BindPlayerVerificationTimer> Timers = new();

    private const int MaxRetries = 6;

    private readonly string _backendUrl;

    private readonly BotContext _botContext;

    private readonly uint? _groupUin;

    private readonly uint _friendUin;

    private readonly uint _playerId;

    private readonly Server _mainServer;

    private readonly bool _unbind;

    private bool _disposed;

    private readonly object _lock;

    private int _retriesRemaining;

    public BindPlayerVerificationTimer(
        string backendUrl, BotContext botContext, uint? groupUin, uint friendUin, uint playerId, Server mainServer,
        bool unbind
    ) : base(15 * 1000) {
        string userId = friendUin.ToString();

        // cancel previous unhandled request
        if (Timers.TryGetValue(userId, out BindPlayerVerificationTimer? timer)) {
            timer.Dispose();
        }

        _backendUrl = backendUrl;
        _botContext = botContext;
        _groupUin = groupUin;
        _friendUin = friendUin;
        _playerId = playerId;
        _mainServer = mainServer;
        _unbind = unbind;
        _disposed = false;
        _lock = new object();
        _retriesRemaining = MaxRetries;

        Elapsed += OnTimerElapsed;
        Enabled = true;

        Timers[userId] = this;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs _) {
        lock (_lock) {
            if (_disposed) {
                return;
            }

            TsuguClient tsugu = new(_backendUrl);

            string userId = _friendUin.ToString();

            Task task = tsugu.User.BindPlayerVerification(
                userId, _mainServer, _playerId, Constant.Platform, _unbind
            );

            try {
                task.Wait();
            } catch (AggregateException e) {
                if (e.InnerException is EndpointCallException ece) {
                    string message = ece.Message.Replace("\n", "");
                    
                    if (_retriesRemaining > 1) {
                        _retriesRemaining--;
                        
                        Logger.LogInformation(
                            "verifying binding player for [{uin} -> {playerId}] failed ({retries} retr{ies} remaining) with reason {reason}",
                            _friendUin, _playerId, _retriesRemaining, _retriesRemaining == 1 ? "y" : "ies", message
                        );

                        return;
                    }

                    Logger.LogInformation(
                        "verifying binding player for [{uin} -> {playerId}] failed with reason {reason}",
                        _friendUin, _playerId, message
                    );

                    ReplyToUser(message + "，绑定失败！");
                } else {
                    Logger.LogError("internal error while verifying binding player for [{uin} -> {playerId}]\n{e}",
                        _friendUin, _playerId, e.ToString()
                    );

                    ReplyToUser("绑定失败，后台异常！");
                }

                Dispose();

                return;
            }

            // success
            string reply = $"玩家 [{_playerId}] 已绑定成功！";

            // 若解绑且userPlayerIndex == userPlayerList.Length - 1，重置userPlayerIndex防止数组越界
            if (_unbind) {
                TsuguUser user = tsugu.User.GetUserData(userId, Constant.Platform).Result;

                if (user.UserPlayerIndex >= user.UserPlayerList.Length) {
                    tsugu.User.ChangeUserData(userId, Constant.Platform, userPlayerIndex: 0).Wait();

                    if (user.UserPlayerList.Length > 0) {
                        reply += "（已重置主账号为第一个绑定的账号）";
                    }
                }
            }

            ReplyToUser(reply);
        }

        Dispose();
    }

    private void ReplyToUser(string text) {
        _botContext
            .SendMessage(
                MessageUtil
                    .GetDefaultMessageBuilder(_friendUin, _groupUin)
                    .Text(text)
                    .Build()
            )
            .Wait();
    }

    protected override void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        Enabled = false;
        _disposed = true;
        Timers.TryRemove(_friendUin.ToString(), out _);
        base.Dispose(disposing);
    }
}
