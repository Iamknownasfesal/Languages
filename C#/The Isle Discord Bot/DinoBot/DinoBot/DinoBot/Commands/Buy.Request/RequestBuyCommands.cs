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
using DinoBot.Core.Services.Items;
using DinoBot.Dal.Models.Items;
using DinoBot.Core.Services.Profiles;
using DinoBot.Attributes;

namespace DinoBot.Commands.Buy.Request
{
    public class RequestBuyCommands : BaseCommandModule
    {
        private readonly IItemService _itemService;
        private readonly IProfileService _profileService;

        public RequestBuyCommands(IItemService itemService, IProfileService profileService)
        {
            _itemService = itemService;
            _profileService = profileService;
        }

        //0 is Profile Doesnt Exist
        //1 is SteamId is Empty
        //2 is Price is Higher than Profile Gold
        //3 is SteamId is not good
        //255 is Good

        [Command("Buy")]
        [Description("Buying Dino")]
        [RequireChannel(checkMode: ChannelCheckMode.Any,channelNames: "buy-dinosaurs")]
        public async Task Buy(CommandContext ctx, string steamId, string DinoName)
        {
            Item item = await _itemService.GetItemByNameAsync(DinoName).ConfigureAwait(false);

            if (item == null)
            {
                await ctx.Channel.SendMessageAsync($"There is no dino called {DinoName}").ConfigureAwait(false);
                return;
            }

            int isItGood = await _profileService.BuyRequest(ctx.Member.Id, ctx.Guild.Id, steamId, item);

            await ctx.Channel.SendMessageAsync("Request has been send :ok_hand:");

            if (isItGood == 0)
            {
                await ctx.Channel.SendMessageAsync("Error");
                return;
            }
            else if(isItGood == 1)
            {
                await ctx.Channel.SendMessageAsync("Error about SteamId");
                return;
            }
            else if(isItGood == 2)
            {
                await ctx.Channel.SendMessageAsync("You need more money! :moneybag:");
                return;
            }
            else if(isItGood == 3)
            {
                await ctx.Channel.SendMessageAsync("I cant find your steam id...");
                return;
            }
            else if(isItGood == 255)
            {
                await ctx.Channel.SendMessageAsync("Request has been completed");
                return;
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Error");
                return;
            }
        }
    }
}
