namespace BuddyLanguage.Domain.Interfaces
{
    public interface ISpeechRecognizer
    {
        Task<string> RecognizeSpeechToTextAsync
            (byte[] voiceMessage, CancellationToken cancellationToken);
    }
}
