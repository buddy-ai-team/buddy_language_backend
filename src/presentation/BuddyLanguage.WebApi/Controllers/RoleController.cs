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
        try
        {
            var role = await _roleService.GetRoleById(roleByIdRequest.Id, cancellationToken);
            var resp = new RoleResponse(role.Id, role.Name, role.Prompt);
            return resp;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred in GetRoleById");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("update")]
    public async Task<ActionResult<UpdateRoleResponse>> UpdateRole(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.ChangePromptByRoleId(request.Id, request.Name, request.Prompt, cancellationToken);
            return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred when changing the role");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<Role[]>> GetAllRoles(CancellationToken cancellationToken)
    {
        try
        {
            var products = await _roleService.GetAll(cancellationToken);
            return Ok(products);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while getting all the roles");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<UpdateRoleResponse>> AddRole(AddRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.AddRole(request.Name, request.Prompt, cancellationToken);
            return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while adding a role");
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Internal Server Error");
        }
    }
}
