using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tools
{
    internal unsafe struct StringChunk
    {
        internal class Comparer : IEqualityComparer<StringChunk>
        {
            internal static readonly Comparer Default = new Comparer(false);
            internal static readonly Comparer CamelCase = new Comparer(true);

            private readonly bool _camelCase;

            private Comparer(bool camelCase)
            {
                _camelCase = camelCase;
            }

            public bool Equals(StringChunk x, StringChunk y)
            {
                if (x._end - x._start != y._end - y._start) return false;
                if (x._pointer[x._start] != y._pointer[y._start] &&
                    (_camelCase && char.ToLower(x._pointer[x._start]) != char.ToLower(y._pointer[y._start]))) return false;
                for (var i = 0; i < 1 + x._end - x._start; i++)
                {
                    if (x._pointer[x._start + i] != y._pointer[y._start + i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(StringChunk obj)
            {
                throw new NotImplementedException("Should not be called");
            }
        }

        internal readonly char* _pointer;
        internal readonly int _start;
        internal readonly int _end;
        internal readonly bool _anyEscape;

        internal readonly bool IsValid;

        internal StringChunk(char* pointer, int start, int end, bool anyEscape)
        {
            _pointer = pointer;
            _start = start;
            _end = end;
            _anyEscape = anyEscape;
            IsValid = true;
        }

        internal StringChunk(string value)
        {
            _pointer = (char*)GCHandle.Alloc(value, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
            _start = 0;
            _end = value.Length - 1;
            _anyEscape = false;
            IsValid = true;
        }

        public override string ToString()
        {
            if (_anyEscape)
                throw new NotImplementedException();
            return new string(_pointer, _start, 1 + _end - _start);
        }

        public bool Equals(StringChunk other)
        {
            throw new NotImplementedException("Should not be called");
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException("Should not be called");
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException("Should not be called");
        }
    }
}
