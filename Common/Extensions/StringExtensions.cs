// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToXml(this string str)
        {
            var newString = str.Aggregate("", (current, ch) => current + (ch < 32 ? ' ' : ch));

            return newString.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").
                Replace("'", "&apos;").Replace("\"", "&quot;").Replace((char) 0x00, ' ');
        }


        /// <summary>
        /// Count different number of characters, e.g. 'Man' and 'map' will result in 2 diffs, 'Ear'
        ///  and 'earth' also in 3.
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static int CountDiffs(this string str1, string str2)
        {
            var diffs = 0; // Math.Abs(str.Length - str2.Length);
            for (var index = 0; index < Math.Min(str1.Length, str2.Length); index++)
            {
                diffs += (str1[index] != str2[index]) ? 1 : 0;
            }

            return diffs;
        }


        /// <summary>
        /// Returns true if both files names are equal (and point to the same file).
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        public static bool IsEqualFileAs(this string fileName1, string fileName2)
        {
            return ((fileName2 != string.Empty) && 
                (string.Equals(Path.GetFullPath(fileName1), Path.GetFullPath(fileName2), 
                StringComparison.CurrentCultureIgnoreCase)));
        }


        /// <summary>
        /// Returns the number of selectedChar around startIndex (not including that index).
        /// E.g. "Text , Text2".CountCharsAroundIndex(5, ' ') returns 2.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="selectedChar"></param>
        /// <returns></returns>
        public static int CountCharsAroundIndex(this string text, int startIndex, char selectedChar)
        {
            var charsFound = 0;

            // Search backwards.
            for (var index = startIndex - 1; index >= 0; index--)
            {
                if (text[index] == selectedChar)
                {
                    charsFound++;
                }
                else
                {
                    break;
                }
            }

            // Search forwards.
            for (var index = startIndex + 1; index < text.Length; index++)
            {
                if (text[index] == selectedChar)
                {
                    charsFound++;
                }
                else
                {
                    break;
                }
            }

            return charsFound;
        }


        /// <summary>
        /// Expand Inserts spaces before capitals (except the first or if there is already a space) after trimming.
        /// E.g. "ThisIsAText ".Expand() returns "This Is A Text".
        /// </summary>
        /// <returns></returns>
        public static string Expand(this string text)
        {
            text = text.Trim();

            var builder = new StringBuilder();

            for (var index = 0; index < text.Length; index++)
            {
                var character = text[index];
                if ((index > 0) && (text[index - 1] != ' ') && (character >= 'A') && (character <= 'Z'))
                {
                    builder.Append(' ');
                }

                builder.Append(character);
            }

            return builder.ToString();
        }


        /// <summary>
        /// Removes  spaces starting from the end until the shrinkAmount is reached.
        /// A non capital character after the removed space is converted into a capital.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="shrinkAmount"></param>
        /// <returns></returns>
        public static string Shrink(this string text, int shrinkAmount)
        {
            var result = text.Trim();

            var shrinked = 0;
            for (var index = result.Length; index >= 0; index++)
            {
                if (result[index] == ' ')
                {
                    result = result.Remove(index, 1);

                    if ((result[index] >= 'a') && (result[index] <= 'z'))
                    {
                        result = result.Remove(index, 1);
                        result = result.Insert(index, ('A' + result[index] - 'a').ToString()); // Change to capital.
                    }

                    shrinked++;
                    if (shrinked == shrinkAmount)
                    {
                        break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Removes a string from the end (or returns string as is if not existing).
        /// Does not work to remove e.g. a '\n' at end of file, use in that case:
        ///  some_string.Substring(0, some_string.Length - 1); or RemoveLastNewLine();
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fragmentToRemove"></param>
        public static string RemoveFromEnd(this string text, string fragmentToRemove)
        {
            if ((!string.IsNullOrWhiteSpace(fragmentToRemove) && text.EndsWith(fragmentToRemove.Trim())))
            {
                return text.Remove(text.Length - fragmentToRemove.Length);
            }

            return text;
        }


        /// <summary>
        /// Returns the last new line if present.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveLastNewLine(this string text)
        {
            return text.EndsWith("\n") ? text.Substring(0, text.Length - 1) : text;
        }
    }
}
