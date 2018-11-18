using ApplicationForRunners.iOS;
using ApplicationForRunners.SharedClasses;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoginSupplier))]

namespace ApplicationForRunners.iOS
{
    class LoginSupplier : ILoginSupplier
    {
        public Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, provider, "applicationforrunners");
        }
    }
}
