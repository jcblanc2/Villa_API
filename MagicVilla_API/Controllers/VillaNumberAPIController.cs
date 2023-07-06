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

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        #region VillaNumberAPIController Depandency Injection
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository, IMapper mapper)
        {
            _villaNumberRepository = villaNumberRepository;
            _mapper = mapper;
            this._response = new();
        }
        #endregion

        #region GetVillaNumbers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> VillaNumberList = await _villaNumberRepository.GetAllAsync(includeProperties: "Villa");

                _response.Results = _mapper.Map<List<VillaNumberDto>>(VillaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
        #endregion

        #region GetVillaNumber
        [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo <= 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo, includeProperties: "Villa");
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Results = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
        #endregion

        #region CreateVillaNumber
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto villaNumberCreateDto)
        {
            try
            {
                if (villaNumberCreateDto == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                if (await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNumberCreateDto.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "VillaNumber already Exists!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorsMessages.Add("VillaNumber already Exists!");
                    return BadRequest(_response);
                }

                if (await _villaNumberRepository.GetAsync(v => v.VillaID == villaNumberCreateDto.VillaID) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa ID is invalid!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorsMessages.Add("VillaNumber already Exists!");
                    return NotFound(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDto);
                await _villaNumberRepository.CreateVillaAsync(villaNumber);

                _response.Results = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

        #region DeleteVillaNumer
        [Authorize(Roles = "CUSTOM")]
        [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumer(int villaNo)
        {
            try
            {
                if (villaNo <= 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                await _villaNumberRepository.RemoveAsync(villaNumber);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

        #region UpdateVillaNumber
        [Authorize(Roles = "admin")]
        [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDto villaNumberUpdateDto)
        {
            try
            {
                if (villaNumberUpdateDto == null || villaNo != villaNumberUpdateDto.VillaNo)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo, tracked: false);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorsMessages = new List<string>() { "Villa ID is invalid!" };
                    return NotFound(_response);
                }
      
                VillaNumber villaNumberModel = _mapper.Map<VillaNumber>(villaNumberUpdateDto);
                await _villaNumberRepository.UpdateAsync(villaNumberModel);


                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

        #region UpdatePartialVillaNumber
        [Authorize(Roles = "admin")]
        [HttpPatch("{villaNo:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, JsonPatchDocument<VillaNumberUpdateDto> patchVillaNumberDto)
        {
            try
            {
                if (patchVillaNumberDto == null || villaNo <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo, tracked: false);
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorsMessages = new List<string>() { "Villa ID is invalid!" };
                    return NotFound(_response);
                }

                VillaNumberUpdateDto villaNumberUpdateDto = _mapper.Map<VillaNumberUpdateDto>(villaNumber);
                patchVillaNumberDto.ApplyTo(villaNumberUpdateDto, ModelState);

                VillaNumber villaNumberModel = _mapper.Map<VillaNumber>(villaNumberUpdateDto);
                await _villaNumberRepository.UpdateAsync(villaNumberModel);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion
    }
}