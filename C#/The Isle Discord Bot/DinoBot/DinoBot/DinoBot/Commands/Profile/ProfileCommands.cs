using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DinoBot.Core.Services.Profiles;
using DinoBot.Dal.Models.Money;
using DinoBot.Attributes;

namespace DinoBot.Commands.Profiles
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;
        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("profile")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank", "owner-bot")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("profile")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank", "owner-bot")]
        public async Task Profile(CommandContext ctx, DiscordMember member)
        {
            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
                ThumbnailUrl = member.AvatarUrl
            };

            profileEmbed.AddField("Money", profile.Gold.ToString());

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }

        [Command("bank")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank", "owner-bot")]
        public async Task Worth(CommandContext ctx)
        {
            await GetWorth(ctx, ctx.Member.Id);
        }
        [Command("bank")]
        [RequireChannel(ChannelCheckMode.Any, "work-crime-slut-bank", "owner-bot")]
        public async Task Worth(CommandContext ctx,DiscordMember member)
        {
            await GetWorth(ctx, member.Id);
        }

        private async Task GetWorth(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            var worthEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Worth"
            };
            worthEmbed.AddField("Money", profile.Gold.ToString());

            await ctx.Channel.SendMessageAsync(embed: worthEmbed).ConfigureAwait(false);
        }
    }
}
