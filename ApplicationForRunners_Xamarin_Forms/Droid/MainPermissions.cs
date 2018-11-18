using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android.Content.PM;
using Plugin.CurrentActivity;
using ApplicationForRunners.SharedClasses;

namespace ApplicationForRunners.Droid
{
    class MainPermissions : ActivityCompat,IApplicationPermissions
    {
        public bool CheckPermission(string permission) {

            var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;

            if (CheckSelfPermission(currentActivity, permission) == (int)Permission.Granted)
                return true;
            else
                return false;       
        }

        public bool CheckPermissions(string[] permission)
        {
            foreach (string variable in permission) {

                if (CheckPermission(variable) == false) 
                    return false;
            }

            return true;
        }


        public List<SolicitPermission> permissionRequest = new List<SolicitPermission>();
        int requestLocationId = 1000;
        //method call only async system response
        public Task<bool> AskForPermissions(string[] permissionTab, Action<int> solicitMethod)
        {
            if (CheckPermissions(permissionTab) == true) {
                solicitMethod(2000);
                return Task.FromResult(true);
            }
              
            requestLocationId += 1;//change index for  Permission
            var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;
            permissionRequest.Add(new SolicitPermission(requestLocationId, permissionTab, currentActivity, solicitMethod));
            RequestPermissions(currentActivity, permissionTab, requestLocationId)/*)*/;//It is not async, only request to the system

            return Task.FromResult(false);
        }
    }
}