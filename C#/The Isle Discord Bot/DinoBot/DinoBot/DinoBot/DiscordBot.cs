using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus.Entities;
using System.Threading;
using DinoBot.Commands.Buy.Request;
using DinoBot.Commands.Games;
using DinoBot.Commands.Admin;
using DinoBot.Commands.Profiles;
using DinoBot.Core;
using DinoBot.Dal.Models.Money;
using DinoBot.Core.Services.Profiles;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using DinoBot.Dal;

namespace DinoBot
{
    public class DiscordBot
    {

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public DiscordPresence Presence { get; private set; }
        private Timer DiscordStatusTimer { get; set; }

        private IProfileService _profileService;
        private ServiceProvider serviceProvider;
        private Func<IEnumerable<IProfileService>> getServices;

        public DiscordBot(IServiceProvider services)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<GameCommands>();
            Commands.RegisterCommands<RequestBuyCommands>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<ProfileCommands>();

            _profileService = services.GetRequiredService<IProfileService>();

            Client.MessageCreated += async e =>
            {
                if (_profileService == null) { return; }

                if(e.Author.IsBot == true) { return; }

                var profile = await _profileService.GetOrCreateProfileAsync(e.Author.Id, e.Guild.Id);

                if(profile == null) { return; }

                if(profile.Message == 20) { await _profileService.ChangeUserGold(profile.DiscordId, profile.GuildId, 500, false); await _profileService.WipeMessage(profile.DiscordId, profile.GuildId); }
                if(profile.Message < 20 && profile.Message > 0)
                { await _profileService.AddOneToMessage(profile.DiscordId, profile.GuildId); }
                else
                { await _profileService.WipeMessage(profile.DiscordId, profile.GuildId); }
            };

                Client.ConnectAsync();
        }

        public DiscordBot(ServiceProvider serviceProvider, Func<IEnumerable<IProfileService>> getServices)
        {
            this.serviceProvider = serviceProvider;
            this.getServices = getServices;
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            this.DiscordStatusTimer = new Timer(this.StatusUpdate,
                                                null,
                                                TimeSpan.Zero,
                                                TimeSpan.FromMinutes(1));


            return Task.CompletedTask;
        }

        private void StatusUpdate(object _)
        {
            try
            {
                this.Client.UpdateStatusAsync(new DiscordActivity("The Isle", ActivityType.Playing), UserStatus.Online, DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                this.Client.DebugLogger.LogMessage(LogLevel.Error, "Companion Cube", string.Concat("Failed to set status (", ex.GetType().ToString(), ": ", ex.Message, ")"), DateTime.Now);
            }
        }
    }
}
