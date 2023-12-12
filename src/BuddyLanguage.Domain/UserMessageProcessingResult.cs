namespace BuddyLanguage.Domain
{
    public class UserMessageProcessingResult
    {
        public UserMessageProcessingResult(
            string recognizedMessage,
            string botAnswerMessage,
            byte[] botAnswerWavMessage,
            byte[]? botPronunciationWordsWavAnswer,
            string[] mistakes,
            Dictionary<string, string> words)
        {
            RecognizedMessage = recognizedMessage
                ?? throw new ArgumentNullException(nameof(recognizedMessage));
            BotAnswerMessage = botAnswerMessage
                ?? throw new ArgumentNullException(nameof(botAnswerMessage));
            BotAnswerWavMessage = botAnswerWavMessage
                ?? throw new ArgumentNullException(nameof(botAnswerWavMessage));
            BotPronunciationWordsWavAnswer = botPronunciationWordsWavAnswer;
            Mistakes = mistakes ?? throw new ArgumentNullException(nameof(mistakes));
            Words = words ?? throw new ArgumentNullException(nameof(words));
        }

        public string RecognizedMessage { get; set; }

        public string BotAnswerMessage { get; set; }

        public byte[] BotAnswerWavMessage { get; set; }

        public byte[]? BotPronunciationWordsWavAnswer { get; set; }

        public string[] Mistakes { get; set; }

        public Dictionary<string, string> Words { get; set; }
    }
}
