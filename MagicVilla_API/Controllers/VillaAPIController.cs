using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using MagicVilla_API.Model;
using MagicVilla_API.Models.DTOS;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        #region VillaAPIController Depandency Injection
        private readonly ILogger<VillaAPIController> _logger;
        public VillaAPIController(ILogger<VillaAPIController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region GetVillas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Get all villas");
            return Ok(VillaStore.villaList);
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

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
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

            if(VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower().Equals(villaDto.Name.ToLower())) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }

            villaDto.Id = VillaStore.villaList.OrderByDescending(v => villaDto.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);

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

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            VillaStore.villaList.Remove(villa);
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

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id  == id);
            if(villa == null)
            {
                return NotFound();
            }

            villa.Name = villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;

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

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            patchVillaDto.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
        #endregion
    }
}