using DinoBot.Dal.Models.Items;
using DinoBot.Dal.Models.Money;
using Microsoft.EntityFrameworkCore;

namespace DinoBot.Dal
{
    public class RPGContext : DbContext
    {
        public RPGContext()
        {

        }
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }
        public DbSet<Item> Items { get; set; }
        public DbSet<Profile> Profiles { get; set; }
    }
}
