namespace BuddyLanguage.Domain.Entities;

public class Role : IEntity
{
    private string? _name;
    private string? _prompt;

    public Role(Guid id, string name, string prompt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(prompt));
        }

        Id = id;
        _name = name;
        _prompt = prompt;
    }

    public Guid Id { get; init; }

    public string? Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _name = value;
        }
    }

    public string? Prompt
    {
        get => _prompt;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value is null or whitespace" + nameof(value));
            }

            _prompt = value;
        }
    }
}
