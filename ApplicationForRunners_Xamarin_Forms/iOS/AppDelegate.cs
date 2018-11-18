using Foundation;
using UIKit;
using Plugin.Toasts;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;

namespace ApplicationForRunners.iOS
{
    [Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();
            SQLitePCL.Batteries.Init();
            // Initialize Xamarin Forms
            Forms.Init ();
            //Init Maps
            Xamarin.FormsMaps.Init();

            Constants.ApplicationActualPermissions = new MainPermissions();

            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init();
            //if you use AppCenter
            //AppCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
            //#if ENABLE_TEST_CLOUD
            //            Xamarin.Calabash.Start();
            //#endif

            LoadApplication(new App ());
			return base.FinishedLaunching (app, options);
		}

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {            
            return DBConnection.MainConnection.Client.ResumeWithURL(url);
        }

    }
}

