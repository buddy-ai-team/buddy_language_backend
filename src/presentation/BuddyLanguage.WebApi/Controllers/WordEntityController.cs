using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Services;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS8604

namespace BuddyLanguage.WebApi.Controllers;

[Route("wordentity")]
[ApiController]
public class WordEntityController : ControllerBase
{
    private readonly WordEntityService _wordService;

    public WordEntityController(WordEntityService wordService)
    {
        _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
    }

    [HttpGet("id")]
    public async Task<ActionResult<WordEntity>> GetWordEntityById(Guid wordId, CancellationToken cancellationToken)
    {
        var word = await _wordService.GetWordById(wordId, cancellationToken);
        return Ok(word);
    }

    [HttpPost("update")]
    public async Task<ActionResult<WordEntity>> UpdateWordEntityStatus(Guid wordId, WordEntityStatus status, CancellationToken cancellationToken)
    {
        //Should I Put WordEntityStatus into Shared or give HttpModels a dependency to domain? because how are we going to create
        //a WordEntityStatus in the frontend?
        var word = await _wordService.UpdateWordEntityStatusById(wordId, status, cancellationToken);
        return Ok(word);
    }

    [HttpGet("accountid")]
    public async Task<ActionResult<IReadOnlyList<WordEntity>>> GetAllWordEntitiesForAccountById
        (Guid accountId, CancellationToken cancellationToken)
    {
        var wordEntities = await _wordService.GetWordsByAccountId(accountId, cancellationToken);
        return Ok(wordEntities);
    }

    [HttpPost("add")]
    public async Task<ActionResult<WordEntity>> AddWordEntity
        (Guid accountId, string word, WordEntityStatus status, CancellationToken cancellationToken)
    {
        //Same issue as update here
        var wordVar = await _wordService.AddWord(accountId, word, status, cancellationToken);
        return Ok(wordVar);
    }
}
