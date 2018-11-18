using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Toasts;
using Xamarin.Forms;
using ApplicationForRunners.DataObjects;

namespace ApplicationForRunners.ItemManager
{
    public class ItemManager<TItemTable> where TItemTable : DataObject
    {
        protected MobileServiceClient client;
        protected IMobileServiceSyncTable<TItemTable> mainTable;

        public ItemManager(MobileServiceClient msClient,string msQuery)
        {
            this.client = msClient;
            this.mainTable = client.GetSyncTable<TItemTable>();

            //Use a different query name for each unique query in your program.//Unique child 
            uniqueQuery = msQuery/* + uniqueQueryCounter*/;
        }


        public async Task ClearTable() {

            await mainTable.PurgeAsync(true);
        }

        public async Task<TItemTable> RefreshItemAsync(string id) {

            if ( string.IsNullOrEmpty(id))            
                return null;

            await this.SyncRecordAsync(id);

            return await mainTable.LookupAsync(id);
        }


        //change to get all
        public async Task<ObservableCollection<TItemTable>> GetItemsAsync(bool syncAllItems = false,bool dataMark=true)
        {
            if (syncAllItems)
            {
                await this.SyncAsync(dataMark);
            }

            IEnumerable<TItemTable> items = await mainTable.ReadAsync();

            return new ObservableCollection<TItemTable>(items);           
        }

        //We can perform the response synchronization each time we change a record.
        public async Task SaveItemAsync(TItemTable item, bool syncItem = false)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await mainTable.InsertAsync(item);
            }
            else
            {
                await mainTable.UpdateAsync(item);
            }

            if(syncItem==true)
                await this.SyncRecordAsync(item.Id);
        }

        public async Task DeleteItemAsync(TItemTable item, bool syncItem = false) {

            await mainTable.DeleteAsync(item);

            if (syncItem == true)
                await this.SyncRecordAsync(item.Id);
        }



        public async Task SyncRecordAsync(string questionId)
        {
            if (!CrossConnectivity.Current.IsConnected || string.IsNullOrEmpty(questionId)) {             
                return;
            }

            try
            {   //push is added automatically
                await mainTable.PullAsync("TablePullOneQuery" + questionId, mainTable.Where(r => r.Id == questionId));
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)               
                    ServicePushFailedManage(exc.PushResult.Errors);
                
            }
        }


        //private static int uniqueQueryCounter = 0;
        readonly private string uniqueQuery;

        public async Task SyncAsync(bool isUsingTS)
        {
            if (!CrossConnectivity.Current.IsConnected) {
                return;
            }

            try
            {   //dont need push before pull, pull automatically use push before it self 
                //push all table
                await client.SyncContext.PushAsync();

                if(isUsingTS==true)
                    //pull actual table
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    await mainTable.PullAsync(uniqueQuery , mainTable.CreateQuery());
                else
                    await mainTable.PullAsync(null, mainTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)               
                    ServicePushFailedManage(exc.PushResult.Errors);
                
            }
        }


        async void ServicePushFailedManage(ReadOnlyCollection<MobileServiceTableOperationError> syncErrors) {

            // Simple error/conflict handling.
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    TItemTable serverItem=null;
                    TItemTable localItem=null;
                   
                    if (error.Result != null )
                        serverItem = error.Result.ToObject<TItemTable>();
                    if( error.Item != null)             
                        localItem = error.Item.ToObject<TItemTable>();


                    switch (error.OperationKind) {

                        case MobileServiceTableOperationKind.Update:

                            var notificator = DependencyService.Get<IToastNotificator>();
                            NotificationOptions options = new NotificationOptions();
                            options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Error);

                            if (serverItem != null)
                            {
                                options.Description = "Update "+ error.TableName + " failed, reverting to server's copy";
                                //Update failed, reverting to server's copy.
                                await error.CancelAndUpdateItemAsync(JObject.FromObject(serverItem));
                            }
                            else
                            {
                                options.Description = "Update " + error.TableName + " failed, discard local change";
                                // Discard local change.
                                await error.CancelAndDiscardItemAsync();
                            }

                            var result = await notificator.Notify(options);
                            break;
                        
                        case MobileServiceTableOperationKind.Insert:
                            //Check record and if corrupted use error.CancelAndDiscardItemAsync();
                            break;
                        case MobileServiceTableOperationKind.Delete:
                            break;
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.RawResult);

                }
            }

        }
    }
}
