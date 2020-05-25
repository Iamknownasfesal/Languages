using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DinoBot.Dal;
using DinoBot.Dal.Models.Items;
using DinoBot.Core.Services.Items;
using DinoBot.Handlers.Dialogue.Steps;
using DinoBot.Handlers.Dialogue;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus;
using DinoBot.Core.Services.Profiles;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using DinoBot.Dal.Models.Money;
using DinoBot.Attributes;

namespace DinoBot.Commands.Admin
{
    public class AdminCommands : BaseCommandModule
    {
        private readonly IItemService _itemService;
        private readonly IProfileService _profileService;

        public AdminCommands(IItemService itemService, IProfileService profileService)
        {
            _itemService = itemService;
            _profileService = profileService;
        }

        [Command("adddino")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task AddDino(CommandContext ctx, string dinoName, string dinoRealName, string Gender, int money)   
        {
            var item = new Item();
            item.Name = dinoName;
            item.RealName = dinoRealName;
            item.Gender = Gender;
            item.Price = money;

            await _itemService.CreateNewItemAsync(item).ConfigureAwait(false);

            string getGender = string.Empty;
            bool isGender = Convert.ToBoolean(item.Gender);
            if (isGender == false)
            {
                getGender = "Male";
            }
            else if (isGender == true)
            {   
                getGender = "Female";
            }
            else
            {
                getGender = "Unknown";
            }

            var itemUpdate = new DiscordEmbedBuilder();
            itemUpdate.WithColor(DiscordColor.White);
            itemUpdate
                .WithTitle("New Dino got Added!")
                .WithAuthor(ctx.Member.DisplayName + " executed this command")
                .WithDescription($"Info about {item.Name}")
                .AddField("Name", item.Name)
                .AddField("Real Name", item.RealName)
                .AddField("Price", item.Price.ToString())
                .AddField("Gender", getGender)
                .WithTimestamp(ctx.Message.CreationTimestamp);

            await ctx.Channel.SendMessageAsync(embed: itemUpdate).ConfigureAwait(false);
        }
        [Command("removedino")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task RemoveDino(CommandContext ctx)
        {
            var itemNameStep = new TextStep("Which item are you gonna remove?", null);
            string itemName = string.Empty;

            itemNameStep.OnValidResult += (result) => itemName = result;
            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                itemNameStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { await ctx.Channel.SendMessageAsync($"Error").ConfigureAwait(false); return; }

            Item item = await _itemService.GetItemByNameAsync(itemName).ConfigureAwait(false);

            if (item == null)
            {
                await ctx.Channel.SendMessageAsync($"There is no item called {itemName}").ConfigureAwait(false);
                return;
            }

            bool delete = await _itemService.RemoveItemAsync(itemName);

            if(delete == false) { await ctx.Channel.SendMessageAsync($"Error while removing item called {itemName}").ConfigureAwait(false); return; }
            if(delete == true) { await ctx.Channel.SendMessageAsync($"{itemName} got removed from the database").ConfigureAwait(false); return; }
        }

        [Command("SetMoney")]
        [RequireRoles(RoleCheckMode.All,"Owner")]
        [RequireChannel(ChannelCheckMode.Any,"owner-bot")]
        public async Task SetMoney(CommandContext ctx,DiscordMember member, int money)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(member.Id, member.Guild.Id).ConfigureAwait(false);
            if(profile == null) { await ctx.Channel.SendMessageAsync("I cant find the profile."); return; }
            await _profileService.SetMoney(member.Id, ctx.Guild.Id, money);
            await ctx.Channel.SendMessageAsync($"{member.DisplayName}'s money has been set to {money}");
        }

        [Command("dinoinfo")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task DinoInfo(CommandContext ctx)
        {
            var itemNameStep = new TextStep("Which dino are you looking for?", null);

            string itemName = string.Empty;

            itemNameStep.OnValidResult += (result) => itemName = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                itemNameStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { await ctx.Channel.SendMessageAsync($"Error").ConfigureAwait(false); return; }

            Item item = await _itemService.GetItemByNameAsync(itemName).ConfigureAwait(false);

            if (item == null)
            {
                await ctx.Channel.SendMessageAsync($"There is no item called {itemName}").ConfigureAwait(false);
                return;
            }

            string getGender = string.Empty;
            bool isGender = Convert.ToBoolean(item.Gender);
            if (isGender == false)
            {
                getGender = "Male";
            }
            else if(isGender == true)
            {
                getGender = "Female";
            }
            else
            {
                getGender = "Unknown";
            }

            var itemUpdate = new DiscordEmbedBuilder();
            itemUpdate.WithColor(DiscordColor.White);
            itemUpdate
                .WithTitle("Dino Info")
                .WithAuthor(ctx.Member.DisplayName + " executed this command")
                .WithDescription($"Info about {item.Name}")
                .AddField("Name", item.Name)
                .AddField("Real Name", item.RealName)
                .AddField("Price", item.Price.ToString())
                .AddField("Gender", getGender)
                .WithTimestamp(ctx.Message.CreationTimestamp);

            await ctx.Channel.SendMessageAsync(embed: itemUpdate).ConfigureAwait(false);
        }
    }
}
