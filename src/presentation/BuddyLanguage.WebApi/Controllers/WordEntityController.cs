using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests.WordEntity;
using BuddyLanguage.HttpModels.Responses.WordEntity;
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
    public async Task<ActionResult<WordEntityResponse>> GetWordEntityById(Guid wordId, CancellationToken cancellationToken)
    {
        WordEntity wordVar = await _wordService.GetWordById(wordId, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
    }

    [HttpPost("update")]
    public async Task<ActionResult<WordEntityResponse>> UpdateWordEntityStatus(UpdateWordEntityRequest request, CancellationToken cancellationToken)
    {
        WordEntity wordVar = await _wordService.UpdateWordEntityById(request.Id, request.Word, request.Language, request.Status, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
    }

    [HttpGet("id-account")]
    public async Task<ActionResult<IReadOnlyList<WordEntity>>> GetAllWordEntitiesForAccountById(
        Guid accountId, CancellationToken cancellationToken)
    {
        IReadOnlyList<WordEntity> wordEntities = await _wordService.GetWordsByAccountId(accountId, cancellationToken);
        return Ok(wordEntities);
    }

    [HttpPost("add")]
    public async Task<ActionResult<WordEntityResponse>> AddWordEntity(
        AddWordEntityRequest request, CancellationToken cancellationToken)
    {
        WordEntity wordVar = await _wordService.AddWord(request.AccountId, request.Word, request.Language, request.Status, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
    }
}
