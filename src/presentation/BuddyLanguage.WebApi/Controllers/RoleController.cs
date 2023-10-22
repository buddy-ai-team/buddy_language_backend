using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.HttpModels.Requests;
using BuddyLanguage.HttpModels.Responses;
using Microsoft.AspNetCore.Mvc;

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
    
    [HttpGet("current")]
    public async Task<ActionResult<RoleResponse>> GetRoleByName(RoleRequest roleRequest, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleByName(roleRequest.Name, cancellationToken);
        var resp = new RoleResponse(role.Id, role.Name, role.Prompt);
        return resp;
    }

    [HttpPost("update")]
    public async Task<ActionResult<UpdateRoleResponse>> UpdateRole(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.ChangePromptByRoleName(request.Name, request.Prompt, cancellationToken);
        
        return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<Role[]>> GetAllAccounts(CancellationToken cancellationToken)
    {
        var products =  await _roleService.GetAll(cancellationToken);
        return Ok(products);
    }

    [HttpPost("add")]
    public async Task<ActionResult<UpdateRoleResponse>> AddRole(AddRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleService.AddRole(new Role(Guid.NewGuid(), request.Name, request.Prompt), cancellationToken);
        
        return new UpdateRoleResponse(role.Id, role.Name, role.Prompt);
    }
}