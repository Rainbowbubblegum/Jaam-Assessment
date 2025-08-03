using Application.Clean;
using Application.Clean.DTOs;
using Domain.Clean;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; // Added for List<Claim>

namespace Infrastructure.Clean
{
    /// <summary>
    /// Authentication service implementation
    /// 
    /// This handles user registration, login, and JWT token generation.
    /// I'm using ASP.NET Core Identity for user management (handles password hashing, 
    /// validation, etc.) and JWT for stateless authentication (no server-side sessions).
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new user
        /// 
        /// I'm using ASP.NET Core Identity which handles all the security stuff:
        /// - Password hashing (bcrypt)
        /// - Password validation (complexity rules)
        /// - Email uniqueness checks
        /// - Account lockout protection
        /// 
        /// The user gets a default role of "User" unless specified otherwise.
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                UserName = registerDto.Email, // Use email as username
                Email = registerDto.Email,
                Name = registerDto.Name,
                Role = registerDto.Role ?? "User" // Default to User role if not specified
            };

            // Create the user with password
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Add user to role
            var roleResult = await _userManager.AddToRoleAsync(user, user.Role);
            if (!roleResult.Succeeded)
            {
                // If role assignment fails, delete the user to maintain consistency
                await _userManager.DeleteAsync(user);
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role assignment failed: {errors}");
            }

            // Generate JWT token for the new user
            var token = await GenerateJwtTokenAsync(user);
            
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(60) // Default 60 minutes
            };
        }

        /// <summary>
        /// Login user and generate JWT token
        /// 
        /// I'm using SignInManager for password verification (it handles the hashing comparison)
        /// and JWT for stateless authentication (no server-side sessions to manage).
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            // Verify password using SignInManager
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            // Generate JWT token
            var token = await GenerateJwtTokenAsync(user);
            
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(60) // Default 60 minutes
            };
        }

        /// <summary>
        /// Generate JWT token for a user
        /// 
        /// The token contains user claims (ID, email, role) and is signed with a secret key.
        /// Token expiration is configured in appsettings.json so we can change it without
        /// recompiling the app.
        /// </summary>
        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            // Get the user's roles for the token claims
            var roles = await _userManager.GetRolesAsync(user);
            
            // Build the claims that will go into the JWT token
            // These claims tell us who the user is and what they can do
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim(ClaimTypes.Email, user.Email),                  // Email
                new Claim(ClaimTypes.Name, user.Name),                    // Display name
                new Claim("Role", user.Role)                              // Custom role claim
            };

            // Add the user's roles as claims (for role-based authorization)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Get JWT configuration
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
                           jwtSettings["SecretKey"] ?? 
                           throw new InvalidOperationException("JWT SecretKey not configured");

            var key = Encoding.ASCII.GetBytes(secretKey);
            var issuer = jwtSettings["Issuer"] ?? "TaskManagementAPI";
            var audience = jwtSettings["Audience"] ?? "TaskManagementUsers";
            var expirationHours = jwtSettings.GetValue<int>("ExpirationHours", 24);
            var expirationMinutes = expirationHours * 60;

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Get user by ID
        /// Useful for getting current user information
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        /// <summary>
        /// Get user by email
        /// Useful for user lookup operations
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Validate JWT token and get user
        /// This is used by the authentication middleware to validate tokens
        /// </summary>
        public async Task<User?> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSettings = _configuration.GetSection("Jwt");
                var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
                               jwtSettings["SecretKey"] ?? 
                               throw new InvalidOperationException("JWT SecretKey not configured");

                var key = Encoding.ASCII.GetBytes(secretKey);
                var issuer = jwtSettings["Issuer"] ?? "TaskManagementAPI";
                var audience = jwtSettings["Audience"] ?? "TaskManagementUsers";

                // Validate token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                return await GetUserByIdAsync(userId);
            }
            catch
            {
                return null; // Token is invalid
            }
        }
    }
}