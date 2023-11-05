﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace BuddyLanguage.Domain.GptDataModels.Answers;

public class MistakesAnswer
{
    public int MistakesCount { get; set; }

    public string[] Mistakes { get; set; }

    public override string ToString() => string.Join(", ", Mistakes);
}
