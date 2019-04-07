namespace PcgTools.Model.Common.Synth.NewParameters
{
    /// <summary>
    /// /
    /// </summary>
    public interface IIntParameter
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }


        /// <summary>
        /// 
        /// </summary>
        int Value { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int MinRange { get; }


        /// <summary>
        /// 
        /// </summary>
        int MaxRange { get; }


    }
}
