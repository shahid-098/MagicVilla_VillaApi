using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.IRepository;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
       
        private readonly IMapper _mapper;
        private readonly IVillaRepository _db;
        protected APIResponse _response;
        public VillaAPIController(IMapper mapper, IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _db = villaRepository;
            this._response = new();
        }

        [HttpGet]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villaList = await _db.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
               
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        //[HttpGet("id")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _db.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
               
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest();
                }
                if (await _db.GetAsync(u => u.Name == createDto.Name) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already exists");
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

                await _db.CreateAsync(model);
                _response.Result = model;
                _response.StatusCode = HttpStatusCode.Created;
               
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>>DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _db.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _db.RemoveAsync(villa);

                _response.StatusCode = HttpStatusCode.NoContent;
               
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.Id != id)
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
                await _db.UpdateAsync(model);
                _response.Result = model;
                _response.StatusCode = HttpStatusCode.NoContent;
               
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.GetAsync(u=>u.Id == id, tracked:false);
            
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
            await _db.UpdateAsync(model);
            return NoContent();

        }
    }
}
