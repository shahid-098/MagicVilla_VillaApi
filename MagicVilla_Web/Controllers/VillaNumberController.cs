using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.Dto.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _service;
        private readonly IVillaService _villa;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService service, IMapper mapper, IVillaService villa)
        {
            _service = service;
            _villa = villa;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaNumberDTO> list = new();
            var response = await _service.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new();
            var response = await _villa.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if(ModelState.IsValid)
            {
                var response = await _service.CreateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    if(response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }
            var resp = await _villa.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result)).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            TempData["error"] = "Villa Number not created";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int villaNum)
        {
            VillaNumberUpdateVM villaNumberVM = new ();
            var response = await _service.GetAsync<APIResponse>(villaNum, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess) 
            { 
                VillaNumberDTO dto = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(dto);
            }
            response = await _villa.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(villaNumberVM);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UpdateAsync<APIResponse>(dto.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }
            var resp = await _villa.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.IsSuccess)
            {
                dto.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result)).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            TempData["success"] = "Villa Number not updated!!!";
            return View(dto);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNum(int villaNum)
        {
            var response = await _service.GetAsync<APIResponse>(villaNum, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                await _service.DeleteAsync<APIResponse>(villaNum, HttpContext.Session.GetString(SD.SessionToken));
                TempData["success"] = "Villa Number deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Villa Number not deleted";
            return BadRequest();
        }
    }
}
