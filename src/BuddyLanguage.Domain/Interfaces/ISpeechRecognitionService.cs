namespace BuddyLanguage.Domain.Interfaces
{
    public interface ISpeechRecognitionService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="voiceMessage">
        /// Supported formats: flac, m4a, mp3, mp4, mpeg, mpga, oga, ogg, wav, webm
        /// </param>
        /// <param name="fileName">
        /// The name of the file with the extension
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        Task<string> RecognizeSpeechToTextAsync
            (byte[] voiceMessage, string fileName, CancellationToken cancellationToken);

        //
    }
}
