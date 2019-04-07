namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// This is a parameter tied to a combi, not used as a regular parameter.
    /// </summary>
    public interface IFixedParameterValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetFixedParameterValue(FixedParameter.EType type);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        void SetFixedParameterValue(FixedParameter.EType type, int value);
    }
}
