namespace BuddyLanguage.Domain.Interfaces
{
    public interface IOggOpusToPcmConverter
    {
        Task<byte[]> ConvertOggToPcm(byte[] oggData);
    }
}
