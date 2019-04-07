using PcgTools.ViewModels;

namespace PcgTools.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPcgWindow : IWindow
    {
        /// <summary>
        /// 
        /// </summary>
        IViewModel ViewModel { get; }
    }
}
