﻿using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Entities
{
    public class User : IEntity
    {
        private string? _firstName;
        private string? _lastName;
        private string _telegramId;

        public User(Guid id, string firstName, string lastName, string telegramId)
        {
            Id = id;
            _firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            _lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            _telegramId = telegramId ?? throw new ArgumentNullException(nameof(telegramId));
            WordEntities = new List<WordEntity>();
            UserPreferences = new Preferences(); 
        }

        public Guid Id { get; init; }

        //List of WordEntities
        //Reverse Navigation Property
        public List<WordEntity>? WordEntities { get; set; }

        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                }

                _firstName = value;
            }
        }

        public string? LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                }

                _lastName = value;
            }
        }

        public string? TelegramId
        {
            get => _telegramId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value is null or whitespace" + nameof(value));
                }

                _telegramId = value;
            }
        }

        public Preferences UserPreferences { get; set; }

        public class Preferences
        {
            public Language NativeLanguage { get; set; }

            public Language TargetLanguage { get; set; }

            public Voice SelectedVoice { get; set; }

            public TtsSpeed SelectedSpeed { get; set; }

            //External Key To The Role
            public Guid AssistantRoleId { get; set; }

            //For Include
            public Role? AssistantRole { get; set; }
        }
    }
}
