using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.ExternalStatisticsServiceLib;
using BuddyLanguage.HttpModels.Responses.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuddyLanguage.WebApi.Controllers
{
    [Route("statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticServic;

        public StatisticsController(IStatisticsService statisticServic)
        {
            _statisticServic = statisticServic ?? throw new ArgumentNullException(nameof(statisticServic));
        }

        [HttpGet("get_statistics")]
        public async Task<ActionResult<StatisticsResponse>> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken)
        {
            var st = await _statisticServic.GetCountOfDaysAndMessages(id, cancellationToken);

            return new StatisticsResponse(st.TotalMessages, st.NumbersDaysCommunication);
        }
    }
}
