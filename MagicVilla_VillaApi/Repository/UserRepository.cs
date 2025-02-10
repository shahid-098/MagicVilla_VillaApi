using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.IRepository;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        public UserRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            secretKey = config.GetValue<string>("APISettings:Secret");
        }
        public bool isUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() && u.Password == loginRequestDTO.Password);
            if(user == null)
            {
                return new LoginResponseDTO
                {
                    User = null,
                    Token = ""
                };
            }

            ///generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };
            return loginResponseDTO;
        }

        public async Task<LocalUser> Register([FromForm]RegisterationRequestDTO registerationRequestDTO)
        {
            LocalUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Name = registerationRequestDTO.Name,
                Password = registerationRequestDTO.Password,
                Role = registerationRequestDTO.Role,
            };
            await _db.LocalUsers.AddAsync(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;

        }
    }
}
