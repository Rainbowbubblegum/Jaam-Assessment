using Domain.Clean;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Clean
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
    }
}