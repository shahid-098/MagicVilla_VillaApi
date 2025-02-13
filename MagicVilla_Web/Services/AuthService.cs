using MagicVilla_Utility;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _client;
        private string villaUrl;

        public AuthService(IHttpClientFactory client, IConfiguration config) : base(client)
        {
            _client = client;
            villaUrl = config.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDto dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                URL = villaUrl + "/api/UserAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                URL = villaUrl + "/api/UserAuth/register"
            });
        }
    }
}
