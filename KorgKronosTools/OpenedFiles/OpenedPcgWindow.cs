using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.OpenedFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenedPcgWindow : ObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        private PcgMemory _pcgMemory;


        /// <summary>
        /// 
        /// </summary>
        public PcgMemory PcgMemory
        {
            get
            {
                return _pcgMemory;
            }

            set
            {
                if (_pcgMemory != value)
                {
                    _pcgMemory = value;
                    RaisePropertyChanged("PcgMemory");
                }
            }
        }
    }
}
