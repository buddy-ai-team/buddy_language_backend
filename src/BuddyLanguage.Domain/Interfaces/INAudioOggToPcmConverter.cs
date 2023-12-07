namespace BuddyLanguage.Domain.Interfaces
{
    public interface INAudioOggToPcmConverter
    {
        byte[] ConvertOggToPcm(byte[] oggData);
    }
}
