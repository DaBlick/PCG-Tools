// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.PcgToolsResources;

namespace PcgTools.Model.Common.Synth.PatchPrograms
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ProgramBank : Bank<Program>, IProgramBank
    {
        /// <summary>
        /// When changed, also change IsModeled.
        /// </summary>
        public enum SynthesisType
        {
            // Sampled types
            Ai,             // M1 sample engine, Advanced Integrated
            Ai2,            // Advanted Integrated 2
            Access,         // Trinity Sample engine
            Hi,             // Triton/Karma Sample engine
            Eds,            // M3/M50 Sample engine
            Edsi,           // MicroStation Sample engine
            Edsx,           // Krome (EX)/Kross(2) Sample engine
            Hd1,            // Kronos/Oasys Sample engine

            // Modeled types
            // Prophecy,       // Trinity option SOLO-TRI
            AnalogModeling, // MS2000, MicroKorg
            Mmt,            // MicroKorg XL (Plus)
            MossZ1,         // Trinity option MOSS-TRI or MOSS board for Korg Triton (except LE)
            Radias,         // M3 option
            Exi,            // Oasys/Kronos modeled engine

            Last,

            Unknown         // Unknown; Used for Oasys/Kronos where synthesis type is dynamic.
        }


        /// <summary>
        /// 
        /// </summary>
        public const SynthesisType FirstModeledSynthesisType = SynthesisType.AnalogModeling;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="synthesisType"></param>
        /// <returns></returns>
        public static string SynthesisTypeAsString(SynthesisType synthesisType)
        {
            var map = new Dictionary<SynthesisType, string>
            {
                {SynthesisType.Ai, Strings.ESynthesisTypeAi},
                {SynthesisType.Ai2, Strings.ESynthesisTypeAi2},
                {SynthesisType.Access, Strings.ESynthesisTypeAccess},
                {SynthesisType.Hi, Strings.ESynthesisTypeHi},
                {SynthesisType.Eds, Strings.ESynthesisTypeEds},
                {SynthesisType.Edsi, Strings.ESynthesisTypeEdsi},
                {SynthesisType.Edsx, Strings.ESynthesisTypeEdsx},
                {SynthesisType.Mmt, Strings.ESynthesisTypeMmt},
                {SynthesisType.AnalogModeling, Strings.ESynthesisTypeAnalogModeling},
                {SynthesisType.MossZ1, Strings.ESynthesisTypeMossZ1},
                {SynthesisType.Radias, Strings.ESynthesisTypeRadias},
                {SynthesisType.Hd1, Strings.ESynthesisTypeHd1},
                {SynthesisType.Exi, Strings.ESynthesisTypeExi}
            };

            if (map.ContainsKey(synthesisType))
            {
                return map[synthesisType];
            }

            throw new ApplicationException("Illegal case");
        }


        /// <summary>
        /// 
        /// </summary>
        public SynthesisType BankSynthesisType
        {
            get; 
            set;
        }


        /// <summary>
        /// Returns the default modeled synthesis type, e.g. EXI for Kronos.
        /// </summary>
        public abstract SynthesisType DefaultModeledSynthesisType { get; }


        /// <summary>
        /// Returns the default sampled synthesis type, e.g. EDS for Kronos.
        /// </summary>
        public abstract SynthesisType DefaultSampledSynthesisType { get; }


        /// <summary>
        /// 
        /// </summary>
        public bool IsModeled => Program.IsModeled(BankSynthesisType);


        /// <summary>
        /// Used in XAML PCG Window in list view column.
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public string Column2 => SynthesisTypeAsString(BankSynthesisType);


        /// <summary>
        /// 
        /// </summary>
        /// // ReSharper disable once UnusedMember.Global
        [UsedImplicitly]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        string Description { [UsedImplicitly] get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        /// <param name="synthesisType"></param>
        /// <param name="description"></param>
        protected ProgramBank(IBanks programBanks, BankType.EType type, string id, int pcgId, SynthesisType synthesisType, 
            string description) 
            : base(programBanks, type, id, pcgId)
        {
            BankSynthesisType = synthesisType;
            Description = description;
        }


        /// <summary>
        /// CountPatches filled patches (except GM banks).
        /// </summary>
        public override int CountFilledPatches
        {
            get { return Patches.Count(patch => ((IBank) (patch.Parent)).IsLoaded && (Type != BankType.EType.Gm)); }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return "n.a."; } set { throw new ApplicationException("Not yet implemented"); } }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Strings.Bank_2str} {Id}";
        }
    }
}
