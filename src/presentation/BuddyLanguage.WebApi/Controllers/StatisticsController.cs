using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.ExternalStatisticsServiceLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuddyLanguage.WebApi.Controllers
{
    [Route("statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ExternalStatisticsService _externalStatisticsService;

        public StatisticsController(ExternalStatisticsService externalStatisticsService)
        {
            _externalStatisticsService = externalStatisticsService ?? throw new ArgumentNullException(nameof(externalStatisticsService));
        }

        [HttpGet("get_statistics")]
        public async Task<ActionResult<StatisticsResponse>> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken)
        {
            var statistics = await _externalStatisticsService.GetCountOfDaysAndMessages(id, cancellationToken);

            return Ok(statistics);
        }
    }
}
