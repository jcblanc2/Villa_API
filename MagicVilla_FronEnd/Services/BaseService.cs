using MagicVilla_FronEnd.Models;
using MagicVilla_FronEnd.Services.IServices;
using MagicVilla_Utility;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace MagicVilla_FronEnd.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }
        public IHttpClientFactory _httpClient;
        public BaseService(IHttpClientFactory httpClient) 
        {
            this.ResponseModel = new();
            _httpClient = httpClient;
        }

        public async Task<T> SentAsync<T>(APIRequest ApiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(ApiRequest.Url);

                if (ApiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(ApiRequest.Data),
                        Encoding.UTF8, "application/json");
                }

                switch (ApiRequest.ApiType)
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

                if (!string.IsNullOrEmpty(ApiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiRequest.Token);
                }

                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    APIResponse APIRESPONSE = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(APIRESPONSE != null && (APIRESPONSE.StatusCode == System.Net.HttpStatusCode.NotFound ||
                        APIRESPONSE.StatusCode == System.Net.HttpStatusCode.BadRequest))
                    {
                        APIRESPONSE.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        APIRESPONSE.IsSuccess = false;

                        var res = JsonConvert.SerializeObject(APIRESPONSE);
                        var rspObj = JsonConvert.DeserializeObject<T>(res);
                        return rspObj;
                    }
                }
                catch (Exception e)
                {
                    var ErrAPIREsponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return ErrAPIREsponse;
                }

                var APIREsponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIREsponse;
            }
            catch(Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorsMessages = new List<string> { Convert.ToString(ex.Message)},
                    IsSuccess = false
                };

                var res = JsonConvert.SerializeObject(dto);
                var APIREsponse = JsonConvert.DeserializeObject<T>(res);
                return APIREsponse;
            }
        }
    }
}
