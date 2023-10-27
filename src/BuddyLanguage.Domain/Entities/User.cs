

namespace BuddyLanguage.Domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; init; }
        private string? _firstName;
        private string? _lastName;
        private string _telegramId;

        //List of WordEntities
        //Reverse Navigation Property
        public List<WordEntity>? WordEntities;

        public User(Guid id, string firstName, string lastName, string telegramId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(telegramId));

            Id = id;
            _firstName = firstName;
            _lastName = lastName;
            _telegramId = telegramId;
            WordEntities = new List<WordEntity>();
        }

        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                _firstName = value;
            }
        }

        public string? LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                _lastName = value;
            }
        }

        public string? TelegramId
        {
            get => _telegramId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                _telegramId = value;
            }
        }
    }
}
