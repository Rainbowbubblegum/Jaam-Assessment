using Application.Clean;
using Domain.Clean;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Clean
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new InvalidOperationException($"User with ID {id} not found.");
            
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("User name is required.");
            
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("User email is required.");

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email {user.Email} already exists.");

            user.UserName = user.Email; // Set username to email for Identity
            var result = await _userManager.CreateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }
            
            return user;
        }
    }
}