using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Exceptions.Role;
using Microsoft.Extensions.Logging;
using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.Domain.Services;

public class RoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IUnitOfWork _uow;

    public RoleService(ILogger<RoleService> logger, IUnitOfWork uow)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public virtual async Task<Role> GetRoleByName(string name, CancellationToken cancellationToken)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        var role = await _uow.RoleRepository.GetByName(name, cancellationToken);
        
        if (role is null)
        {
            throw new RoleNotFoundException("Role with given name not found");
        }
        
        return role;
    }

    public virtual async Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken)
    {
        return await _uow.RoleRepository.GetAll(cancellationToken);
    }

    public virtual async Task<Role> ChangePromptByRoleName(string name, string newPrompt, CancellationToken cancellationToken)
    {
        var role = await _uow.RoleRepository.GetByName(name, cancellationToken);
        
        if (role is null)
        {
            throw new RoleNotFoundException("Role with given name not found");
        }
        
        role.Prompt = newPrompt;
        
        await _uow.RoleRepository.Update(role, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return await _uow.RoleRepository.GetById(role.Id, cancellationToken);
    }

    public virtual async Task<Role> AddRole(Role role, CancellationToken cancellationToken)
    {
        var existingRole = await _uow.RoleRepository.GetByName(role.Name!, cancellationToken);
        
        if (existingRole is not null)
        {
            throw new RoleAlreadyExistsException("Role with given name already exists");
        }
        await _uow.RoleRepository.Add(role, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return await _uow.RoleRepository.GetById(role.Id, cancellationToken);
    }
}