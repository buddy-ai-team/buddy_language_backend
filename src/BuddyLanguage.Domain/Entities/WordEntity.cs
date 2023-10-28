﻿using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Entities;

public class WordEntity : IEntity
{
    private string? _word;

    private WordEntityStatus _wordStatus;

    public WordEntity(Guid id, Guid accountId, string word, WordEntityStatus wordStatus)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        Id = id;
        AccountId = accountId;
        _word = word;
        _wordStatus = wordStatus;
    }

    public Guid Id { get; init; }

    //External Key To An Account Once They Are Added To Our Project
    public Guid AccountId { get; init; }

    public string? Word
    {
        get => _word;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _word = value;
        }
    }

    public WordEntityStatus WordStatus
    {
        get => _wordStatus;
        set
        {
            _wordStatus = value;
        }
    }
}
