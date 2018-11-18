using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;

namespace ApplicationForRunners.Droid
{
    class SolicitPermission
    {
        public int id;
        public Activity solicitReference;
        public Action<int> solicitMethod;
        public string[] solicitString;

        public SolicitPermission(int idC, string[]solicitStringC, Activity solicitReferenceC, Action<int> solicitMethodC)
        {
            id = idC;
            solicitString = solicitStringC;
            solicitReference = solicitReferenceC;   
            solicitMethod = solicitMethodC;

        }

        public static SolicitPermission GetSolicitPermission(int idCheck, string[] solicitStringCheck, List<SolicitPermission> permissionRequest)
        {
            for (int i = 0; i < permissionRequest.Count(); i++)
            {
                if (permissionRequest[i].id == idCheck)
                {
                    for (int x = 0; x < permissionRequest[i].solicitString.Length; x++)
                        if (solicitStringCheck.Length < 1 || !permissionRequest[i].solicitString[x].SequenceEqual(solicitStringCheck[x]))
                            return null;

                    return permissionRequest[i];
                }
            }
            return null;
        }
    }
}

