using System;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.ViewModels
{
    public interface ICombiViewModel : IViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        ICombi Combi { get; }


        /// <summary>
        /// 
        /// </summary>
        Action UpdateUiContent { get; }
    }
}
