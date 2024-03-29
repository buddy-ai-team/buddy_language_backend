﻿using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.Role;
using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _uow;
        private readonly RoleService _roleService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork uow, RoleService roleService, ILogger<UserService> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<User> TryRegister(
            string? firstName,
            string? lastName,
            string telegramId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(telegramId, nameof(telegramId));

            try
            {
                return await _uow.UserRepository.GetUserByTelegramId(telegramId, cancellationToken);
            }
            catch (UserNotFoundException)
            {
                User user = await Register(firstName, lastName, telegramId, cancellationToken);
                return user;
            }
        }

        public async Task<User> Register(
            string? firstName,
            string? lastName,
            string telegramId,
            CancellationToken cancellationToken)
        {
            var user = new User(Guid.NewGuid(), firstName ?? string.Empty, lastName ?? string.Empty, telegramId)
            {
                UserPreferences =
                {
                    //Temporary Default Values
                    NativeLanguage = Language.Russian,
                    TargetLanguage = Language.English,
                    SelectedVoice = Voice.Male,
                    SelectedSpeed = TtsSpeed.Slow,
                    AssistantRole = await _roleService.GetOrCreateDefaultRole(cancellationToken)
                }
            };

            await _uow.UserRepository.Add(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return user;
        }

        public virtual async Task<User> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository.GetById(id, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException("User with given id not found");
            }

            return user;
        }

        public virtual async Task<User> GetUserByTelegramId(string telegramId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(telegramId, nameof(telegramId));

            var user = await _uow.UserRepository.GetUserByTelegramId(telegramId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException("User with given telegramId not found");
            }

            return user;
        }

        public virtual async Task<User> UpdateUserById(
            Guid id,
            string firstName,
            string lastName,
            string telegramId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrEmpty(lastName, nameof(lastName));
            ArgumentException.ThrowIfNullOrEmpty(telegramId, nameof(telegramId));

            var user = await _uow.UserRepository.GetById(id, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException("User with given id not found");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.TelegramId = telegramId;

            await _uow.UserRepository.Update(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _uow.UserRepository.GetById(user.Id, cancellationToken);
        }

        public virtual async Task<User> UpdateUserPreferencesByUserId(
            Guid id,
            Language nativeLanguage,
            Language targetLanguage,
            TtsSpeed selectedSpeed,
            Voice selectedVoice,
            Guid assistantRoleId,
            CancellationToken cancellationToken)
        {
            var user = await _uow.UserRepository.GetById(id, cancellationToken);
            var role = await _uow.RoleRepository.GetById(assistantRoleId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException("User with given id not found");
            }

            if (role is null)
            {
                throw new RoleNotFoundException("User with given id not found");
            }

            user.UserPreferences.NativeLanguage = nativeLanguage;
            user.UserPreferences.TargetLanguage = targetLanguage;
            user.UserPreferences.SelectedSpeed = selectedSpeed;
            user.UserPreferences.SelectedVoice = selectedVoice;
            user.UserPreferences.AssistantRoleId = role.Id;
            user.UserPreferences.AssistantRole = role;

            await _uow.UserRepository.Update(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return user;
        }

        public virtual async Task<User> AddUser(
            string firstName,
            string lastName,
            string telegramId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrEmpty(lastName, nameof(lastName));
            ArgumentException.ThrowIfNullOrEmpty(telegramId, nameof(telegramId));

            var user = new User(Guid.NewGuid(), firstName, lastName, telegramId);

            await _uow.UserRepository.Add(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
