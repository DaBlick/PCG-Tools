// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KromeExExSpecific.Song;
using PcgTools.Model.KromeExSpecific.Pcg;
using PcgTools.Model.KromeExSpecific.Song;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KromeExSpecific.Synth
{
    public class KromeExFactory : MFactory
    {
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            PcgMemory pcgMemory = new KromeExPcgMemory(fileName);
            pcgMemory.Fill();
            return pcgMemory;
        }

        public override IPatchesFileReader CreateFileReader(IPcgMemory pcgMemory, byte[] content)
        {
            return new KromeExPcgFileReader(pcgMemory, content);
        }

        public override ISongMemory CreateSongMemory(string fileName)
        {
            SongMemory songMemory = new KromeExSongMemory(fileName);
            return songMemory;
        }

        public override ISongFileReader CreateSongFileReader(ISongMemory memory, byte[] content)
        {
            return new KromeExSongFileReader(memory, content);
        }
    }
}
