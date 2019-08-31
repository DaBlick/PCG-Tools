// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;

namespace PcgTools.Model.Common.Synth.PatchInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDirtiable
    {
        /// <summary>
        /// 'Real' file is dirty.
        /// </summary>
        bool IsDirty { get; set; }


        /// <summary>
        /// Backup is dirty.
        /// </summary>
        bool IsBackupDirty { get; set; }



        /// <summary>
        /// Last date/time the file was saved.
        /// </summary>
        DateTime LastSaved { get; }
    }
}
