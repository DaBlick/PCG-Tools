// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    public abstract class Timbres : ITimbres
    {
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollectionEx<ITimbre> TimbresCollection { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        readonly ICombi _combi;


        /// <summary>
        /// 
        /// </summary>
        public int TimbresPerCombi { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public int TimbresOffset { get; private set; }            


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <param name="timbresPerCombi"></param>
        /// <param name="timbresOffset"></param>
        protected Timbres(ICombi combi, int timbresPerCombi, int timbresOffset)
        {
            Debug.Assert(timbresPerCombi > 0);
            Debug.Assert(timbresOffset > 0);

            TimbresCollection = new ObservableCollectionEx<ITimbre>();

            _combi = combi;
            TimbresPerCombi = timbresPerCombi;
            TimbresOffset = timbresOffset;
            
            Fill();
        }


        /// <summary>
        /// 
        /// </summary>
        void Fill()
        {
            FillTimbres();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract ITimbre CreateNewTimbre(int index );


        /// <summary>
        /// 
        /// </summary>
        void FillTimbres()
        {
            for (var i = 0; i < TimbresCollection.Count; i++)
            {
                for (var index = 0; index < TimbresPerCombi; index++)
                {
                    TimbresCollection.Add(CreateNewTimbre(index));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IMemory Root => _combi.Root;

        // INavigable

        /// <summary>
        /// 
        /// </summary>
        public INavigable Parent => _combi;


        /// <summary>
        /// 
        /// </summary>
        public int ByteOffset { get; set; }
    }
}
