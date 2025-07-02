using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthPractice.Core.DTOs;
using AuthPractice.Data.Repositories.UserRepository;
using AutoMapper;

namespace AuthPractice.Business.Services.UserService
{
    public class UserBusinessService: IUserBusinessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserBusinessService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }
    }
}
