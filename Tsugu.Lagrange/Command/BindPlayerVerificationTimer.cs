using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using System.Collections.Concurrent;
using System.Timers;
using Tsugu.Api;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;
using Tsugu.Lagrange.Util;
using Timer = System.Timers.Timer;

namespace Tsugu.Lagrange.Command;

public class BindPlayerVerificationTimer : Timer {
    private readonly static ConcurrentDictionary<string, BindPlayerVerificationTimer> Timers = new();
    
    private readonly string _backendUrl;

    private readonly BotContext _botContext;

    private readonly uint? _groupUin;

    private readonly uint _friendUin;

    private readonly uint _playerId;

    private readonly Server _mainServer;

    private readonly bool _unbind;

    private bool _disposed;

    private readonly object _lock;

    public BindPlayerVerificationTimer(
        string backendUrl, BotContext botContext, uint? groupUin, uint friendUin, uint playerId, Server mainServer,
        bool unbind
    ) : base(90 * 1000) {
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

        AutoReset = false;
        Elapsed += OnTimerElapsed;
        Enabled = true;

        Timers[userId] = this;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs _) {
        try {
            lock (_lock) {
                if (_disposed) {
                    return;
                }

                TsuguClient tsugu = new(_backendUrl);

                Task task = tsugu.User.BindPlayerVerification(
                    _friendUin.ToString(), _mainServer, _playerId, Constant.Platform, _unbind
                );

                string reply = $"玩家 [{_playerId}] 已绑定成功！";

                try {
                    task.Wait();
                } catch (EndpointCallException e) {
                    reply = e.Message;
                } catch (Exception e) {
                    reply = e.InnerException?.Message ?? e.Message;
                }
                
                MessageBuilder mb = MessageUtil.GetDefaultMessageBuilder(_friendUin, _groupUin);

                mb.Text(reply);

                _botContext.SendMessage(mb.Build());
            }
        } finally {
            Dispose();
        }
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
