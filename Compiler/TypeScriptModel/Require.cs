using System;
using System.Diagnostics;
using TypeScriptModel.ExtensionMethods;

namespace TypeScriptModel
{
    public static class Require
    {
        [DebuggerStepThrough]
        public static void NotNull<T>(T arg, string name) where T : class
        {
            if (arg == null) throw new ArgumentNullException(name);
        }

        [DebuggerStepThrough]
        public static void ValidJavaScriptIdentifier(string arg, string name, bool allowNull = false)
        {
            if (!allowNull && arg == null) throw new ArgumentNullException(name);
            if (arg != null && !arg.IsValidJavaScriptIdentifier()) throw new ArgumentException(name);
        }

        [DebuggerStepThrough]
        public static void ValidJavaScriptNestedIdentifier(string arg, string name, bool allowNull = false)
        {
            if (arg == null)
            {
                if (!allowNull)
                    throw new ArgumentNullException(name);
                return;
            }
            if (!arg.IsValidNestedJavaScriptIdentifier()) throw new ArgumentException(name);
        }
    }
}
