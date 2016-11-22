using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson
{
    public class JsonSettings
    {
        public static readonly JsonSettings Default = new JsonSettings();

        public bool CamelCase { get; set; }

        internal IEqualityComparer<StringChunk> Comparer => CamelCase ? StringChunk.Comparer.CamelCase : StringChunk.Comparer.Default;
    }
}
