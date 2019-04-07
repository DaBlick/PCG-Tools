using Common.Mvvm;
using Common.Utils;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.NewParameters
{
    /// <summary>
    /// 
    /// </summary>
    public class IntParameterBitsInByte : ObservableObject, IIntParameter
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IPatch _patch;


        /// <summary>
        /// 
        /// </summary>
        private readonly int _byteOffset;


        /// <summary>
        /// 
        /// </summary>
        private readonly int _highBit;


        /// <summary>
        /// 
        /// </summary>
        private readonly int _lowBit;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="patch"></param>
        /// <param name="byteOffset"></param>
        /// <param name="highBit"></param>
        /// <param name="lowBit"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        public IntParameterBitsInByte(string name, 
            IPatch patch, int byteOffset, int highBit, int lowBit, int minRange, int maxRange)
        {
            Name = Name;
            _patch = patch;
            _byteOffset = _patch.ByteOffset + byteOffset;
            _highBit = highBit;
            _lowBit = lowBit;
            MinRange = minRange;
            MaxRange = maxRange;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Value
        {
            get { return BitsUtil.GetBits(_patch.Root.Content, _byteOffset, _highBit, _lowBit); }

            set
            {
                if (Value != value)
                {
                    _patch.Root.IsDirty = true;
                    BitsUtil.SetBits(_patch.Root.Content, _byteOffset, _highBit, _lowBit, value);
                    _patch.RaisePropertyChanged(Name, false); 
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public int MinRange { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public int MaxRange { get; private set; }
    }
}
