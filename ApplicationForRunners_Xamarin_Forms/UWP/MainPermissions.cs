using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using ApplicationForRunners.SharedClasses;

namespace ApplicationForRunners.UWP
{
    class MainPermissions : IApplicationPermissions
    {
        GeolocationAccessStatus accessStatus;

        public bool CheckPermission(string permission)
        {
            switch (permission)
            {
                case "UWP.permission.LOCATION":
                    switch (accessStatus){
                        case GeolocationAccessStatus.Allowed:
                            return true;
                            //break;

                        case GeolocationAccessStatus.Denied:
                            return false;
                            //break;

                        case GeolocationAccessStatus.Unspecified:
                            return false;
                            //break;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return false;
        }


        public bool CheckPermissions(string[] permission){

            foreach (string variable in permission)
            {
                if (CheckPermission(variable) == false)
                    return false;
            }
            return true;
        }

        public async Task<bool> AskForPermissions(string[] permissionTab, Action<int> solicitMethod)
        {
            if (CheckPermissions(permissionTab) == true) {
                solicitMethod(2000);
                return await Task.FromResult(true);
            }
                

            foreach (string permissionUWP in permissionTab) {
                switch (permissionUWP) {
                    case "UWP.permission.LOCATION":

                        accessStatus = await Geolocator.RequestAccessAsync();
                        solicitMethod((int)accessStatus);
                        break;
                    default:                      
                           throw new NotImplementedException();
                }
           }

            return await Task.FromResult(false);
        }
    }
}
