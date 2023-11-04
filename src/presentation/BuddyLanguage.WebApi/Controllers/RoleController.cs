using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests.Role;
using BuddyLanguage.HttpModels.Responses.Role;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Serilog;

#pragma warning disable CS8604

namespace BuddyLanguage.WebApi.Controllers;

[Route("role")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public RoleController(RoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [HttpGet("id")]
    public async Task<ActionResult<RoleResponse>> GetRoleById(RoleByIdRequest roleByIdRequest, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleById(roleByIdRequest.Id, cancellationToken);
        var resp = new RoleResponse(role.Id, role.Name, role.Prompt);
        return resp;
    }

    [HttpPost("update")]
    public async Task<ActionResult<UpdateRoleResponse>> UpdateRole(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.ChangePromptByRoleId(request.Id, request.Name, request.Prompt, cancellationToken);
        return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
    }

    [HttpGet("all")]
    public async Task<ActionResult<Role[]>> GetAllRoles(CancellationToken cancellationToken)
    {
        var products = await _roleService.GetAll(cancellationToken);
        return Ok(products);
    }

    [HttpPost("add")]
    public async Task<ActionResult<UpdateRoleResponse>> AddRole(AddRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.AddRole(request.Name, request.Prompt, cancellationToken);
        return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
    }
}
