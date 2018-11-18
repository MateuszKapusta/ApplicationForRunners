using Xamarin.Forms;
using Plugin.Toasts;
using Plugin.SecureStorage;
using ApplicationForRunners.SharedClasses;

namespace ApplicationForRunners
{
    public static class Constants
	{
		// Replace strings with your Azure Mobile App endpoint.
		public static string ApplicationURL = @"https://applicationforrunners.azurewebsites.net";

        public static string AppUnitsType {
            get {
                if (CrossSecureStorage.Current.HasKey(":UnitsType"))
                    return CrossSecureStorage.Current.GetValue(":UnitsType");
                else
                    return "KM";
            }
            set {
                if (value.Equals("KM") || value.Equals("MI")) {
                    CrossSecureStorage.Current.SetValue(":UnitsType", value);
                }                   
                else return;
            }
        }

        public static IApplicationPermissions ApplicationActualPermissions { get; set; }
        public static string[] LocationPermissions {
            get {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        string[] droidLocationPermissions = {
                            "android.permission.ACCESS_COARSE_LOCATION",//internet
                            "android.permission.ACCESS_FINE_LOCATION"//GPS
                    };
                        return droidLocationPermissions;
                        //break;
                    case Device.iOS:
                        string[] iOSLocationPermissions = {
                           "IOS.permission.LOCATION",//Location
                            };
                        return iOSLocationPermissions;
                        //break;
                    case Device.UWP:             
                        string[] uWPLocationPermissions = {
                            "UWP.permission.LOCATION",//Location
                            };
                        return uWPLocationPermissions;
                        //break;
                }
                return null;
            }
        }

        public static string[] NotificationPermissionsiOS {
            get
            {
                string[] iOSNotification = { "IOS.permission.NOTIFICATION" };
                return iOSNotification;
            }
        }


        //Factory
        public static class NotificationStyle
        {
            public enum Which {Info,Success, Warning, Error };

            static public  NotificationOptions Options(Which chosen) {
                var value = new NotificationOptions();
                switch (chosen) {
                    case Which.Info:
                        value = new NotificationOptions()
                        {
                            Title = "Info",
                            //Description = "The Description Content",
                            //IsClickable = true,
                            WindowsOptions = new WindowsOptions() { LogoUri = "icon.png" },
                            ClearFromHistory = true,
                            //AllowTapInNotificationCenter = false,
                            AndroidOptions = new AndroidOptions()
                            {
                                HexColor = "#00C1DE",
                                SmallDrawableIcon = 17301659,
                                //ForceOpenAppOnNotificationTap = true
                            }
                        };
                        break;

                    case Which.Success:
                        value = new NotificationOptions()
                        {
                            Title = "Success",
                            WindowsOptions = new WindowsOptions() { LogoUri = "icon.png" },
                            ClearFromHistory = true,
                            AndroidOptions = new AndroidOptions()
                            {
                                HexColor = "#3FF18B",
                                SmallDrawableIcon = 17301540,
                            }
                        };
                        break;

                    case Which.Warning:
                        value = new NotificationOptions()
                        {
                            Title = "Warning",
                            WindowsOptions = new WindowsOptions() { LogoUri = "icon.png" },
                            ClearFromHistory = true,
                            AndroidOptions = new AndroidOptions()
                            {
                                HexColor = "#7500E5",
                                SmallDrawableIcon = 17301642,
                            }
                        };
                        break;

                    case Which.Error:
                        value = new NotificationOptions()
                        {
                            Title = "Error",
                            WindowsOptions = new WindowsOptions() { LogoUri = "icon.png" },
                            ClearFromHistory = true,
                            AndroidOptions = new AndroidOptions()
                            {
                                HexColor = "#E7002F",
                                SmallDrawableIcon = 17301543,
                            }
                        };
                        break;
                }
                return value;
            }
        }
    }
}

