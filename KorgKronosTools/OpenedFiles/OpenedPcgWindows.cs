using System.Collections.ObjectModel;
using System.Linq;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.OpenedFiles;

namespace PcgTools.Songs
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenedPcgWindows : ObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<OpenedPcgWindow> _items;
 

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<OpenedPcgWindow> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged("Items");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public OpenedPcgWindows()
        {
            Items = new ObservableCollection<OpenedPcgWindow>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        internal void RemoveWindowWithPcgMemory(IMemory memory)
        {
            if (memory is IPcgMemory)
            {
                foreach (var item in Items.Where(item => item.PcgMemory == memory))
                {
                    Items.Remove(item);
                }
            }
        }
    }
}
