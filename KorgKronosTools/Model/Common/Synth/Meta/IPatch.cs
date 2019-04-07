// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using Common.Mvvm;
using PcgTools.ClipBoard;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPatch : INamable, ISelectable, ILoadable, IClearable, INotificatable, IIndexable,
        ILocatable, IPcgNavigable,  IComparable<IPatch>, IObservableObject, IIsEmptyCheckable, IParameterSettable, 
        IUpdatable
    {
        /// <summary>
        /// 
        /// </summary>
        string Id { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        int CalcByteDifferencesInNameAndDescription(IPatch patch);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="includingName"></param>
        /// <param name="maxDiffs"></param>
        /// <returns></returns>
        int CalcByteDifferences(IPatch patch, bool includingName, int maxDiffs);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="includingName"></param>
        /// <param name="maxDiffs"></param>
        /// <returns></returns>
        int CalcByteDifferences(IClipBoardPatch patch, bool includingName, int maxDiffs);
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useName"></param>
        /// <returns></returns>
        int CalcCrc(bool useName);


        ///
        /// <summary>
        /// 
        /// </summary>
        bool IsFromMasterFile { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignoreInit"></param>
        /// <param name="filterOnText"></param>
        /// <param name="filterText"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="useFavorites"></param>
        /// <param name="filterDescription"></param>
        /// <returns></returns>
        bool UseInList(
            bool ignoreInit, bool filterOnText, string filterText, bool caseSensitive,
            ListGenerator.ListGenerator.FilterOnFavorites useFavorites,
            bool filterDescription);


        /// <summary>
        /// Finds the first duplicate of the patch (excluding itself), or None if not existing (so patch is unique).
        /// </summary>
        IPatch FirstDuplicate { get; }


        //### void ChangeReferences(IPatch newPatch);


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
        bool ToolTipEnabled { get; }


        /// <summary>
        /// 
        /// </summary>
        string ToolTip { get; }
    }
}
