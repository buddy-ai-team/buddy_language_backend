using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuddyLanguage.WebApi.Controllers
{
    [Route("statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        }

        [HttpGet("get_words_learned")]
        public async Task<ActionResult<int>> GetCountWordsLearned(Guid id, CancellationToken cancellationToken)
        {
            var countWordsLearned = await _statisticsService.GetCountWordsLearned(id, cancellationToken);

            if (countWordsLearned is null)
            {
                throw new Exception("The number of learned words wasn't found");
            }

            return countWordsLearned.Value;
        }

        [HttpGet("get_words_learning")]
        public async Task<ActionResult<int>> GetCountWordsLearning(Guid id, CancellationToken cancellationToken)
        {
            var countWordsLearning = await _statisticsService.GetCountWordsLearning(id, cancellationToken);

            if (countWordsLearning is null)
            {
                throw new Exception("The number of learned words wasn't found");
            }

            return countWordsLearning.Value;
        }

        [HttpGet("get_count_messages")]
        public async Task<ActionResult<int>> GetTotalCountMessages(string id, Guid topicId, CancellationToken cancellationToken)
        {
            var totalCountMessages = await _statisticsService.GetTotalCountMessages(id, topicId, cancellationToken);

            if (totalCountMessages is null)
            {
                throw new Exception("The total number of messages wasn't found");
            }

            return totalCountMessages.Value;
        }

        [HttpGet("get_days_communication")]
        public async Task<ActionResult<int>> GetCountDaysCommunication(string id, CancellationToken cancellationToken)
        {
            var countDaysCommunication = await _statisticsService.GetCountDaysCommunication(id, cancellationToken);

            if (countDaysCommunication is null)
            {
                throw new Exception("The total number of days of communication wasn't found");
            }

            return countDaysCommunication.Value;
        }
    }
}
