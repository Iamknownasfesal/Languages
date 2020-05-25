using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System.IO;
using System.Diagnostics;
using DinoBot.Dal.Models.Money;
using DinoBot.Core.Services.Profiles;
using DinoBot.Attributes;

namespace DinoBot.Commands.Games
{
    public class GameCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;

        //Work is 0
        //Slut is 1
        //Crime is 2

        public GameCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("work")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank")]
        public async Task Work(CommandContext ctx)
        {
            int reward = 0;

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            Random random = new Random();
            reward = random.Next(500, 2001);

            if (profile.cooldown_work.HasValue)
            {
                var timeSpan = DateTime.Now - profile.cooldown_work.Value;
                if (timeSpan >= TimeSpan.FromHours(1.5))
                {
                    await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward);
                    await _profileService.SetUserTimeWork(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                    var embedMessage = GetMessage(reward, 0);

                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
                else
                {
                    var embedMessage = new DiscordEmbedBuilder();
                    TimeSpan timeLeft = TimeSpan.FromHours(1.5) - timeSpan;
                    embedMessage.WithTitle("Too early!");
                    embedMessage.WithDescription("You need to wait " + timeLeft.ToString(@"hh\:mm\:ss"));

                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
            }
            else
            {
                await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward);
                await _profileService.SetUserTimeWork(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                var embedMessage = GetMessage(reward, 0);

                await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                return;
            }
        }

        [Command("slut")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank")]
        public async Task Slut(CommandContext ctx)
        {
            int reward = 0;

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            Random random = new Random();
            reward = random.Next(501, 4001);

            int loseNum = random.Next(0, 101);
            bool lose = false;

            if (loseNum > 35)
            {
                lose = false;
            }
            else if (loseNum <= 35)
            {
                lose = true;
            }

            if (profile.cooldown_slut.HasValue)
            {
                var timeSpan = DateTime.Now - profile.cooldown_slut.Value;
                if (timeSpan >= TimeSpan.FromHours(1.5))
                {
                    await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward, lose);
                    await _profileService.SetUserTimeSlut(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                    var embedMessage = GetMessage(reward, 1, lose);

                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
                else
                {
                    var embedMessage = new DiscordEmbedBuilder();
                    TimeSpan timeLeft = TimeSpan.FromHours(1.5) - timeSpan;
                    embedMessage.WithTitle("Too early!");
                    embedMessage.WithDescription("You need to wait " + timeLeft.ToString(@"hh\:mm\:ss"));
                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
            }
            else
            {
                await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward, lose);
                await _profileService.SetUserTimeSlut(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                var embedMessage = GetMessage(reward, 1 , lose);

                await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                return;
            }
        }
        [Command("crime")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank")]
        public async Task Crime(CommandContext ctx)
        {
            int reward = 0;

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id).ConfigureAwait(false);
            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            Random random = new Random();
            reward = random.Next(501, 4001);

            int loseNum = random.Next(0, 101);
            bool lose = false;

            if (loseNum > 35)
            {
                lose = false;
            }
            else if (loseNum <= 35)
            {
                lose = true;
            }

            if (profile.cooldown_crime.HasValue)
            {
                var timeSpan = DateTime.Now - profile.cooldown_crime.Value;
                if (timeSpan >= TimeSpan.FromHours(1.5))
                {
                    await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward, lose);
                    await _profileService.SetUserTimeCrime(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                    var embedMessage = GetMessage(reward, 2, lose);

                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
                else
                {
                    var embedMessage = new DiscordEmbedBuilder();
                    TimeSpan timeLeft = TimeSpan.FromHours(1.5) - timeSpan;
                    embedMessage.WithTitle("Too early!");
                    embedMessage.WithDescription("You need to wait " + timeLeft.ToString(@"hh\:mm\:ss"));
                    await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                    return;
                }
            }
            else
            {
                await _profileService.ChangeUserGold(ctx.Member.Id, ctx.Guild.Id, reward, lose);
                await _profileService.SetUserTimeCrime(ctx.Member.Id, ctx.Guild.Id, DateTime.Now);
                var embedMessage = GetMessage(reward, 2, lose);

                await ctx.Channel.SendMessageAsync(embed: embedMessage).ConfigureAwait(false);
                return;
            }
        }


        private DiscordEmbedBuilder GetMessage(int Reward, int actionNum, bool lose = false)
        {
            var embedMessage = new DiscordEmbedBuilder();
            if (lose == true)
            {
                embedMessage.WithTitle("Lost");
                embedMessage.WithDescription(GetRandomText(Reward, actionNum, lose) + $" :moneybag:{Reward}");
                embedMessage.WithColor(DiscordColor.Red);
            }
            else if (lose == false)
            {
                embedMessage.WithTitle("Win");
                embedMessage.WithDescription(GetRandomText(Reward, actionNum, lose) + $" :moneybag:{Reward}");
                embedMessage.WithColor(DiscordColor.Green);
            }
            else if (Reward == 0)
            {
                embedMessage.WithTitle("What the fuck");
                embedMessage.WithDescription(GetRandomText(Reward, actionNum, lose) + $" :moneybag:{Reward}");
                embedMessage.WithColor(DiscordColor.White);
            }

            return embedMessage;
        }

        private string GetRandomText(int reward, int actionNum, bool lose = false)
        {
            Random random = new Random();
            int HandleOfMessage;
            int SlutOfMessage;
            string Message = "NaN";
            HandleOfMessage = random.Next(0, 5);
            SlutOfMessage = random.Next(0, 2);

            if(actionNum == 0)
            {
                switch(HandleOfMessage)
                {
                    case 0:
                        Message = "You did a pretty good job!";
                        break;
                    case 1:
                        Message = "Here is a little bonus for your great work!";
                        break;                    
                    case 2:
                        Message = "Hey! Here is some money from your job, don't spend it all at once. ;)";
                        break;                    
                    case 3:
                        Message = "Keep up the good work!";
                        break;                    
                    case 4:
                        Message = "Here you go, you earned it!";
                        break;
                    case 5:
                        Message = "Ka-ching! You just got money!";
                        break;
                    default:
                        Message = "NaN";
                        break;
                }
            }
            else if(actionNum == 1)
            {
                if(lose == true)
                {
                    switch (SlutOfMessage)
                    {
                        case 0:
                            Message = "Oopsy daisy you just lost, maybe next time sugar.";
                            break;
                        case 1:
                            Message = "Are you just unlucky or did you want to lose?";
                            break;
                        case 2:
                            Message = "Uhh... Did you meant to do that?";
                            break;
                        default:
                            Message = "NaN";
                            break;
                    }
                }
                else if(lose == false)
                {
                    switch (SlutOfMessage)
                    {
                        case 0:
                            Message = "Lucky you, you got some juicy money!";
                            break;
                        case 1:
                            Message = "You really know how to suck money out of people don't you?";
                            break;
                        case 2:
                            Message = "Money, money, money!";
                            break;
                        default:
                            Message = "NaN";
                            break;
                    }
                }
                else if(reward == 0)
                {
                    Message = "Wow...";
                }
                else
                {
                    Message = "Contact Fesal";
                }
            }
            else if(actionNum == 2)
            {
                if(lose == false)
                {
                    switch (SlutOfMessage)
                    {
                        case 0:
                            Message = "You invaded a hangout party and stole. You win";
                            break;
                        case 1:
                            Message = "You rob a squirrel of his nuts and pawn them. You win";
                            break;
                        case 2:
                            Message = "You blow up Bungie Headquarters and earn";
                            break;
                        default:
                            Message = "NaN";
                            break;
                    }
                }
                else if(lose == true)
                {
                    switch (SlutOfMessage)
                    {
                        case 0:
                            Message = "N-Word? What did you say? You pay";
                            break;
                        case 1:
                            Message = "You attempt to steal, but the person stabs you. You pay";
                            break;
                        case 2:
                            Message = "You got caught by FBI while trying to get loli photos. You've been sent to jail and fined. You lost";
                            break;
                        default:
                            Message = "NaN";
                            break;
                    }
                }
            }
            else
            {
                Message = "Something is really wrong";
            }


            return Message;
        }
    }
}
