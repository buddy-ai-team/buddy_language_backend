using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NAudio.Lame;
using NAudio.Wave;

namespace BuddyLanguage.TextToSpeech
{
    public class AzureTextToSpeech : ITextToSpeech
    {
        private readonly ILogger<AzureTextToSpeech> _logger;

        public AzureTextToSpeech(ILogger<AzureTextToSpeech> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task TextToMP3FileAsync(string text, string outputFilePath, SynthesisVoices voice)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(outputFilePath);
            ArgumentNullException.ThrowIfNull(voice);

            if(outputFilePath.Contains(".mp3"))
            {
                outputFilePath = outputFilePath.Replace(".mp3", ".wav");
            }

            var speechKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");
            var speechRegion = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION");

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechSynthesisVoiceName = GetSynthesisVoiceNameFromEnum(voice);

            var audioConfig = AudioConfig.FromWavFileOutput(outputFilePath);

            using (var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig))
            {
                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        _logger.LogInformation("Speech synthesized to file.");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        _logger.LogInformation($"Speech synthesis canceled. Reason: {cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            _logger.LogInformation($"Speech synthesis error. ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}");
                        }
                    }
                }
            }

            var mp3path = outputFilePath.Replace(".wav", ".mp3");
            ConvertWavToMp3Async(outputFilePath, mp3path);
            File.Delete(outputFilePath);
        }

        public void ConvertWavToMp3Async(string inputWavFilePath, string outputMp3FilePath)
        {
            try
            {
                using (var reader = new WaveFileReader(inputWavFilePath))
                using (var writer = new LameMP3FileWriter(outputMp3FilePath, reader.WaveFormat, 128))
                {
                    reader.CopyTo(writer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"MP3 conversion error: {ex.Message}");
            }
        }

        public string GetSynthesisVoiceNameFromEnum(SynthesisVoices voice)
        {
            switch (voice)
            {
                //[VOICES] https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts
                case SynthesisVoices.RussianFemale:
                    return "ru-RU-SvetlanaNeural";
                case SynthesisVoices.RussianMale:
                    return "ru-RU-DmitryNeural";

                case SynthesisVoices.EnglishFemale:
                    return "en-US-JennyNeural";
                case SynthesisVoices.EnglishMale:
                    return "en-US-GuyNeural";

                default:
                    throw new NotImplementedException();
            }
        }

    }
}
