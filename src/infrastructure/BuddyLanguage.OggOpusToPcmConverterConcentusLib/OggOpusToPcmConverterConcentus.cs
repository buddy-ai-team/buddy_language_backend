using BuddyLanguage.Domain.Interfaces;
using Concentus.Oggfile;
using Concentus.Structs;

namespace BuddyLanguage.OggOpusToPcmConverterConcentusLib;

public class OggOpusToPcmConverterConcentus : IOggOpusToPcmConverter
{
    public Task<byte[]> ConvertOggToPcm(byte[] oggData)
    {
        using var oggStream = new MemoryStream(oggData);
        using var pcmStream = new MemoryStream();
        var opusDecoder = new OpusDecoder(16000, 1);
        var reader = new OpusOggReadStream(opusDecoder, oggStream);

        while (reader.HasNextPacket)
        {
            short[] packet = reader.DecodeNextPacket();
            if (packet != null)
            {
                byte[] pcmBytes = ConvertShortArrayToByteArray(packet);
#pragma warning disable VSTHRD103
                pcmStream.Write(pcmBytes, 0, pcmBytes.Length);
#pragma warning restore VSTHRD103
            }
        }

        return Task.FromResult(pcmStream.ToArray());
    }

    private byte[] ConvertShortArrayToByteArray(short[] shortArray)
    {
        byte[] byteArray = new byte[shortArray.Length * sizeof(short)];
        Buffer.BlockCopy(shortArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }
}
