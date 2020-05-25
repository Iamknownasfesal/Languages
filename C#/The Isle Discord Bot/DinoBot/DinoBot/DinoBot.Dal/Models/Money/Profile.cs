using System;

namespace DinoBot.Dal.Models.Money
{
    public class Profile : Entity
    {
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }
        public int Gold { get; set; }
        public DateTime? cooldown_slut { get; set; }
        public DateTime? cooldown_crime { get; set; }
        public DateTime? cooldown_work { get; set; }
        public int Message { get; set; }

    }
}