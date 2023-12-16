using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain;

public class UserMessageProcessingResult
{
    public UserMessageProcessingResult(
        string recognizedUserMessage,
        string botAnswerMessage,
        byte[] botAnswerWavMessage,
        byte[]? badPronunciationAudio,
        Dictionary<string, double>? badPronunciationWords, 
        string[] grammarMistakes,
        byte[]? grammarMistakesAudio,
        Dictionary<string, string> words)
    {
        RecognizedUserMessage = recognizedUserMessage
                                ?? throw new ArgumentNullException(nameof(recognizedUserMessage));
        BotAnswerMessage = botAnswerMessage
                           ?? throw new ArgumentNullException(nameof(botAnswerMessage));
        BotAnswerAudio = botAnswerWavMessage
                         ?? throw new ArgumentNullException(nameof(botAnswerWavMessage));
        BadPronunciationAudio = badPronunciationAudio;
        BadPronunciationWords = badPronunciationWords;
        GrammarMistakes = grammarMistakes ?? throw new ArgumentNullException(nameof(grammarMistakes));
        GrammarMistakesAudio = grammarMistakesAudio;
        Words = words ?? throw new ArgumentNullException(nameof(words));
    }

    public string RecognizedUserMessage { get; set; }

    public string BotAnswerMessage { get; set; }

    public byte[] BotAnswerAudio { get; set; }

    public Dictionary<string, double>? BadPronunciationWords { get; set; }

    public byte[]? BadPronunciationAudio { get; set; }

    public string[] GrammarMistakes { get; set; }

    public byte[]? GrammarMistakesAudio { get; set; }

    public Dictionary<string, string> Words { get; set; }
}
