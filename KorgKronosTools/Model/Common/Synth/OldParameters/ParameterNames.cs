namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// Contains parameter names.
    /// </summary>
    public class ParameterNames
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ProgramParameterName
        {
            OscMode, 
            Mode,
            
            Favorite,

            Category,
            SubCategory,
            

            DrumTrackCommonPatternBank,
            DrumTrackCommonPatternNumber,

            DrumTrackProgramBank,
            DrumTrackProgramNumber,
            
        }


        /// <summary>
        /// 
        /// </summary>
        public enum CombiParameterName
        {
            Category,
            SubCategory,
            Favorite,
            Tempo,

            DrumTrackCommonPatternBank,
            DrumTrackCommonPatternNumber
        }


        /// <summary>
        /// 
        /// </summary>
        public enum TimbreParameterName 
        {
            Status,
            Mute,
            OscMode,
            OscSelect,
            Priority,

            MidiChannel,
            
            Volume,
            
            BottomKey,
            TopKey,
            
            BottomVelocity,
            TopVelocity,

            BendRange,
            Portamento,
            Transpose,
            Detune
        }


        /// <summary>
        /// 
        /// </summary>
        public enum SetListSlotParameterName
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public enum DrumKitParameterName
        {
            Category
        }


        /// <summary>
        /// 
        /// </summary>
        public enum DrumPatternParameterName
        {

        }


        /// <summary>
        /// 
        /// </summary>
        public enum WaveSequencetParameterName
        {

        }
    }
}