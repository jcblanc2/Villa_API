using AutoMapper;
using MagicVilla_API.Logging;
using MagicVilla_API.Model;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using MagicVilla_API.Repository;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIV2Controller : ControllerBase
    {
        #region VillaNumberAPIV2Controller Depandency Injection
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaNumberAPIV2Controller(IVillaNumberRepository villaNumberRepository, IMapper mapper)
        {
            _villaNumberRepository = villaNumberRepository;
            _mapper = mapper;
            _response = new();
        }
        #endregion

        //[MapToApiVersion("2.0")]
        [HttpGet("GetStrings")]
        public IEnumerable<string> Get()
        {
            return new string[] { "John", "Clayton" };
        }
    }
}