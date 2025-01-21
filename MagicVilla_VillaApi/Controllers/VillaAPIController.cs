using AutoMapper;
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
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        //[HttpGet("id")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDto)
        {
            if(createDto == null)
            {
                return BadRequest();
            }
            if(await _db.Villas.FirstOrDefaultAsync(u => u.Name == createDto.Name) != null)
            {
                ModelState.AddModelError("CustomeError", "Villa already exists");
                return BadRequest(ModelState);
            }

            ///<summary>
            ///using Auto mapper
            ///</summary>
            Villa model = _mapper.Map<Villa>(createDto);

            ///<summary>
            ///without using Auto mapper
            ///</summary>
            
            //Villa model = new()
            //{
            //    Name = createDto.Name,
            //    ImageUrl = createDto.ImageUrl,
            //    Amenity = createDto.Amenity,
            //    Occupancy = createDto.Occupancy,
            //    Sqft = createDto.Sqft,
            //    Rate = createDto.Rate,
            //    Details = createDto.Details
            //};

            await _db.Villas.AddAsync(model);
            _db.SaveChanges();
            return Ok(model);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
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
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if(updateDTO == null || updateDTO.Id != id)
            {
                return BadRequest(ModelState);
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            //Villa model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Amenity = updateDTO.Amenity,
            //    Occupancy = updateDTO.Occupancy,
            //    Sqft = updateDTO.Sqft,
            //    Rate = updateDTO.Rate,
            //    Details = updateDTO.Details
            //};
             _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            //VillaUpdateDTO villaDTO = new()
            //{
            //    Id = villa.Id,
            //    Name = villa.Name,
            //    ImageUrl = villa.ImageUrl,
            //    Amenity = villa.Amenity,
            //    Occupancy = villa.Occupancy,
            //    Sqft = villa.Sqft,
            //    Rate = villa.Rate,
            //    Details = villa.Details
            //};

            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDTO,ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);

            //Villa model = new Villa()
            //{
            //    Id = villaDTO.Id,
            //    Name = villaDTO.Name,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Amenity = villaDTO.Amenity,
            //    Occupancy = villaDTO.Occupancy,
            //    Sqft = villaDTO.Sqft,
            //    Rate = villaDTO.Rate,
            //    Details = villaDTO.Details
            //};

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            return NoContent();

        }
    }
}
