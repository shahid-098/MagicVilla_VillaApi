using AutoMapper.Internal;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient {  get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
            this.responseModel = new();
        }

        public async Task<T> SendAsync<T>(APIRequest apIRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apIRequest.URL);
                //apIRequest.Data wiil not be null in POST/PUT HTTP requests
                if (apIRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apIRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                switch (apIRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                HttpResponseMessage apiResponse = null;

                if (!string.IsNullOrEmpty(apIRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apIRequest.Token);
                }


                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(apiResponse.StatusCode == HttpStatusCode.BadRequest || apiResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        ApiResponse.StatusCode = HttpStatusCode.BadRequest;
                        ApiResponse.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(ApiResponse);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch(Exception ex)
                {
                    var APIResponseExpection = JsonConvert.DeserializeObject<T>(apiContent);
                    return APIResponseExpection;
                }
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponse;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false,
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
