using System;
using System.Collections.Generic;
using Common.Mvvm;
using PcgTools.PcgToolsResources;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class Model : ObservableObject, IModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Models.EModelType ModelType { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public Models.EOsVersion OsVersion { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public string OsVersionString { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="osVersion"></param>
        /// <param name="osVersionString"></param>
        public Model(Models.EModelType modelType, Models.EOsVersion osVersion, string osVersionString)
        {
            ModelType = modelType;
            OsVersion = osVersion;
            OsVersionString = osVersionString;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static string ModelTypeAsString(Models.EModelType modelType)
        {
            var map = new Dictionary<Models.EModelType, string>
            {
                {Models.EModelType.Kronos, Strings.EModelTypeKronos},
                {Models.EModelType.Oasys, Strings.EModelTypeOasys},
                {Models.EModelType.Krome, Strings.EModelTypeKrome},
                {Models.EModelType.KromeEx, Strings.EModelTypeKromeEx},
                {Models.EModelType.Kross, Strings.EModelTypeKross},
                {Models.EModelType.Kross2, Strings.EModelTypeKross2},
                {Models.EModelType.M1, Strings.EModelTypeM1},
                {Models.EModelType.M3, Strings.EModelTypeM3},
                {Models.EModelType.M3R, Strings.EModelTypeM3R},
                {Models.EModelType.M50, Strings.EModelTypeM50},
                {Models.EModelType.Ms2000, Strings.EModelTypeMs2000},
                {Models.EModelType.MicroKorgXl, Strings.EModelTypeMicroKorgXl},
                {Models.EModelType.MicroKorgXlPlus, Strings.EModelTypeMicroKorgXlPlus},
                {Models.EModelType.MicroStation, Strings.EModelTypeMicroStation},
                {Models.EModelType.Trinity, Strings.EModelTypeTrinity},
                {Models.EModelType.TritonExtreme, Strings.EModelTypeTritonExtreme},
                {Models.EModelType.TritonTrClassicStudioRack, Strings.EModelTypeTritonTrClassicStudioRack},
                {Models.EModelType.TritonLe, Strings.EModelTypeTritonLE},
                {Models.EModelType.TritonKarma, Strings.EModelTypeKarma},
                {Models.EModelType.TSeries, Strings.EModelTypeTSeries},
                {Models.EModelType.XSeries, Strings.EModelTypeXSeries},
                {Models.EModelType.Z1, Strings.EModelTypeZ1},
                {Models.EModelType.ZeroSeries, Strings.EModelTypeZeroSeries},
                {Models.EModelType.Zero3Rw, Strings.EModelTypeZero3Rw}
            };

            if (map.ContainsKey(modelType))
            {
                return map[modelType];
            }

            throw new ApplicationException("Illegal type");
        }


        /// <summary>
        /// 
        /// </summary>
        public string ModelAsString => ModelTypeAsString(ModelType);


        /// <summary>
        /// 
        /// </summary>
        public string ModelAndVersionAsString => ModelAsString + (string.IsNullOrEmpty(OsVersionString) ? string.Empty : (" " + OsVersionString));
    }
}
