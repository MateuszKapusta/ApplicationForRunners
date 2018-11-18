using ApplicationForRunners.SharedClasses;
using ApplicationForRunners.UWP;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoginSupplier))]

namespace ApplicationForRunners.UWP
{
    class LoginSupplier : ILoginSupplier
    {
        public Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(provider, "applicationforrunners");
        }
    }
}
