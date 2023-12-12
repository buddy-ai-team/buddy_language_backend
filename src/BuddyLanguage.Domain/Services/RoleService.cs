using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.Role;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services;

public class RoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IPromptService _promptService;
    private readonly IUnitOfWork _uow;

    public RoleService(
        ILogger<RoleService> logger,
        IPromptService promptService,
        IUnitOfWork uow)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _promptService = promptService
                ?? throw new ArgumentNullException(nameof(promptService));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public virtual async Task<Role> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var role = await _uow.RoleRepository.GetById(id, cancellationToken);

        if (role is null)
        {
            throw new RoleNotFoundException("Role with given id not found");
        }

        return role;
    }

    public virtual async Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken)
    {
        return await _uow.RoleRepository.GetAll(cancellationToken);
    }

    public virtual async Task<Role> ChangePromptByRoleId(Guid id, string newName, string newPrompt, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(newName);
        ArgumentException.ThrowIfNullOrEmpty(newPrompt);

        var role = await _uow.RoleRepository.GetById(id, cancellationToken);

        if (role is null)
        {
            throw new RoleNotFoundException("Role with given id not found");
        }

        role.Name = newName;
        role.Prompt = newPrompt;

        await _uow.RoleRepository.Update(role, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return await _uow.RoleRepository.GetById(role.Id, cancellationToken);
    }

    public virtual async Task<Role> AddRole(
        string name, string prompt, RoleType roleType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NameOfRoleNotDefinedException("Name of role was not defined");
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new PromptOfRoleNotDefinedException("Prompt of role was not defined");
        }

        var role = new Role(Guid.NewGuid(), name, prompt, roleType);

        await _uow.RoleRepository.Add(role, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return await _uow.RoleRepository.GetById(role.Id, cancellationToken);
    }

    public virtual async Task<Role> GetOrCreateDefaultRole(CancellationToken cancellationToken)
    {
        var existedDefaultRole = await _uow.RoleRepository.FindRoleByRoleType(
            RoleType.Default, cancellationToken);
        if (existedDefaultRole == null)
        {
            string name = _promptService.GetNameForDefaultRole();
            string prompt = _promptService.GetPromptForDefaultRole();
            return await AddRole(name, prompt, RoleType.Default, cancellationToken);
        }

        return existedDefaultRole;
    }
}
