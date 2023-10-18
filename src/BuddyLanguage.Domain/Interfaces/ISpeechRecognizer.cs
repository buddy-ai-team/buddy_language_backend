namespace BuddyLanguage.Domain.Interfaces
{
    public interface ISpeechRecognizer
    {
        /// <summary>
        /// Supported formats of voiceMessage: flac, m4a, mp3, mp4, mpeg, mpga, oga, ogg, wav, webm
        /// </summary>
        /// <param name="voiceMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> RecognizeSpeechToTextAsync
            (byte[] voiceMessage, CancellationToken cancellationToken);
    }
}
