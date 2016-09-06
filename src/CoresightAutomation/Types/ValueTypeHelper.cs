using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.Types
{
    public static class ValueTypeHelper
    {
        public static bool TypeIsSupported(string typeName)
        {
            return !TypeIsArray(typeName);
        }

        public static bool TypeIsArray(string typeName)
        {
            return typeName.EndsWith("Array", StringComparison.OrdinalIgnoreCase);
        }

        public static bool TypeIsNumeric(string typeName)
        {
            return _numericTypeNames.Contains(typeName);
        }

        private static readonly HashSet<string> _numericTypeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Byte",
            "Double",
            "Int16",
            "Int32",
            "Int64",
            "Single"
        };


    }
}
