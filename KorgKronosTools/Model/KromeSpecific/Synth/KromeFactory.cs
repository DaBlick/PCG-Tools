// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KromeSpecific.Pcg;
using PcgTools.Model.KromeSpecific.Song;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KromeSpecific.Synth
{
    public class KromeFactory : MFactory
    {
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            PcgMemory pcgMemory = new KromePcgMemory(fileName);
            pcgMemory.Fill();
            return pcgMemory;
        }

        public override IPatchesFileReader CreateFileReader(IPcgMemory pcgMemory, byte[] content)
        {
            return new KromePcgFileReader(pcgMemory, content);
        }

        public override ISongMemory CreateSongMemory(string fileName)
        {
            SongMemory songMemory = new KromeSongMemory(fileName);
            return songMemory;
        }

        public override ISongFileReader CreateSongFileReader(ISongMemory memory, byte[] content)
        {
            return new KromeSongFileReader(memory, content);
        }
    }
}
