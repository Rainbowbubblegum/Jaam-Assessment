using Application.Clean.DTOs;
using System.Threading.Tasks;

namespace Application.Clean
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtTokenAsync(Domain.Clean.User user);
    }
}