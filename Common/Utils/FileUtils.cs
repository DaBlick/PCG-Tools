// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.IO;

namespace Common.Utils
{
    /// <summary>
    /// File utilities.
    /// </summary>
    public abstract class FileUtils
    {
        /// <summary>
        /// File comparer for creation time.
        /// </summary>
        public class FileAgeComparer : IComparer<string>
        {
            /// <summary>
            /// Compares two file strings.
            /// </summary>
            /// <param name="file1"></param>
            /// <param name="file2"></param>
            /// <returns></returns>
            public int Compare(string file1, string file2)
            {
                var timeStamp1 = new FileInfo(file1).CreationTime;
                var timeStamp2 = new FileInfo(file2).CreationTime;
                return timeStamp1.CompareTo(timeStamp2);
            }
        }
    }
}