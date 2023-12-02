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
using NAudio.Wave.SampleProviders;

namespace BuddyLanguage.NAudioOggToWavConverter
{
    public class NAudioOggToWavConverter
    {
        public byte[] ConvertOggToWav(byte[] oggData)
        {
            using (var oggStream = new MemoryStream(oggData))
            using (var pcmStream = new MemoryStream())
            {
                using (var readerStream = new VorbisWaveReader(oggStream))
                {
                    // 16kHz, 16bit, mono
                    var resampler = new WdlResamplingSampleProvider(readerStream.ToSampleProvider(), 16000);
                    var to16Bit = new WaveFloatTo16Provider(resampler.ToWaveProvider());
                    var mono = new StereoToMonoProvider16(to16Bit);

                    WaveFileWriter.WriteWavFileToStream(pcmStream, mono);
                    return pcmStream.ToArray();
                }
            }
        }
    }
}
