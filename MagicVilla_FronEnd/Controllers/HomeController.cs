using AutoMapper;
using MagicVilla_FronEnd.Models;
using MagicVilla_FronEnd.Models.DTOS;
using MagicVilla_FronEnd.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVilla_FronEnd.Controllers
{
    public class HomeController : Controller
    {

        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public HomeController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            //List<VillaDto> list = new List<VillaDto>();

            //var response = await _villaService.GetAllAsync<APIResponse>();
            //if (response != null && response.IsSuccess)
            //{
            //    list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Results));
            //}

            return View();
        }
    }
}