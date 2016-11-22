using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tools
{
    [DebuggerStepThrough]
    public static class Guard
    {
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        public static void NotNull(object value, string name)
        {
            if (value == null) throw new NullReferenceException(name);
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new NullReferenceException(name);
        }

        public static void NotNullOrWhiteSpace(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new NullReferenceException(name);
        }

        public static void True<TException>(bool condition, string message, Func<string, TException> factory) where TException : Exception
        {
            if (!condition) throw factory(message);
        }
    }
}
