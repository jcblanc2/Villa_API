using MagicVilla_FronEnd.Models;

namespace MagicVilla_FronEnd.Services.IServices
{
    public interface IBaseService
    {
        APIResponse ResponseModel { get; set;  }
        Task<T> SentAsync<T> (APIRequest ApiRequest);
    }
}
