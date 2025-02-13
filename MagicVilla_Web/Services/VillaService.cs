using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.SD;
using System;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaUrl;
        public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaCreateDTO villaCreateDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = villaCreateDTO,
                URL = villaUrl + "/api/VillaAPI",
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                URL = villaUrl + "/api/VillaAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = villaUrl + "/api/VillaAPI",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = villaUrl + "/api/VillaAPI/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO villaUpdateDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = villaUpdateDTO,
                URL = villaUrl + "/api/VillaAPI/" + villaUpdateDTO.Id,
                Token = token
            });
        }
    }
}
