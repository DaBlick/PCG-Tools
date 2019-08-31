// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;

namespace Common.Utils
{
    public abstract class MathUtils
    {
        public static int ClipValue(int valueToClip, int minValue, int maxValue)
        {
            Debug.Assert(minValue <= maxValue);
            return Math.Max(minValue, Math.Min(valueToClip, maxValue));
        }
                

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToMap"></param>
        /// <param name="minSourceRange"></param>
        /// <param name="maxSourceRange"></param>
        /// <param name="minDestinationRange"></param>
        /// <param name="maxDestinationRange"></param>
        /// <returns></returns>
        public static int MapValue(int valueToMap, int minSourceRange, int maxSourceRange, int minDestinationRange, int maxDestinationRange)
        {
            return (int)((valueToMap - minSourceRange) * 
                (float)(maxDestinationRange - minDestinationRange) / 
                (float)(maxSourceRange      - minSourceRange     ) + minDestinationRange + 0.5);
        }
    }
}
