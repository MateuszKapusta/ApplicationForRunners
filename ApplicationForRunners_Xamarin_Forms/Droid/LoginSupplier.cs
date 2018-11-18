using System.Threading.Tasks;
using ApplicationForRunners.Droid;
using ApplicationForRunners.SharedClasses;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoginSupplier))]

namespace ApplicationForRunners.Droid
{
    class LoginSupplier : ILoginSupplier
    {
        public Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;

            return client.LoginAsync(currentActivity, provider, "applicationforrunners");
        }
    }
}