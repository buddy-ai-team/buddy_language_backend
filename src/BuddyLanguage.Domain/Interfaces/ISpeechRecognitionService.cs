using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface ISpeechRecognitionService
    {
        /// <summary>
        ///  Recognize speech to text
        /// </summary>
        /// <param name="voiceMessage">
        /// Supported formats: flac, m4a, mp3, mp4, mpeg, mpga, oga, ogg, wav, webm
        /// </param>
        /// <param name="format">The audio format</param>
        /// <param name="nativeLanguage">The user's native language</param>
        /// <param name="studiedLanguage">The user's target language</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Recognized text</returns>
        Task<string> RecognizeSpeechToTextAsync(
            byte[] voiceMessage,
            AudioFormat format,
            Language nativeLanguage,
            Language studiedLanguage,
            CancellationToken cancellationToken);
    }
}
