using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests.User;
using BuddyLanguage.HttpModels.Responses.User;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Serilog;

#pragma warning disable CS8604

namespace BuddyLanguage.WebApi.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("id")]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var userVar = await _userService.GetUserById(userId, cancellationToken);
                return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while getting the user by ID");
                SentrySdk.CaptureException(ex);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("telegram-id")]
        public async Task<ActionResult<UserResponse>> GetUserByTelegramId(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var userVar = await _userService.GetUserByTelegramId(userId, cancellationToken);
                return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred when receiving a user by Telegram ID");
                SentrySdk.CaptureException(ex);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<UserResponse>> UpdateUser(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userVar = await _userService.UpdateUserById(
                request.Id,
                request.FirstName,
                request.LastName,
                request.TelegramId,
                cancellationToken);
                return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while updating the user");
                SentrySdk.CaptureException(ex);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<UserResponse>> AddWordEntity(
            AddUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userVar = await _userService.AddUser(request.FirstName, request.LastName, request.TelegramId, cancellationToken);
                return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while adding a user");
                SentrySdk.CaptureException(ex);
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
