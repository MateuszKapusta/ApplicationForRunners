using ApplicationForRunners.DataObjects;
using Microsoft.WindowsAzure.MobileServices;

namespace ApplicationForRunners.ItemManager
{
    public  class WorkoutItemManager:ItemManager<WorkoutItem>
    {

        public WorkoutItemManager(MobileServiceClient msClient) : base(msClient, "WorkoutItemPullAllQuery")
        {
            
        }
    }
}
