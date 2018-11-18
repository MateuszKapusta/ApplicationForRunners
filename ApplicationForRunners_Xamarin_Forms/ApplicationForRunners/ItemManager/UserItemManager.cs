using ApplicationForRunners.DataObjects;
using Microsoft.WindowsAzure.MobileServices;

namespace ApplicationForRunners.ItemManager
{
    public class UserItemManager : ItemManager<UserItem>
    {

        public UserItemManager(MobileServiceClient msClient) : base(msClient, "UserItemPullAllQuery")
        {

        }
    }
}
