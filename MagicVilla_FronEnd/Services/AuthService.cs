using MagicVilla_FronEnd.Models;
using MagicVilla_FronEnd.Models.DTOS;
using MagicVilla_FronEnd.Services.IServices;
using MagicVilla_Utility;
using System.Net.Http;

namespace MagicVilla_FronEnd.Services
{
    public class AuthService: BaseService, IAuthService
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaUrl;
        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            this.villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginDto loginDto)
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginDto,
                Url = villaUrl + "api/v1/UsersAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationDto registerationDto)
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = registerationDto,
                Url = villaUrl + "api/v1/UsersAuth/register"
            });
        }
    }
}
