using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests.WordEntity;
using BuddyLanguage.HttpModels.Responses.WordEntity;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Serilog;

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
        try
        {
            WordEntity wordVar = await _wordService.GetWordById(wordId, cancellationToken);
            return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while fetching a word entity by ID");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpPost("update")]
    public async Task<ActionResult<WordEntityResponse>> UpdateWordEntityStatus(UpdateWordEntityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            WordEntity wordVar = await _wordService.UpdateWordEntityById(request.Id, request.Word, request.Language, request.Status, cancellationToken);
            return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while updating a word entity");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpGet("id-account")]
    public async Task<ActionResult<IReadOnlyList<WordEntity>>> GetAllWordEntitiesForAccountById(
        Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<WordEntity> wordEntities = await _wordService.GetWordsByAccountId(accountId, cancellationToken);
            return Ok(wordEntities);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while fetching word entities by account ID");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<WordEntityResponse>> AddWordEntity(
        AddWordEntityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            WordEntity wordVar = await _wordService.AddWord(request.AccountId, request.Word, request.Language, request.Status, cancellationToken);
            return new WordEntityResponse(wordVar.Id, wordVar.UserId, wordVar.Word, wordVar.Language, wordVar.WordStatus);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while adding a word entity");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "An error occurred");
        }
    }
}
