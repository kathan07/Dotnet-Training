using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;
using Final_POC.Data.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Final_POC.Business.Services.UserService
{
    public class UserBusinessService: IUserBusinessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserBusinessService> _logger;

        public UserBusinessService(IUserRepository userRepository, IMapper mapper, ILogger<UserBusinessService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto?> CreateUser(RegisterUserDto request)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(x => x.Email == request.Email || x.Username == request.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("CreateUser failed: Email or Username already exists for '{Email}' or '{Username}'", request.Email, request.Username);
                    return null;
                }

                var user = _mapper.Map<User>(request);
                user.Password = HashPassword(request.Password);
                var newUser = await _userRepository.CreateUser(user);

                return _mapper.Map<UserDto>(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user: {Email}", request.Email);
                return null;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(x => x.Id == id);
                if (existingUser == null)
                {
                    _logger.LogInformation("DeleteUser: No user found with Id {UserId}. Returning true.", id);
                    return true;
                }

                var result = await _userRepository.DeleteUser(existingUser);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with Id: {UserId}", id);
                return false;
            }
        }


        public async Task<UserDto?> EditUser(UpdateUserDto request)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(x => x.Id == request.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("EditUser failed: User with Id {UserId} not found.", request.Id);
                    return null;
                }

                var userExist = await _userRepository.GetUser(x =>
                    x.Id != request.Id &&
                    (x.Username == request.Username || x.Email == request.Email));

                if (userExist != null)
                {
                    _logger.LogWarning("EditUser failed: Duplicate username or email for another user (Id: {UserId})", request.Id);
                    return null;
                }

                var user = _mapper.Map<User>(request);
                var updatedUser = await _userRepository.UpdateUser(user);
                if (updatedUser == null)
                {
                    _logger.LogError("EditUser failed: Update operation returned null for Id: {UserId}", request.Id);
                    return null;
                }

                return _mapper.Map<UserDto>(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing user with Id: {UserId}", request.Id);
                return null;
            }
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUser(x => x.Id == id);
                if (user == null)
                {
                    _logger.LogInformation("GetUserById: No user found with Id {UserId}", id);
                    return null;
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with Id: {UserId}", id);
                return null;
            }
        }

        public async Task<List<UserDto>> GetUsers(Expression<Func<User, bool>>? predicate = null)
        {
            try
            {
                var users = await _userRepository.GetUsers(predicate);
                if (users == null || !users.Any())
                {
                    _logger.LogInformation("GetUsers: No users found matching the condition");
                    return new List<UserDto>();
                }

                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users with condition");
                return new List<UserDto>();
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
