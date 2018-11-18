using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApplicationForRunners.DataObjects;
using ApplicationForRunners.ItemManager;
using ApplicationForRunners.SharedClasses;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.SecureStorage;
using Plugin.Toasts;
using Xamarin.Forms;

namespace ApplicationForRunners
{
    public class DBConnection
    {
        public static DBConnection MainConnection { get; private set; } = new DBConnection();
        public MobileServiceClient Client { get; }
        ILoginSupplier UserLogin { get; }

        public WorkoutItemManager WorkoutItem;
        public MapPointManager MapPoint ;
        public UserItemManager UserItem;

        private DBConnection()
        {

            Client = new MobileServiceClient(Constants.ApplicationURL);

            const string offlineDbPath = @"localstore.db";
            var store = new MobileServiceSQLiteStore(offlineDbPath);

            store.DefineTable<WorkoutItem>();
            store.DefineTable<UserItem>();
            store.DefineTable<MapPointItem>();
            //Initializes the SyncContext using the default IMobileServiceSyncHandler.This class executes the async calls to synchronize the database
            Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            WorkoutItem = new WorkoutItemManager(Client);
            MapPoint = new MapPointManager(Client);
            UserItem = new UserItemManager(Client);
            UserLogin = DependencyService.Get<ILoginSupplier>();
        }


        public async Task ClearDB() {

            await MapPoint.ClearTable();
            await WorkoutItem.ClearTable();
            await UserItem.ClearTable();
        }

        public async Task DownloadDB()
        {
            await UserItem.GetItemsAsync(true, false);
            await WorkoutItem.GetItemsAsync(true, false);
            await MapPoint.GetItemsAsync(true, false);
        }

        const string userIdKey = ":UserId";
        const string tokenKey = ":Token";

        public bool CheckLogin() {

            if (CrossSecureStorage.Current.HasKey(userIdKey)
                && CrossSecureStorage.Current.HasKey(tokenKey))
            {
                string userId = CrossSecureStorage.Current.GetValue(userIdKey);
                string token = CrossSecureStorage.Current.GetValue(tokenKey);

                if (IsTokenExpired(token)) 
                    return false;
                
                Client.CurrentUser = new MobileServiceUser(userId)
                {
                    MobileServiceAuthenticationToken = token
                };
                  
                return true;
            }
            return false;
        }

        public async Task<bool> AcountExist()
        {
            UserItemManager userManager = DBConnection.MainConnection.UserItem;
            await userManager.ClearTable();

            var isLogged = await userManager.GetItemsAsync(true,false);

            if (isLogged.Count == 1)           
                return await Task.FromResult(true);
            else
                return await Task.FromResult(false);
        }


        public async Task<bool> LoginUser(MobileServiceAuthenticationProvider provider,bool tempLogin=false) {

            var notificator = DependencyService.Get<IToastNotificator>();
            NotificationOptions options = new NotificationOptions();

            if (!CrossConnectivity.Current.IsConnected){          
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Info);
                options.Description = "There is no internet connection";
                await notificator.Notify(options);

                return await Task.FromResult(false);
            }

            try
            {
                await UserLogin.LoginAsync(Client, provider);
               
                if (tempLogin==false)               
                    await ConfirmUser();                
            }
            catch (Exception ex)
            {
                var message = string.Format("{0}", ex.Message);           
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Error);
                options.Description = message;
                await notificator.Notify(options);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        public async Task ConfirmUser()
        {
            var user = Client.CurrentUser;
            if (user != null)
            {
                CrossSecureStorage.Current.SetValue(userIdKey, user.UserId);
                CrossSecureStorage.Current.SetValue(tokenKey, user.MobileServiceAuthenticationToken);
                await DownloadDB();
            }
        }


        public async Task<bool> LogoutUser() {

        var notificator = DependencyService.Get<IToastNotificator>();
        NotificationOptions options = new NotificationOptions();
        try
        {
            var uri = new Uri($"{Client.MobileAppUri}/.auth/logout");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Client.CurrentUser.MobileServiceAuthenticationToken);
                HttpResponseMessage serverAnswer=await httpClient.GetAsync(uri);
            }

            await Client.LogoutAsync();

            CrossSecureStorage.Current.DeleteKey(userIdKey);
            CrossSecureStorage.Current.DeleteKey(tokenKey);

            await ClearDB();
        }
            catch  {               
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Error);
                options.Description = "Logout failed";
                await notificator.Notify(options);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private bool IsTokenExpired(string token)
        {
            // No token == expired.
            if (string.IsNullOrEmpty(token))
                return true;

            // Split the string apart; we want the JSON payload.
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Token must consist from 3 delimited by dot parts.");

            string jwt = parts[1]
                .Replace('-', '+')  // 62nd char of encoding
                .Replace('_', '/'); // 63rd char of encoding
            switch (jwt.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: jwt += "=="; break; // Two pad chars
                case 3: jwt += "="; break;  // One pad char
                default:
                    throw new ArgumentException("Token is not a valid Base64 string.");
            }

            // Convert to a JSON string (std. Base64 decode)
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(jwt));

            // Get the expiration date from the JSON object.
            var jsonObj = JObject.Parse(json);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // JWT expiration is an offset from 1/1/1970 UTC
            var expire = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(exp);
            return expire < DateTime.UtcNow;
        }
    }
}
