using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDto dto);
        Task<T> RegisterAsync<T>(RegisterationRequestDTO dto);

    }
}
