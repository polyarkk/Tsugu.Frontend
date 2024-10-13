using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Timers;
using Tsugu.Api;
using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;
using Timer = System.Timers.Timer;

namespace Tsugu.Lagrange.Command;

public class BindPlayerVerificationTimer : Timer {
    private readonly static ILogger<BindPlayerVerificationTimer> Logger =
        LoggerUtil.GetLogger<BindPlayerVerificationTimer>();

    private readonly static ConcurrentDictionary<string, BindPlayerVerificationTimer> Timers = new();

    private const int MaxRetries = 6;

    private readonly string _backendUrl;

    private readonly IMessageContext _messageContext;

    private readonly uint _playerId;

    private readonly Server _mainServer;

    private readonly bool _unbind;

    private bool _disposed;

    private readonly object _lock;

    private int _retriesRemaining;

    public BindPlayerVerificationTimer(
        string backendUrl, IMessageContext messageContext, uint playerId, Server mainServer,
        bool unbind
    ) : base(15 * 1000) {

        // cancel previous unhandled request
        if (Timers.TryGetValue(messageContext.UserIdentifier, out BindPlayerVerificationTimer? timer)) {
            timer.Dispose();
        }

        _backendUrl = backendUrl;
        _messageContext = messageContext;
        _playerId = playerId;
        _mainServer = mainServer;
        _unbind = unbind;
        _disposed = false;
        _lock = new object();
        _retriesRemaining = MaxRetries;

        Elapsed += OnTimerElapsed;
        Enabled = true;

        Timers[messageContext.UserIdentifier] = this;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs _) {
        lock (_lock) {
            if (_disposed) {
                return;
            }

            using TsuguClient tsugu = new(_backendUrl);

            string userId = _messageContext.FriendId;

            Task task = tsugu.User.BindPlayerVerification(
                userId, _mainServer, _playerId, _messageContext.Platform, _unbind
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
                            _messageContext.FriendId, _playerId, _retriesRemaining, _retriesRemaining == 1 ? "y" : "ies", message
                        );

                        return;
                    }

                    Logger.LogInformation(
                        "verifying binding player for [{uin} -> {playerId}] failed with reason {reason}",
                        _messageContext.FriendId, _playerId, message
                    );

                    _messageContext.ReplyPlainText(message + "，绑定失败！");
                } else {
                    Logger.LogError("internal error while verifying binding player for [{uin} -> {playerId}]\n{e}",
                        _messageContext.FriendId, _playerId, e.ToString()
                    );

                    _messageContext.ReplyPlainText("绑定失败，后台异常！");
                }

                Dispose();

                return;
            }

            // success
            string reply = _unbind ? "解绑成功" : "绑定成功，现在可以使用 *玩家状态* 命令查看绑定的玩家状态";

            // 若解绑且userPlayerIndex == userPlayerList.Length - 1，重置userPlayerIndex防止数组越界
            if (_unbind) {
                TsuguUser user = tsugu.User.GetUserData(userId, _messageContext.Platform).Result;

                if (user.UserPlayerIndex >= user.UserPlayerList.Length) {
                    tsugu.User.ChangeUserData(userId, _messageContext.Platform, userPlayerIndex: 0).Wait();

                    if (user.UserPlayerList.Length > 0) {
                        reply += "（已重置主账号为第一个绑定的账号）";
                    }
                }
            }

            _messageContext.ReplyPlainText(reply);
            Dispose();
        }
    }

    protected override void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        Enabled = false;
        _disposed = true;
        Timers.TryRemove(_messageContext.UserIdentifier, out _);
        base.Dispose(disposing);
    }
}
