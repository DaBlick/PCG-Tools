// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.NewParameters;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.PatchSetLists
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISetListSlot : IPatch, IArtistable, ICompleteInPcgable, IReferencable
    {
        /// <summary>
        /// 
        /// </summary>
        IPatch UsedPatch { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        SetListSlot.PatchType SelectedPatchType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        string Description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        string DescriptionInList { get; }


        /// <summary>
        /// 
        /// </summary>
        int Volume { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int MaxDescriptionLength { get; }


        /// <summary>
        /// 
        /// </summary>
        SetListSlot.TextSize SelectedTextSize { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IIntParameter Color { get; }


        /// <summary>
        /// 
        /// </summary>
        int Transpose { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameter GetParam(ParameterNames.SetListSlotParameterName name);
    }
}
