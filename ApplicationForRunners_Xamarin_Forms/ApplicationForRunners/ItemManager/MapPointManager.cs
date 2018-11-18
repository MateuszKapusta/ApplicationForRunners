using ApplicationForRunners.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ApplicationForRunners.ItemManager
{
    public class MapPointManager : ItemManager<MapPointItem>
    {
        public MapPointManager(MobileServiceClient msClient) : base(msClient, "MapPointItemPullAllQuery")
        {

        }

        public async Task<ObservableCollection<MapPointItem>> GetSpecificItemsAsync(string search, bool syncAllItems = false, bool dataMark = true)
        {
            if (syncAllItems)
            {
                await this.SyncAsync(dataMark);
            }

            IEnumerable<MapPointItem> items = await mainTable.Where(e => e.WorkoutItemId.Contains(search)).ToEnumerableAsync();
            return new ObservableCollection<MapPointItem>(items);
        }
    }
}
