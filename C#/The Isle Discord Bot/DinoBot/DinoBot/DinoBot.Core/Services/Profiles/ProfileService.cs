using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DinoBot.Dal.Models.Money;
using DinoBot.Dal;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using DinoBot.Dal.Models.Items;
using Newtonsoft.Json;

namespace DinoBot.Core.Services.Profiles
{
    public interface IProfileService
    {
        Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId);
        Task ChangeUserGold(ulong discordId, ulong guildId, int Gold, bool lose = false);
        Task SetUserTimeWork(ulong discordId, ulong guildId, DateTime? time);
        Task SetUserTimeSlut(ulong discordId, ulong guildId, DateTime? time);
        Task SetUserTimeCrime(ulong discordId, ulong guildId, DateTime? time);
        Task<int> BuyRequest(ulong discordId, ulong guildId, string steamId, Item item);
        Task AddOneToMessage(ulong discordId, ulong guildId);
        Task WipeMessage(ulong discordId, ulong guildId);
        Task SetMoney(ulong discordId, ulong guildId, int money);
    }

    public class ProfileService : IProfileService
    {
        private readonly DbContextOptions<RPGContext> _options;

        public ProfileService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile != null) { return profile; }

            profile = new Profile
            {
                DiscordId = discordId,
                GuildId = guildId,
                Gold = 0
            };

            context.Add(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return profile;
        }

        public async Task ChangeUserGold(ulong discordId, ulong guildId, int Gold, bool lose = false)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if(profile == null) { return; }

            if(lose == true)
            {
                if (profile.Gold <= 0) { return; }
                if (profile.Gold > 0) { profile.Gold -= Gold; if (profile.Gold < 0) { profile.Gold = 0; } }
            }
            else if(lose == false)
            {
                if (Gold == 0) { return; }
                if (Gold > 0) { profile.Gold += Gold; }
            }

            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task SetUserTimeWork(ulong discordId, ulong guildId, DateTime? time)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.cooldown_work = time;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task SetUserTimeSlut(ulong discordId, ulong guildId, DateTime? time)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.cooldown_slut = time;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task AddOneToMessage(ulong discordId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.Message++;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task WipeMessage(ulong discordId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.Message = 0;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task SetUserTimeCrime(ulong discordId, ulong guildId, DateTime? time)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.cooldown_crime = time;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        public async Task SetMoney(ulong discordId, ulong guildId, int money)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile == null) { return; }

            profile.Gold = money;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return;
        }

        //0 is Profile Doesnt Exist
        //1 is SteamId is Empty
        //2 is Price is Higher than Profile Gold
        //3 is SteamId is not good
        //255 is Good

        public async Task<int> BuyRequest(ulong discordId, ulong guildId, string steamId, Item item)
        {
            using var context = new RPGContext(_options);
            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            string[] filePaths = Directory.GetFiles(@"C:\server\TheIsle\Saved\Databases\Survival\Players");

            string file = @"C:\server\TheIsle\Saved\Databases\Survival\Players" + steamId + ".json";
            bool fileSuccess = false;

            foreach (string files in filePaths)
            {
                if(files == file)
                {
                    fileSuccess = true;
                    break;
                }
                fileSuccess = false;
            }

            if (profile == null) { return 0; }
            if(steamId == string.Empty) { return 1; }
            if(profile.Gold < item.Price) { return 2; }
            if(fileSuccess == false) { return 3; }


            //Reading File (Made by Fesal)
            string jsonString = File.ReadAllText(@"C:\server\TheIsle\Saved\Databases\Survival\Players" + steamId+".json");

            //if you cant find it. Just return 3.
            if(jsonString == null) { return 3; }


            int gold = profile.Gold;
            profile.Gold -= item.Price;

            if(profile.Gold < 0)
            {
                profile.Gold = gold;

                context.Profiles.Update(profile);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return 2;
            }

            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);

            //Taking file as a jsonObject
            JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as JObject;
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);


            //Changing jObject's jTokens
            JToken jToken1 = jObject.SelectToken("CharacterClass");
            jToken1.Replace(item.RealName);
            JToken jToken2 = jObject.SelectToken("Growth");
            jToken2.Replace("1");
            JToken jToken3 = jObject.SelectToken("Hunger");
            jToken3.Replace("99999");
            JToken jToken4 = jObject.SelectToken("Thirst");
            jToken4.Replace("99999");
            JToken jToken5 = jObject.SelectToken("Stamina");
            jToken5.Replace("99999");
            JToken jToken6 = jObject.SelectToken("Health");
            jToken6.Replace("99999");
            JToken jToken7 = jObject.SelectToken("UnlockedCharacters");
            jToken7.Replace(" ");
            JToken jToken8 = jObject.SelectToken("bGender");
            jToken8.Replace(Convert.ToBoolean(item.Gender));

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(@"C:\server\TheIsle\Saved\Databases\Survival\Players" + steamId + ".json", output);

            return 255;
        }

    }
}
