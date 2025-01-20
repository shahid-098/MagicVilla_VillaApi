using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            List<Villa> Villas = _db.Villas.ToList();
            return Ok(Villas);
        }

        //[HttpGet("id")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null)
            {
                return BadRequest();
            }
            if(villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if(_db.Villas.FirstOrDefault(u => u.Name == villaDTO.Name) != null)
            {
                ModelState.AddModelError("CustomeError", "Villa already exists");
                return BadRequest(ModelState);
            }

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details
            };

            _db.Villas.Add(model);
            _db.SaveChanges();
            return Ok(villaDTO);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if(villa == null)
            {
                return NotFound();
            } 
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || villaDTO.Id != id)
            {
                return BadRequest(ModelState);
            }

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);

            VillaDTO villaDTO = new()
            {
                Id = villa.Id,
                Name = villa.Name,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Rate = villa.Rate,
                Details = villa.Details
            };

            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDTO,ModelState);

            Villa model = new Villa()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();

        }
    }
}
