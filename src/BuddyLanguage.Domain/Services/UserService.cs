﻿using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow, ILogger<UserService> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public virtual async Task<User> TryRegister(string firstName,
            string lastName, string telegramId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("firstName was null or empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("lastName was null or empty");
            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ArgumentException("telegramId was null or empty");

            try
            {
                await _uow.UserRepository.GetUserByTelegramId(telegramId, cancellationToken);
            }
            catch(UserNotFoundException)
            {
                var userVar = new User(Guid.NewGuid(), firstName, lastName, telegramId);

                await _uow.UserRepository.Add(userVar, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                return await _uow.UserRepository.GetById(userVar.Id, cancellationToken);
            }
            return await _uow.UserRepository.GetUserByTelegramId(telegramId, cancellationToken); 
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
            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ArgumentException("telegramId was null or empty");

            var user = await _uow.UserRepository.GetUserByTelegramId(telegramId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException("User with given telegramId not found");
            }

            return user;
        }

        public virtual async Task<User> UpdateUserById(Guid id, string firstName, string lastName, 
            string telegramId, CancellationToken cancellationToken)
        {
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

        public virtual async Task<User> AddUser(string firstName, 
            string lastName, string telegramId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("firstName was null or empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("lastName was null or empty");
            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ArgumentException("telegramId was null or empty");

            var userVar = new User(Guid.NewGuid(), firstName, lastName, telegramId);

            await _uow.UserRepository.Add(userVar, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _uow.UserRepository.GetById(userVar.Id, cancellationToken);
        }
    }
}
