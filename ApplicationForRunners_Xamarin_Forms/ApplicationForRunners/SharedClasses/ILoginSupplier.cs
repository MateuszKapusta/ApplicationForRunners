using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace ApplicationForRunners.SharedClasses
{
    public interface ILoginSupplier
    {
        Task<MobileServiceUser> LoginAsync(MobileServiceClient client,MobileServiceAuthenticationProvider provider);
    }
}
