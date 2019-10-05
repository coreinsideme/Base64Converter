using System;
using System.Collections;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Base64Converter
{
    public class Converter
    {
        private readonly static char[] asciiArray = CreateBase64AsciiDictionary();
        private readonly static char emptyByteSymbol = '=';

        public string Encode(byte[] inputBytes) {
            if (inputBytes.Length == 0) {
                return string.Empty;
            }

            int groupsOfThree = inputBytes.Length / 3 + 1;
            Span<char> span = stackalloc char[groupsOfThree * 4];
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBytes);

            for (int currentGroup = 0; currentGroup < groupsOfThree; currentGroup++)
            {
                var lastIndex = (currentGroup + 1) * 3 > inputSpan.Length ? inputSpan.Length : (currentGroup + 1) * 3;
                var currentChars = inputSpan.Slice(currentGroup * 3, lastIndex - currentGroup * 3);
                
                ConvertChars(currentChars, span.Slice(currentGroup * 4, 4));
            }

            return span.ToString();
        }

        private void ConvertChars(ReadOnlySpan<byte> bytesToConvert, Span<char> convertedChars)
        {
            var lastCharsToReset = 3 - bytesToConvert.Length;

            convertedChars[0] = asciiArray[(bytesToConvert[0] >> 2)];
            convertedChars[1] = asciiArray[(((bytesToConvert[0] & 0b_0000_0011) << 4) | ((lastCharsToReset > 1 ? 0 : bytesToConvert[1]) >> 4))];
            convertedChars[2] = lastCharsToReset > 1 ? emptyByteSymbol : asciiArray[(((bytesToConvert[1] & 0b_0000_1111) << 2) | ((lastCharsToReset > 0 ? 0 : bytesToConvert[2]) >> 6))];
            convertedChars[3] = lastCharsToReset > 0 ? emptyByteSymbol : asciiArray[(bytesToConvert[2] & 0b_0011_1111)];
        }

        private static char[] CreateBase64AsciiDictionary()
        {
            return Enumerable.Range(65, 26) // A-Z
                .Concat(Enumerable.Range(97, 26)) // a-z
                .Concat(Enumerable.Range(48, 10)) //0-9
                .Concat(new int[] { 43, 47 }) // "+", "/"
                .Select(asciiIndex => (char)asciiIndex)
                .ToArray();
        }
    }    
}
