// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITimbre : ILoadable, IClearable, ILocatable, INavigable, ISelectable,
        IIndexable, INotificatable, IFixedParameterValue, IParameterSettable, IObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        IProgramBank UsedProgramBank { get; }


        /// <summary>
        /// 
        /// </summary>
        IProgram UsedProgram { get; set; }


        /// <summary>
        /// 
        /// </summary>
        string ColumnProgramId { get; }


        /// <summary>
        /// 
        /// </summary>
        string ColumnProgramName { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTimbre"></param>
        void Swap(ITimbre otherTimbre);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromTimbre"></param>
        void CopyFrom(ITimbre fromTimbre);


        /// <summary>
        /// 
        /// </summary>
        int TimbresOffset { get; }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        int GetInt(int offset, int length);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        void SetInt(int offset, int length, int value);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameter GetParam(ParameterNames.TimbreParameterName name);


        /// <summary>
        /// Size
        /// </summary>
        int TimbresSize { get; set; }

    }
}
