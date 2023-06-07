using AutoMapper;
using MagicVilla_API.Logging;
using MagicVilla_API.Model;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        #region VillaAPIController Depandency Injection
        private readonly ILogger<VillaAPIController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _dbContext = context;
            _mapper = mapper;
        }
        #endregion

        #region GetVillas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Get all villas");

            IEnumerable<Villa> villasList = await _dbContext.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDto>>(villasList));
        }
        #endregion

        #region GetVilla
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Error, get villa by id: " + id);
                return BadRequest();
            }

            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa  == null)
            {
                _logger.LogError("Error, villa with id: " + id + " not found");
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }
        #endregion

        #region CreateVilla
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto villaCreateDto)
        {
            if (villaCreateDto == null)
            {
                return BadRequest(villaCreateDto);
            }

            if(await _dbContext.Villas.FirstOrDefaultAsync(v => v.Name.ToLower().Equals(villaCreateDto.Name.ToLower())) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }

            Villa villa = _mapper.Map<Villa>(villaCreateDto);

            await _dbContext.Villas.AddAsync(villa);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
        }
        #endregion

        #region DeleteVilla
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _dbContext.Villas.Remove(villa);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region UpdateVilla
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaUpdateDto)
        {
            if(villaUpdateDto == null || id != villaUpdateDto.Id)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id  == id);
            if(villa == null)
            {
                return NotFound();
            }

            Villa villaModel = _mapper.Map<Villa>(villaUpdateDto);
            
            _dbContext.Update(villaModel);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region UpdatePartialVilla
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchVillaDto)
        {
            if (patchVillaDto == null || id <= 0)
            {
                return BadRequest();
            }

            var villa = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            VillaUpdateDto villaModelDto = _mapper.Map<VillaUpdateDto>(villa);

            patchVillaDto.ApplyTo(villaModelDto, ModelState);

            Villa villaModel = _mapper.Map<Villa>(villaModelDto);

            _dbContext.Update(villaModel);
            await _dbContext.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
        #endregion
    }
}