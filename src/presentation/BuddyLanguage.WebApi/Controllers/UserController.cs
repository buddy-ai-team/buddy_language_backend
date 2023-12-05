using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests.User;
using BuddyLanguage.HttpModels.Responses.User;
using Microsoft.AspNetCore.Mvc;

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

        /*
         * /user/get?userId=...
         */
        [HttpGet("get")]
        public async Task<ActionResult<User>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            return await _userService.GetUserById(userId, cancellationToken);
        }

        /*
         * /user/get_by_telegram_id?id=...
         */
        [HttpGet("get_by_telegram_id")]
        public async Task<ActionResult<User>> GetUserByTelegramId(string id, CancellationToken cancellationToken)
        {
            var userVar = await _userService.GetUserByTelegramId(id, cancellationToken);
            return Ok(userVar);
        }

        [HttpPost("update")]
        public async Task<ActionResult<UserResponse>> UpdateUser(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var userVar = await _userService.UpdateUserById(
                request.Id,
                request.FirstName,
                request.LastName,
                request.TelegramId,
                cancellationToken);
            return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
        }

        [HttpPost("update_user_preferences")]
        public async Task<ActionResult<UserResponse>> UpdateUserPreferences(UpdateUserPreferencesRequest request, CancellationToken cancellationToken)
        {
            var userVar = await _userService.UpdateUserPreferencesByUserId(
                request.Id,
                request.NativeLanguage,
                request.TargetLanguage,
                request.SelectedSpeed,
                request.SelectedVoice,
                request.AssistantRoleId,
                cancellationToken);
            return Ok(userVar);
        }

        [HttpPost("add")]
        public async Task<ActionResult<UserResponse>> AddUser(
            AddUserRequest request, CancellationToken cancellationToken)
        {
            var userVar = await _userService.AddUser(request.FirstName, request.LastName, request.TelegramId, cancellationToken);
            return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
        }
    }
}
