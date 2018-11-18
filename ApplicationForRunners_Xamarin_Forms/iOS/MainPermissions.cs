using System;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;
using UserNotifications;
using ApplicationForRunners.SharedClasses;

namespace ApplicationForRunners.iOS
{
    class MainPermissions : IApplicationPermissions
    {
        public bool CheckPermission(string permission)
        {
            switch (permission)
            {
                case "IOS.permission.LOCATION":
                        //application dont need location per below ios 8.0
                        if (!(UIDevice.CurrentDevice.CheckSystemVersion(8, 0)))
                            return true;

                         switch (CLLocationManager.Status)
                        {
                        case CLAuthorizationStatus.Authorized:
                                return true;
                                //break;
                        case CLAuthorizationStatus.AuthorizedWhenInUse:
                                return true;
                                //break;
                        case CLAuthorizationStatus.Denied:
                                return false;
                                //break;
                        case CLAuthorizationStatus.NotDetermined:
                                return false;
                                //break;
                        }          
                    break;

                case "IOS.permission.NOTIFICATION":

                    if (!(UIDevice.CurrentDevice.CheckSystemVersion(10, 0)))
                        return false;
                        
                    UIUserNotificationType types = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;                   

                    if (types.HasFlag(UIUserNotificationType.Alert)/*|| types.HasFlag(UIUserNotificationType.Badge) || types.HasFlag(UIUserNotificationType.Sound)*/) {
                        return true;
                    } 
                    return false;
                    //break;
            }
            return false;
        }

        public bool CheckPermissions(string[] permission)
        {
            foreach (string variable in permission)
            {
                if (CheckPermission(variable) == false)
                    return false;
            }
            return true;
        }



        CLLocationManager locManager = new CLLocationManager();
        Action<int> locHandler = null;

        public  Task<bool> AskForPermissions(string[] permissionTab, Action<int> solicitMethod)
        {
            if (CheckPermissions(permissionTab) == true) {
                solicitMethod(2000);
                return Task.FromResult(true);
            }
               
            foreach (string permissionIOS in permissionTab)
            {
                switch (permissionIOS)
                {
                    case "IOS.permission.LOCATION":

                        locHandler = solicitMethod;
                        locManager.AuthorizationChanged += LocHandlerEvent;

                        if (!(UIDevice.CurrentDevice.CheckSystemVersion(8, 0)))
                            return Task.FromResult(true);

                        locManager.RequestAlwaysAuthorization(); //RequestWhenInUseAuthorization

                        break;

                    case "IOS.permission.NOTIFICATION":
                        // Request Permissions
                        if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                        {                         
                            // Request Permissions
                            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
                            {   // Do something 
                                if(granted==true)
                                    solicitMethod(1);
                                else
                                    solicitMethod(0);
                            });
                        }
                        break;
                }
            }

            return Task.FromResult(false);
        }

        private void LocHandlerEvent(object sender, CLAuthorizationChangedEventArgs args)
        {
            if (locHandler != null)
                locHandler((int)args.Status);// this is CLLocationManager.Status
        }

    }
}