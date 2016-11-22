using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson
{
    public class JsonReadException : Exception
    {
        public JsonReadException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
