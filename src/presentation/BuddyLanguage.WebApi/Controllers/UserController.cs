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

        [HttpGet("id")]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var userVar = await _userService.GetUserById(userId, cancellationToken);
            return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
        }

        [HttpGet("telegram-id")]
        public async Task<ActionResult<UserResponse>> GetUserByTelegramId(string userId, CancellationToken cancellationToken)
        {
            var userVar = await _userService.GetUserByTelegramId(userId, cancellationToken);
            return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
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

        [HttpPost("add")]
        public async Task<ActionResult<UserResponse>> AddWordEntity(
            AddUserRequest request, CancellationToken cancellationToken)
        {
            var userVar = await _userService.AddUser(request.FirstName, request.LastName, request.TelegramId, cancellationToken);
            return new UserResponse(userVar.Id, userVar.FirstName, userVar.LastName, userVar.TelegramId);
        }
    }
}
