using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;

namespace MagicVilla_VillaApi.IRepository
{
    public interface IUserRepository
    {
        bool isUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
