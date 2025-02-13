using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _service;
        private readonly IMapper _mapper;

        public VillaController(IVillaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<VillaDTO> list = new();
            var response = await _service.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Error Encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var response = await _service.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDTO>(villaDTO));
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Not Updated!!!!!!";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response = await _service.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa deleted successfully";
                await _service.DeleteAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            }
            TempData["error"] = "Villa not deleted!!!";
            return RedirectToAction("Index");
        }
    }
}
