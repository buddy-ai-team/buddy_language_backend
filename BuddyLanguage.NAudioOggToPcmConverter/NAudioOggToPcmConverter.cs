// https://github.com/naudio/NAudio/tree/master/Docs
// https://github.com/naudio/Vorbis/tree/master/NAudio.Vorbis
// https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/statements/using
// https://markheath.net/post/naudio-wave-stream-architecture
// https://markheath.net/post/naudio-wavestream-in-depth
// https://markheath.net/post/fully-managed-input-driven-resampling-wdl
// https://markheath.net/post/realtime-transcribe-vosk-twilio
// https://www.twilio.com/blog/transcribe-phone-calls-in-real-time-with-twilio-vosk-and-aspdotnet-core
// https://github.com/naudio/NAudio/tree/master/NAudio.Wasapi
// https://markheath.net/post/how-to-resample-audio-with-naudio

using NAudio.Vorbis;
using NAudio.Wave;

namespace BuddyLanguage.NAudioOggToPcmConverter
{
    public class NAudioOggToPcmConverter
    {
        public byte[] ConvertOggToPcm(byte[] oggData)
        {
            using (var oggStream = new MemoryStream(oggData))
            using (var pcmStream = new MemoryStream())
            {
                using (var readerStream = new VorbisWaveReader(oggStream))
                {
                    var waveFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16bit, mono
                    using (var resampler = new MediaFoundationResampler(readerStream, waveFormat))
                    {
                        WaveFileWriter.WriteWavFileToStream(pcmStream, resampler);
                        return pcmStream.ToArray();
                    }
                }
            }
        }
    }
}
