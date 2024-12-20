﻿using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Event;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Text.Json;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Enum;

namespace Tsugu.Frontend.Service;

internal class LagrangeHostedService : IHostedService, IDisposable {
    private readonly BotContext _botContext;

    private readonly ILogger<LagrangeHostedService> _logger;

    private readonly Timer _gcTimer;

    private readonly FilterService _filterService;

    private BotKeystore _keyStore;

    private bool _needQrCodeLogin;

    private readonly AppSettings _appSettings;

    public LagrangeHostedService(
        IConfiguration configuration,
        ILogger<LagrangeHostedService> logger,
        FilterService filterService
    ) {
        _appSettings = configuration.GetSection("Tsugu").Get<AppSettings>()!;
        _logger = logger;

        _logger.LogInformation("--- TSUGU LAGRANGE FRONTEND IS NOW STARTING!!! ---");
        _filterService = filterService;
        _gcTimer = new Timer(_ => GC.Collect(), null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));

        BotDeviceInfo deviceInfo;

        if (!File.Exists("device.dat")) {
            deviceInfo = GenerateDeviceInfo();
        } else {
            string deviceStr = File.ReadAllText("device.dat");

            if (!string.IsNullOrWhiteSpace(deviceStr)) {
                deviceInfo = JsonSerializer.Deserialize<BotDeviceInfo>(deviceStr) ?? GenerateDeviceInfo();
            } else {
                deviceInfo = GenerateDeviceInfo();
            }
        }

        if (!File.Exists("key.dat")) {
            _keyStore = GenerateKeyStore();
        } else {
            string keyStoreStr = File.ReadAllText("key.dat");

            if (!string.IsNullOrWhiteSpace(keyStoreStr)) {
                _keyStore = JsonSerializer.Deserialize<BotKeystore>(keyStoreStr) ?? GenerateKeyStore();
            } else {
                _keyStore = GenerateKeyStore();
            }
        }

        _botContext = BotFactory.Create(new BotConfig(), deviceInfo, _keyStore);
    }

    public void Dispose() {
        _botContext.Dispose();
        _gcTimer.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        if (_appSettings.Lagrange.Enabled) {
            await Login();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private BotDeviceInfo GenerateDeviceInfo() {
        _logger.LogInformation("generating new device info file...");

        BotDeviceInfo deviceInfo = BotDeviceInfo.GenerateInfo();
        File.WriteAllText("device.dat", JsonSerializer.Serialize(deviceInfo));

        _needQrCodeLogin = true;

        return deviceInfo;
    }

    private BotKeystore GenerateKeyStore() {
        _logger.LogInformation("generating new key file...");

        BotKeystore keyStore = new();
        File.WriteAllText("key.dat", JsonSerializer.Serialize(keyStore));

        _needQrCodeLogin = true;

        return keyStore;
    }

    private async Task Login() {
        if (!_needQrCodeLogin) {
            _logger.LogInformation("attempting to login using existing key... confirm on phone may be needed");

            if (await _botContext.LoginByPassword()) {
                await HandlePostLogin();

                return;
            }
        }

        (string url, byte[] qrCodePng) = await _botContext.FetchQrCode() ??
            throw new Exception("failed to fetch qr code! (receiving null)");

        using (QRCodeGenerator qrGenerator = new())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
        using (AsciiQRCode qrCode = new(qrCodeData)) {
            _logger.LogInformation("Login using QR Code! \n{qrcode}", qrCode.GetGraphic(1));
        }

        File.Delete("login-qr.png");
        await File.WriteAllBytesAsync("login-qr.png", qrCodePng);
        _logger.LogInformation("note: you can also scan qr code through [login-qr.png] generated in working dir");

        await _botContext.LoginByQrCode();

        await HandlePostLogin();
    }

    private async Task HandlePostLogin() {
        _logger.LogInformation("bot login with UIN {uin}, updating key and device info...", _botContext.BotUin);

        _keyStore = _botContext.UpdateKeystore();
        File.Delete("key.dat");
        await File.WriteAllTextAsync("key.dat", JsonSerializer.Serialize(_keyStore));

        File.Delete("device.dat");
        await File.WriteAllTextAsync("device.dat", JsonSerializer.Serialize(_botContext.UpdateDeviceInfo()));

        InitializeHandlers();
    }

    private void InitializeHandlers() {
        _logger.LogInformation("initializing handlers...");

        _botContext.Invoker.OnFriendMessageReceived += OnMessageReceived;
        _botContext.Invoker.OnGroupMessageReceived += OnMessageReceived;
        _botContext.Invoker.OnGroupPokeEvent += OnPokeEvent;
        _botContext.Invoker.OnFriendPokeEvent += OnPokeEvent;
    }

    private async void OnMessageReceived<TMessageEvent>(BotContext ctx, TMessageEvent e)
    where TMessageEvent : EventBase {
        MessageChain chain;
        MessageSource source;

        if (e is FriendMessageEvent fe) {
            chain = fe.Chain;
            source = MessageSource.Friend;
        } else if (e is GroupMessageEvent ge) {
            chain = ge.Chain;
            source = MessageSource.Group;
        } else {
            return;
        }

        await _filterService.InvokeFilters(new LagrangeMessageContext(ctx, chain, source));
    }

    private async void OnPokeEvent<TPokeEvent>(BotContext ctx, TPokeEvent e) {
        uint attacker, victim;
        uint? groupUin = null;
        
        if (e is FriendPokeEvent fpe) {
            attacker = fpe.OperatorUin;
            victim = fpe.TargetUin;
        } else if (e is GroupPokeEvent gpe) {
            attacker = gpe.OperatorUin;
            victim = gpe.TargetUin;
            groupUin = gpe.GroupUin;
        } else {
            return;
        }

        if (victim != ctx.BotUin || attacker == victim) {
            return;
        }

        if (groupUin == null) {
            await ctx.FriendPoke(attacker);
        } else {
            await ctx.GroupPoke((uint)groupUin, attacker);
        }
    }
}
