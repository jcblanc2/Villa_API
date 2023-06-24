using AutoMapper;
using MagicVilla_FronEnd.Models;
using MagicVilla_FronEnd.Models.DTOS;
using MagicVilla_FronEnd.Models.ViewModel;
using MagicVilla_FronEnd.Services;
using MagicVilla_FronEnd.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_FronEnd.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberServices _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberServices villaNumberService, IVillaService villaService, IMapper mapper)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;  
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDto> list = new List<VillaNumberDto>();

            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDto>>(Convert.ToString(response.Results));
            }

            return View(list);
        }


        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateView createView = new VillaNumberCreateView();

            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess )
            {
                createView.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
                    (Convert.ToString(response.Results)).Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }

            return View(createView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateView createView)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateVillaAsync<APIResponse>(createView.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (response.ErrorsMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorsMessages.FirstOrDefault());
                    }
                }
            }


            var resp = await _villaService.GetAllAsync<APIResponse>();
            if (resp != null && resp.IsSuccess )
            {
                createView.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
                    (Convert.ToString(resp.Results)).Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(createView);
        }


        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            VillaNumberUpdateView updateView = new VillaNumberUpdateView();

            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Results));
                updateView.VillaNumber = _mapper.Map<VillaNumberUpdateDto>(model);
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                updateView.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
                    (Convert.ToString(response.Results)).Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(updateView);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateView updateView)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(updateView.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (response.ErrorsMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorsMessages.FirstOrDefault());
                    }
                }
            }


            var resp = await _villaService.GetAllAsync<APIResponse>();
            if (resp != null && resp.IsSuccess)
            {
                updateView.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
                    (Convert.ToString(resp.Results)).Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(updateView);
        }




        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            VillaNumberDeleteView deleteView = new VillaNumberDeleteView();

            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Results));
                deleteView.VillaNumber = model;
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                deleteView.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
                    (Convert.ToString(response.Results)).Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(deleteView);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteView model)
        {
            var response = await _villaNumberService.RemoveAsync<APIResponse>(model.VillaNumber.VillaID);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            return View(model);
        }
    }
}
