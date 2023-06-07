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
        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _dbContext = context;
        }
        #endregion

        #region GetVillas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Get all villas");
            return Ok(_dbContext.Villas.ToList());
        }
        #endregion

        #region GetVilla
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Error, get villa by id: " + id);
                return BadRequest();
            }

            var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
            if (villa  == null)
            {
                _logger.LogError("Error, villa with id: " + id + " not found");
                return NotFound();
            }

            return Ok(villa);
        }
        #endregion

        #region CreateVilla
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }

            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if(_dbContext.Villas.FirstOrDefault(v => v.Name.ToLower().Equals(villaDto.Name.ToLower())) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }

            Villa villa = new Villa()
            {
                Name = villaDto.Name,
                Id = villaDto.Id,
                Details = villaDto.Details,
                Rate = villaDto.Rate,
                Occupancy = villaDto.Occupancy,
                Sqft = villaDto.Sqft,
                Amenity = villaDto.Amenity,
                ImageUrl = villaDto.ImageUrl
            };
            _dbContext.Villas.AddAsync(villa);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);
        }
        #endregion

        #region DeleteVilla
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _dbContext.Villas.Remove(villa);
            _dbContext.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateVilla
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if(villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }

            var villa = _dbContext.Villas.AsNoTracking().FirstOrDefault(v => v.Id  == id);
            if(villa == null)
            {
                return NotFound();
            }

            Villa villaModel = new Villa()
            {
                Name = villaDto.Name,
                Id = villaDto.Id,
                Details = villaDto.Details,
                Rate = villaDto.Rate,
                Occupancy = villaDto.Occupancy,
                Sqft = villaDto.Sqft,
                Amenity = villaDto.Amenity,
                ImageUrl = villaDto.ImageUrl
            };
            _dbContext.Update(villaModel);
            _dbContext.SaveChanges();

            return NoContent();
        }
        #endregion

        #region UpdatePartialVilla
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchVillaDto)
        {
            if (patchVillaDto == null || id <= 0)
            {
                return BadRequest();
            }

            var villa = _dbContext.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            VillaDto villaModelDto = new ()
            {
                Name = villa.Name,
                Id = villa.Id,
                Details = villa.Details,
                Rate = villa.Rate,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Amenity = villa.Amenity,
                ImageUrl = villa.ImageUrl
            };

            patchVillaDto.ApplyTo(villaModelDto, ModelState);
            Villa villaModel = new()
            {
                Name = villaModelDto.Name,
                Id = villaModelDto.Id,
                Details = villaModelDto.Details,
                Rate = villaModelDto.Rate,
                Occupancy = villaModelDto.Occupancy,
                Sqft = villaModelDto.Sqft,
                Amenity = villaModelDto.Amenity,
                ImageUrl = villaModelDto.ImageUrl
            };

            _dbContext.Update(villaModel);
            _dbContext.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
        #endregion
    }
}