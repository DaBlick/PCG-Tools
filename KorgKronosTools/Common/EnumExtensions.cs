using System;
using System.ComponentModel;
using System.Linq;

namespace PcgTools.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }


        /// <summary>
        /// Returns the attribute like in:
        /// public enum EModule
        /// {
        /// [componentModel.Description(Introducing Extension Methods")]
        /// Intro,
        /// Advanced
        /// ...
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            var descriptionAttribute = fieldInfo.GetCustomAttributes(
                typeof (DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return descriptionAttribute == null
                ? value.GetName()
                : descriptionAttribute.Description;
        }
    }
}
