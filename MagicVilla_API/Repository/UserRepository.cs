using MagicVilla_API.DI;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private string secretKey;
        public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            secretKey = configuration.GetValue<string>("APISettings:Secret");
        }
        
        public bool IsUnique(string username)
        {
            var user = _dbContext.LocalUsers.FirstOrDefault(u => u.Username.Equals(username));
            return user == null;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {

            var user = await _dbContext.LocalUsers.FirstOrDefaultAsync(u => u.Username.Equals(loginDto.Username));

            if(user != null)
            {
                string hashedPassword = PasswordHelper.HashPassword(loginDto.Password, user.Salt);

                if (user.Password.Equals(hashedPassword))
                {
                    // Generate JWT Token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(secretKey);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role)
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    user.Password = "";
                    user.Salt = "";

                    LoginResponseDto loginResponseDto = new LoginResponseDto()
                    {
                        Token = tokenHandler.WriteToken(token),
                        User = user
                    };

                    return loginResponseDto;
                }
            }

            LoginResponseDto loginResponse = new LoginResponseDto()
            {
                Token = "",
                User = null
            };

            return loginResponse;
        }

        public async Task<LocalUser> Register(RegisterationDto registerationDto)
        {
            string salt = PasswordHelper.GenerateSalt();
            string hashedPassword = PasswordHelper.HashPassword(registerationDto.Password, salt);

            LocalUser localUser = new LocalUser()
            {
                Username = registerationDto.Username,
                Name = registerationDto.Name,
                Password = hashedPassword,
                Salt = salt,
                Role = registerationDto.Role
            };

            _dbContext.LocalUsers.Add(localUser);
            await _dbContext.SaveChangesAsync();

            localUser.Password = "";
            localUser.Salt = "";

            return localUser;
        }
    }
}
