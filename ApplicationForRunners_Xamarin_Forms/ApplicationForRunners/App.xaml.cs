using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.Toasts;
using System;
using Xamarin.Forms;

namespace ApplicationForRunners
{
    public partial class App : Application
    {
        public App ()
		{
			InitializeComponent ();

            if (DBConnection.MainConnection.CheckLogin())
                MainPage = new AplicationPages.MainMDPage(); 
            else
                MainPage = new AplicationPages.StartPage(); 
        }

        protected override void OnStart()
        {   // Handle when your app starts 
            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    Action<int> actionNotification = HandleNotificationIOS;
                    Constants.ApplicationActualPermissions.AskForPermissions(Constants.NotificationPermissionsiOS, actionNotification);
                    break;
                case Device.Android:
                    CrossConnectivity.Current.ConnectivityChanged += HandleConnectChanged;
                    break;
                case Device.UWP:
                    CrossConnectivity.Current.ConnectivityChanged += HandleConnectChanged;
                    break;
            }


            if (!AppGeolocator.IsSupported)
                throw new Exception("CrossGeolocator isn’t supported yet.");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        void HandleNotificationIOS(int value)
        {
            if (value == 1 || value == 2000)
                CrossConnectivity.Current.ConnectivityChanged += HandleConnectChanged;
            // NotifPerIOSFlag
        }

        async void HandleConnectChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            NotificationOptions options= new NotificationOptions();
            
            if (e.IsConnected)
            {
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Info);
                options.Description = "Connected to internet";      
            }
            else {
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Info);
                options.Description = "Not connected to internet";                  
            }          
            var result = await notificator.Notify(options);
        }
    }
}