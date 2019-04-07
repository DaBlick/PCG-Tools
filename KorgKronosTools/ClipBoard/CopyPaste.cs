namespace PcgTools.ClipBoard
{
    /// <summary>
    /// Duplicate algorithm (see PcgViewModel).
    /// Ignore characters for duplication (setting IgnoreCharactersForPatchDuplication) can contains commas to check
    /// for multiple possible fragments.
    /// </summary>
    public class CopyPaste
    {
        /// <summary>
        /// 
        /// </summary>
        public enum PatchDuplication
        {
            /// <summary>
            /// Do not check patch names for checking if patches are duplicates.
            /// </summary>
            DoNotUsePatchNames,

            /// <summary>
            /// Patch names with identical names are considerate duplicates (assuming there is no byte wise duplicate). 
            /// </summary>
            EqualNames,

            /// <summary>
            /// Patch names with like-named names are considerate duplicates (assuming there is no byte wise duplicate 
            /// and no equal name. Ignore characters can be used to check if a name is like named. 
            /// </summary>
            LikeNamedNames
        }
    }
}
