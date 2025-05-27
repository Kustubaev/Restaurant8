using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant8.Dtos.Dish;
using Restaurant8.Dtos.Query;
using Restaurant8.Interfaces;
using Restaurant8.Mappers;
using Restaurant8.Models;

namespace Restaurant8.Controllers
{
    [Route("api/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishRepository _dishRepo;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDishService _dishService;

        public DishController(IDishRepository dishRepo, IWebHostEnvironment env, UserManager<AppUser> userManager, IDishService dishService)
        {
            _dishRepo = dishRepo;
            _env = env;
            _userManager = userManager;
            _dishService = dishService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DishQueryDto query)
        {
            var result = await _dishService.GetAllFilteredAsync(query);

            return Ok(new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                CurrentPage = result.Page,
                PageSize = result.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var dishDto = await _dishService.GetDetailsByIdAsync(id);
            if (dishDto == null) return NotFound();
            return Ok(dishDto);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromForm] CreateDishRequestDto createDishDto)
        {
            var dishModel = await _dishRepo.CreateAsync(createDishDto);
            var dishDto = await _dishService.GetDetailsByIdAsync(dishModel.Id);
            return CreatedAtAction(nameof(GetById), new {id = dishModel.Id}, dishDto);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateDishRequestDto updateDishDto)
        {
            var dishModel = await _dishRepo.UpdateAsync(id, updateDishDto);
            if(dishModel == null) return NotFound();
            var dishDto = await _dishService.GetDetailsByIdAsync(dishModel.Id);
            return Ok(dishDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dishModel = await _dishRepo.DeleteAsync(id);
            if (dishModel == null) return NotFound();

            return NoContent();
        }
    }
}
