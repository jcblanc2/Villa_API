﻿using AutoMapper;
using MagicVilla_API.Logging;
using MagicVilla_API.Model;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {
        #region VillaAPIController Depandency Injection
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaAPIController(ILogger<VillaAPIController> logger, IVillaRepository villaRepository, IMapper mapper)
        {
            _logger = logger;
            _villaRepository = villaRepository;
            _mapper = mapper;
            _response = new();
        }
        #endregion

        #region GetVillas
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
            [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Get all villas");

                IEnumerable<Villa> villasList;
                if (occupancy > 0)
                {
                    villasList = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy, pageSize:pageSize, pageNumber:pageNumber);
                }
                else
                {
                    villasList = await _villaRepository.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villasList = villasList.Where(u => u.Name.Contains(search));
                }

                Pagination pagination = new Pagination {PageSize = pageSize, PageNumber = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Results = _mapper.Map<List<VillaDto>>(villasList);
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

        #region GetVilla
        [ResponseCache(Duration = 30)]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogError("Error, get villa by id: " + id);
                    return BadRequest();
                }

                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _logger.LogError("Error, villa with id: " + id + " not found");
                    return NotFound();
                }

                _response.Results = _mapper.Map<VillaDto>(villa);
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

        #region CreateVilla
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto villaCreateDto)
        {
            try
            {
                if (villaCreateDto == null)
                {
                    return BadRequest(villaCreateDto);
                }

                if (await _villaRepository.GetAsync(v => v.Name.ToLower().Equals(villaCreateDto.Name.ToLower())) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa already Exists!");
                    return BadRequest(ModelState);
                }

                Villa villa = _mapper.Map<Villa>(villaCreateDto);
                await _villaRepository.CreateVillaAsync(villa);

                _response.Results = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                _logger.LogInformation("Create villa");
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

        #region DeleteVilla
        [Authorize(Roles = "CUSTOM")]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                await _villaRepository.RemoveAsync(villa);

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

        #region UpdateVilla
        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto villaUpdateDto)
        {
            try
            {
                if (villaUpdateDto == null || id != villaUpdateDto.Id)
                {
                    return BadRequest();
                }

                var villa = await _villaRepository.GetAsync(v => v.Id == id, tracked: false);
                if (villa == null)
                {
                    return NotFound();
                }

                Villa villaModel = _mapper.Map<Villa>(villaUpdateDto);
                await _villaRepository.UpdateAsync(villaModel);


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

        #region UpdatePartialVilla
        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchVillaDto)
        {
            try
            {
                if (patchVillaDto == null || id <= 0)
                {
                    return BadRequest();
                }

                var villa = await _villaRepository.GetAsync(v => v.Id == id, tracked: false);
                if (villa == null)
                {
                    return NotFound();
                }

                VillaUpdateDto villaModelDto = _mapper.Map<VillaUpdateDto>(villa);

                patchVillaDto.ApplyTo(villaModelDto, ModelState);
                Villa villaModel = _mapper.Map<Villa>(villaModelDto);

                await _villaRepository.UpdateAsync(villaModel);

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