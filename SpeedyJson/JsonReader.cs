using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson
{
    internal unsafe class JsonReader
    {
        private readonly char* _pointer;
        private int _length;
        private int _position;

        internal JsonReader(char* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
            _position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipWhitespaces()
        {
            _position = StringUtils.SkipWhitespaces(_pointer, _length, _position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StringChunk TryReadString()
        {
            SkipWhitespaces();
            if (_pointer[_position] != '"')
                return default(StringChunk);
            var start = ++_position;
            bool anyEscape;
            var end = StringUtils.ReadJsonString(_pointer, _length, _position, out anyEscape);
            if (end == -1)
                throw new JsonReadException($"Unbalanced string quotes.");
            _position = end + 2;
            return new StringChunk(_pointer, start, end, anyEscape);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            var result = TryReadString();
            if (!result.IsValid)
                throw new JsonReadException("Input is not a valid Json.");
            return result.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringChunk ReadMemberName()
        {
            int position = _position;
            var result = TryReadString();
            if (!result.IsValid)
                throw new JsonReadException($"Unable to read member name at position {position}.");
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadAssignment()
        {
            SkipWhitespaces();
            if (_pointer[_position] != ':')
                throw new JsonReadException($"Missing assignment in the Json value.");
            _position++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadNextChar()
        {
            _position++;
            SkipWhitespaces();
            return _pointer[_position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char CurrentChar()
        {
            SkipWhitespaces();
            return _pointer[_position];
        }

        public void ThrowError(string pattern)
        {
            throw new JsonReadException(string.Format(pattern, _position));
        }
    }
}
