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
        var wordVar = await _wordService.GetWordById(wordId, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.AccountId, wordVar.Word, wordVar.WordStatus);
    }

    [HttpPost("update")]
    public async Task<ActionResult<WordEntityResponse>> UpdateWordEntityStatus(UpdateWordEntityStatusRequest request, CancellationToken cancellationToken)
    {
        var wordVar = await _wordService.UpdateWordEntityStatusById(request.Id, request.Status, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.AccountId, wordVar.Word, wordVar.WordStatus);
    }

    [HttpGet("id-account")]
    public async Task<ActionResult<IReadOnlyList<WordEntity>>> GetAllWordEntitiesForAccountById
        (Guid accountId, CancellationToken cancellationToken)
    {
        var wordEntities = await _wordService.GetWordsByAccountId(accountId, cancellationToken);
        return Ok(wordEntities);
    }

    [HttpPost("add")]
    public async Task<ActionResult<WordEntityResponse>> AddWordEntity
        (AddWordEntityRequest request, CancellationToken cancellationToken)
    {
        var wordVar = await _wordService.AddWord(request.AccountId, request.Word, request.Status, cancellationToken);
        return new WordEntityResponse(wordVar.Id, wordVar.AccountId, wordVar.Word, wordVar.WordStatus);
    }
}
