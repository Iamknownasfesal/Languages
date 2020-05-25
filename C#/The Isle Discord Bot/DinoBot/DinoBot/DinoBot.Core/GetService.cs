using DinoBot.Core.Services.Items;
using DinoBot.Core.Services.Profiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinoBot.Core
{
    public class GetService
    {
        private readonly IItemService itemService;
        private readonly IProfileService profileService;

        public GetService(IItemService item,IProfileService profile)
        {
            itemService = item;
            profileService = profile;
        }

        public IItemService ReturnItemService()
        {
                return itemService;
        }

        public IProfileService ReturnProfileService()
        {
                return profileService;
        }

    }
}
