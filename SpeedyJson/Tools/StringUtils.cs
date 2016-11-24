using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tools
{
    public static class StringUtils
    {
        internal const int Backslash = '\\';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int ReadJsonString(char* pointer, int length, int position, bool quoted, out bool anyEscape)
        {
            anyEscape = false;
            bool backslash = false;
            while (position < length - 1)
            {
                var c = pointer[position];
                if (!backslash && c == Backslash)
                {
                    backslash = true;
                    anyEscape = true;
                    position++;
                    continue;
                }
                else if (backslash)
                {
                    throw new NotImplementedException();
                    //switch (pointer[position])
                    //{
                    //    case '"':
                    //    case '\\':
                    //    case '/': pointer[position++] = pointer[position]; break;
                    //    case 'b':
                    //    case 'u':
                    //    case 'f': throw new NotImplementedException($"Escaped {pointer[position]} is not implemented yer.");
                    //    case 'n': pointer[position++] = '\n'; break;
                    //    case 'r': pointer[position++] = '\r'; break;
                    //    case 't': pointer[position++] = '\t'; break;
                    //    default: throw new JsonReadException($"Unknown escaped character {pointer[position]}.");
                    //}
                    //position++;
                }
                else
                {
                    if ((quoted && c == '"') ||
                        (!quoted && (char.IsWhiteSpace(c) || c == ',' || c == '{' || c == '}' || c == '[' || c == ']'))) return position - 1;
                }
                backslash = false;
                position++;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static int SkipWhitespaces(char* pointer, int length, int position)
        {
            while (position < length - 1 && char.IsWhiteSpace(pointer[position]))
            {
                position++;
            }
            return position;
        }
    }
}
