using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Vorbis;
using NAudio.Wave;

namespace BuddyLanguage.NAudioOggToWavConverter
{
    internal class NAudioOggToWavConverter
    {
        public byte[] ConvertOggToWav(byte[] oggData)
        {
            var oggStream = new MemoryStream(oggData);
            var wavStream = new MemoryStream();
            var vorbis = new VorbisWaveReader(oggStream);
            WaveFileWriter.WriteWavFileToStream(wavStream, vorbis);
            return wavStream.ToArray();
        }
    }
}
