// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Text.RegularExpressions;
using PcgTools.PcgToolsResources;

namespace PcgTools.Edit
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EditUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ECheckType
        {
            Name,
            Description,
            SplitCharacter
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="checkType"></param>
        /// <returns></returns>
        public static string CheckText(string text, int maxLength, ECheckType checkType)
        {
            if (text.Length > maxLength)
            {
// ReSharper disable RedundantStringFormatCall
                return string.Format($"{checkType} too long");
// ReSharper restore RedundantStringFormatCall
            }

            switch (checkType)
            {
                case ECheckType.SplitCharacter:
                    // Space, a-Z, A-Z, 0-9 are illegal too.
                    if (!Regex.IsMatch(text, @"^[\!\@\#\-\%\+\&\*\'\""\:\<\>\?\,\.\/\~\$\^\/\[\]\{\}\(\)_\=\`\|]*$"))
                    {
                        return string.Format(Strings.CheckTypeContainsIllegalCharacters, checkType);
                    }
                    break;

                case ECheckType.Name:
                    if (!Regex.IsMatch(text, 
                        @"^[a-zA-Z0-9 \!\@\#\-\%\+\&\*\'\""\:\<\>\?\,\.\/\~\$\^\/\[\]\{\}\(\)_\=\`\|]*$"))
                    {
                        return string.Format(Strings.CheckTypeContainsIllegalCharacters, checkType);
                    }
                    break;

                case ECheckType.Description:
                {
                    // Check is same as name check, except for two additional characters: return (\r\n).
                    if (!Regex.IsMatch(text, 
                        @"^[a-zA-Z0-9 \!\@\#\-\%\+\&\*\'\""\:\<\>\?\,\.\/\~\$\^\/\[\]\{\}\(\)_\=\`\|\n\r]*$"))
                    {
                        return string.Format(Strings.CheckTypeContainsIllegalCharacters, checkType);
                    }
                    break;
                }

                default:
                {
                    throw new ApplicationException("Illegal switch");
                }
            }

            return string.Empty;
        }
    }
}