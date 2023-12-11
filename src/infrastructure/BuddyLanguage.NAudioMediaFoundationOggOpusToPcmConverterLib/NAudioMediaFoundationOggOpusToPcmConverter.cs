using System.Diagnostics.CodeAnalysis;
using BuddyLanguage.Domain.Interfaces;
using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Wave;

namespace BuddyLanguage.NAudioMediaFoundationOggOpusToPcmConverterLib;

public class NAudioMediaFoundationOggOpusToPcmConverter : IOggOpusToPcmConverter
{
    private const int DefaultSampleRate = 48000; // Assuming the original sample rate is 48 kHz
    private const int TargetSampleRate = 16000; // 16 kHz target sample rate
    private const int ChannelCount = 1; // Mono

    [SuppressMessage(
        "Usage",
        "VSTHRD103:Call async methods when in an async method",
        Justification = "This is synchronous code")]
    public Task<byte[]> ConvertOggToPcm(byte[] oggData)
    {
        using (var oggStream = new MemoryStream(oggData))
        {
            var pcmData = ExtractOpusFromOgg(oggStream);
            return Task.FromResult(ResamplePcm(pcmData));
        }
    }

    private byte[] ExtractOpusFromOgg(MemoryStream oggStream)
    {
        using (var tempStream = new MemoryStream())
        {
            var decoder = new OpusDecoder(DefaultSampleRate, ChannelCount);
            var opusOggReadStream = new OpusOggReadStream(decoder, oggStream);

            while (opusOggReadStream.HasNextPacket)
            {
                short[] packet = opusOggReadStream.DecodeNextPacket();
                if (packet != null)
                {
                    byte[] pcmData = new byte[packet.Length * sizeof(short)];
                    Buffer.BlockCopy(packet, 0, pcmData, 0, pcmData.Length);
                    tempStream.Write(pcmData, 0, pcmData.Length);
                }
            }

            return tempStream.ToArray();
        }
    }

    private byte[] ResamplePcm(byte[] pcmData)
    {
        using var pcmStream = new MemoryStream(pcmData);
        using var waveStream = new RawSourceWaveStream(pcmStream, new WaveFormat(DefaultSampleRate, 16, ChannelCount));
        using var resampler = new MediaFoundationResampler(waveStream, new WaveFormat(TargetSampleRate, 16, ChannelCount));
        resampler.ResamplerQuality = 60; // Set the quality of the resampler
        using var finalStream = new MemoryStream();
        WaveFileWriter.WriteWavFileToStream(finalStream, resampler);
        return finalStream.ToArray();
    }
}
