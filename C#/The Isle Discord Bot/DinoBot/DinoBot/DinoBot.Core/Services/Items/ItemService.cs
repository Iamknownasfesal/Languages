using DinoBot.Dal;
using DinoBot.Dal.Models.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoBot.Core.Services.Items
{
    public interface IItemService
    {
        Task CreateNewItemAsync(Item item);
        Task<Item> GetItemByNameAsync(string itemName);
        Task<bool> RemoveItemAsync(string itemName);
    }
    public class ItemService : IItemService
    {
        private readonly DbContextOptions<RPGContext> _options;

        public ItemService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        public async Task CreateNewItemAsync(Item item)
        {
            using var context = new RPGContext(_options);

            context.Add(item);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Item> GetItemByNameAsync(string itemName)
        {
            using var context = new RPGContext(_options);

            return await context.Items.FirstOrDefaultAsync(x => x.Name.ToLower() == itemName.ToLower()).ConfigureAwait(false);
        }        
        public async Task<bool> RemoveItemAsync(string itemName)
        {
            using var context = new RPGContext(_options);

            var item = await GetItemByNameAsync(itemName).ConfigureAwait(false);

            context.Remove(item);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
    }
}
