using AutoMapper;
using MagicVilla_API.DI;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            secretKey = configuration.GetValue<string>("APISettings:Secret");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        
        public bool IsUnique(string username)
        {
            var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName.Equals(username));
            return user == null;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {

            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.Equals(loginDto.Username));

            bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(user == null || isValid == false)
            {
                LoginResponseDto loginResponse = new LoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            var role = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.Name, user.UserName.ToString()),
                            new Claim(ClaimTypes.Role, role.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDto>(user),
                //Role = role.FirstOrDefault()
            };

            return loginResponseDto;
        }

        public async Task<UserDto> Register(RegisterationDto registerationDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerationDto.Username,
                Name = registerationDto.Name,
                Email = registerationDto.Username,
                NormalizedEmail = registerationDto.Username.ToUpper(),
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationDto.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }

                    await _userManager.AddToRoleAsync(user, "admin");
                    var userReturnDto = _dbContext.ApplicationUsers.FirstOrDefaultAsync(u =>
                    u.UserName.Equals(registerationDto.Username));

                    return _mapper.Map<UserDto>(userReturnDto);
                }
            }
            catch(Exception ex) 
            {

            }

            return new UserDto();
        }
    }
}
