using System;
using System.Threading.Tasks;

namespace ApplicationForRunners.SharedClasses
{
    public interface IApplicationPermissions
    {
        bool CheckPermission(string permission);
        bool CheckPermissions(string[] permission);
        Task<bool> AskForPermissions(string[] permissionTab, Action<int> solicitMethod);
    }
}
