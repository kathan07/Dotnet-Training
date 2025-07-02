using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EFCore.Data.Models;
using EFCore.Data.Repositories.UserRepository;
using EFCore.Service.DTOModels;

namespace EFCore.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserDTO?> SignUp(UserDTO userDto)
        {
            if (userDto == null)
            {
                return null;
            }

            try
            {
                var existingUser = await _userRepository.GetUser(userDto.Email);
                if (existingUser != null)
                {
                    return null; 
                }

                var user = _mapper.Map<User>(userDto);
                var createdUser = await _userRepository.AddUser(user);

                if (createdUser == null) return null;

                var userDtoResult = _mapper.Map<UserDTO>(createdUser);
                userDtoResult.Password = null;

                return userDtoResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<UserDTO?> SignIn(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                var user = await _userRepository.GetUser(email);
                if (user == null || user.Password != password)
                {
                    return null;
                }

                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Password = null; // Removing password before returning the user

                return userDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
