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

using BuddyLanguage.Domain.Interfaces;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace BuddyLanguage.NAudioOggToWavConverter
{
    public class NAudioOggToPcmConverter : INAudioOggToPcmConverter
    {
        public byte[] ConvertOggToPcm(byte[] oggData)
        {
            using var oggStream = new MemoryStream(oggData);
            using var vorbis = new VorbisWaveReader(oggStream);
            var resampler = new WdlResamplingSampleProvider(vorbis, 16000);

            // Converts from float to 16-bit PCM
            var to16Bit = new WaveFloatTo16Provider(resampler.ToWaveProvider());

            // Converts to mono
            var mono = new StereoToMonoProvider16(to16Bit) { LeftVolume = 0.5f, RightVolume = 0.5f };

            // Create a new MemoryStream to store the converted PCM data
            using var pcmStream = new MemoryStream();

            // Write the converted PCM data to the MemoryStream
            using (var pcmWriter = new WaveFileWriter(pcmStream, mono.WaveFormat))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = mono.Read(buffer, 0, buffer.Length)) > 0)
                {
                    pcmWriter.Write(buffer, 0, bytesRead);
                }
            }

            return pcmStream.ToArray();
        }
    }
}
