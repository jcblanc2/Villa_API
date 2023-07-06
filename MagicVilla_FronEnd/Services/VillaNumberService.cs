using MagicVilla_FronEnd.Models;
using MagicVilla_FronEnd.Models.DTOS;
using MagicVilla_FronEnd.Services.IServices;
using MagicVilla_Utility;

namespace MagicVilla_FronEnd.Services
{
    public class VillaNumberService : BaseService, IVillaNumberServices
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaUrl;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            this.villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateVillaAsync<T>(VillaNumberCreateDto dto)
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = villaUrl + "api/VillaNumberAPI",
                Token = SD.token
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "api/VillaNumberAPI",
                Token = SD.token
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SentAsync<T>(new APIRequest() 
            { 
                ApiType = SD.ApiType.GET, 
                Url = villaUrl + "api/VillaNumberAPI/" + id,
                Token = SD.token
            });
        }

        public Task<T> RemoveAsync<T>(int id)
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaUrl + "api/VillaNumberAPI/" + id,
                Token = SD.token
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto)
        {
            return SentAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = villaUrl + "api/VillaNumberAPI/" + dto.VillaNo,
                Token = SD.token
            });
        }
    }
}
