using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    /// <summary>
    /// Represents an interface for performing text-to-speech synthesis operations.
    /// </summary>
    public interface ITextToSpeech
    {
        /// <summary>
        /// Asynchronously converts text to a WAV audio byte array using the specified language, voice, and cancellation token.
        /// </summary>
        /// <param name="text">The text to be synthesized into speech.</param>
        /// <param name="language">The language of the voice.</param>
        /// <param name="voice">The desired voice for synthesis.</param>
        /// <param name="speed">The desired voice speed for synthesis.</param>
        /// <param name="cancellationToken">A CancellationToken for possible cancellation of the operation.</param>
        /// <exception cref="SpeechSynthesizingException">Thrown when an error occurs during speech synthesis.</exception>
        /// <returns>A Task representing the asynchronous operation, returning a byte array containing the synthesized audio.</returns>
        Task<byte[]> TextToWavByteArrayAsync(string text, Language language, Voice voice, TtsSpeed speed, CancellationToken cancellationToken);
    }
}
