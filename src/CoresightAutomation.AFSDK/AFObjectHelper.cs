using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace CoresightAutomation.AFSDK
{
    public static class AFObjectHelper
    {
        public static T Resolve<T>(string absolutePath) where T : AFObject
        {
            T afObject = AFObject.FindObject(absolutePath) as T;
            if (afObject == null)
            {
                throw new ArgumentException("Could not resolve the specified " + typeof(T).Name);
            }
            return afObject;
        }

    }
}
