using AutoMapper;
using MagicVilla_VillaApi.IRepository;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberController : ControllerBase
    {
        private readonly IVillaNumberRepository _db;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillaNumberController(IVillaNumberRepository db, IMapper mapper, IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _db = db;
            this._response = new APIResponse();
            _villaRepository=villaRepository;
        }

        //public async Task<ActionResult<IEnumerable<VillaNumberDTO>>> GetVillaNumbers()
        //{
        //    return Ok(await _db.GetAllAsync());
        //}

        /// <summary>
        /// APIResponse is to used to return data to user as shown in model
        /// mapper is used to map VillaNumberDTO to VillaNumber
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbersList = await _db.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(_response);
                }
                var villaNumber = await _db.GetAsync(u => u.VillaNum == id);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return Ok(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if (villaNumberCreateDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                }
                if (await _db.GetAsync(u => u.VillaNum == villaNumberCreateDTO.VillaNum)!=null)
                {
                    ModelState.AddModelError("Villa Number Error", "Villa Number already exists");
                    return BadRequest(ModelState);
                }
                if(await _db.GetAsync(u => u.VillaId == villaNumberCreateDTO.VillaId)==null)
                {
                    ModelState.AddModelError("Villa Error", "Villa doesn't exists");
                    return BadRequest(ModelState);
                }
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);
                await _db.CreateAsync(villaNumber);
                _response.Result = villaNumber;
                _response.StatusCode=HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villaNumberUpdateDto)
        {
            try
            {
                if (id != villaNumberUpdateDto.VillaNum || villaNumberUpdateDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(_response);
                }
                if (await _db.GetAsync(u => u.VillaId == villaNumberUpdateDto.VillaId)==null)
                {
                    ModelState.AddModelError("Villa Error", "Villa doesn't exists");
                    return BadRequest(ModelState);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(villaNumberUpdateDto);
                await _db.UpdateAsync(model);
                _response.Result = model;
                _response.StatusCode=HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(_response);
                }
                var villaNumber = await _db.GetAsync(u => u.VillaNum == id);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                }
                await _db.RemoveAsync(villaNumber);
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

    }
}
