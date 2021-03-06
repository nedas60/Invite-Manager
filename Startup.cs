﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Invite_Manager.Event;
using Invite_Manager.Util;
using Discord.Rest;
using System.Collections.Generic;

namespace Invite_Manager
{
    class Startup
    {
        public static void Main(string[] args)
        => new Startup().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LoggingService.LogAsync;
                services.GetRequiredService<CommandService>().Log += LoggingService.LogAsync;
                client.UserJoined += services.GetRequiredService<Events>().AnnounceUserJoined;
                client.Ready += services.GetRequiredService<Events>().onReady;
                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
                await client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                
                await Task.Delay(Timeout.Infinite);
            }
        }
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Events>()
                .AddSingleton<ConfigService>()
                .AddSingleton<InviteService>()
                .BuildServiceProvider();
        }
    }
}
