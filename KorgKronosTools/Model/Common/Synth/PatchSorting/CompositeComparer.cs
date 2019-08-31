// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Linq;

namespace PcgTools.Model.Common.Synth.PatchSorting
{
    /// <summary>
    /// Class for comparing multiple keys.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositeComparer<T> : Comparer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<IComparer<T>> Comparers { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public CompositeComparer()
        {
            Comparers = new List<IComparer<T>>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(T x, T y)
        {
            return Comparers.Select(comparer => comparer.Compare(x, y)).FirstOrDefault(result => result != 0);
        }
    }


}
