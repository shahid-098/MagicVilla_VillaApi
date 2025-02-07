using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.Dto.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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
            var response = await _service.GetAllAsync<APIResponse>();
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new();
            var response = await _villa.GetAllAsync<APIResponse>();
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
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if(ModelState.IsValid)
            {
                var response = await _service.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> UpdateVillaNum(int villaNum)
        {
            var response = await _service.GetAsync<APIResponse>(villaNum);
            if(response != null && response.IsSuccess) 
            { 
                VillaNumberDTO dto = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaNumberUpdateDTO>(dto));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVillaNum(VillaNumberUpdateDTO dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UpdateAsync<APIResponse>(dto);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(dto);
        }

        public async Task<IActionResult> DeleteVillaNum(int villaNum)
        {
            var response = await _service.GetAsync<APIResponse>(villaNum);
            if (response != null && response.IsSuccess)
            {
                await _service.DeleteAsync<APIResponse>(villaNum);
            }
            return RedirectToAction("Index");
        }
    }
}
