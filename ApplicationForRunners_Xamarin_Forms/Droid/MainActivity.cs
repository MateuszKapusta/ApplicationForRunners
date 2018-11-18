using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Toasts;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.SecureStorage;
using Plugin.CurrentActivity;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;

namespace ApplicationForRunners.Droid
{
    [Activity (Label = "AFRunner",
		Icon = "@drawable/trainers144AFR",// "@android:color/transparent"
        MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, 
        Theme = "@android:style/Theme.Holo.Light")]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

        MainPermissions newMainPermissions;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            //token save 
            ProtectedFileImplementation.StoragePassword = Build.Id;
            SecureStorageImplementation.StorageType = StorageTypes.PasswordProtectedFile;
            // Initialize Azure Mobile Apps
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            // Initialize Xamarin Forms
            global::Xamarin.Forms.Forms.Init (this, bundle);
            //Init Map
            Xamarin.FormsMaps.Init(this, bundle);
            //Register your dependency
            DependencyService.Register<ToastNotification>(); 
            ToastNotification.Init(this);

            newMainPermissions = new MainPermissions();
            Constants.ApplicationActualPermissions = newMainPermissions;

            CrossCurrentActivity.Current.Init(this, bundle);
            //if you use AppCenter
            //AppCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
            // Load the main application
            LoadApplication(new App ());
        }

	
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        
            if (null != SolicitPermission.GetSolicitPermission(requestCode, permissions, newMainPermissions.permissionRequest)) {
                SolicitPermission solicidRequestMethod = SolicitPermission.GetSolicitPermission(requestCode, permissions, newMainPermissions.permissionRequest);
                solicidRequestMethod.solicitMethod((int)grantResults[0]);                
            }
            //Geolocator automaticly install Permission Plugin, with is not useby rest class
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

