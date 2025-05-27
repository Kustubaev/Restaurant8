using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant8.Dtos.Comment;
using Restaurant8.Extensions;
using Restaurant8.Interfaces;
using Restaurant8.Mappers;
using Restaurant8.Models;

namespace Restaurant8.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IDishRepository _dishRepo;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentRepository commentRepo, IDishRepository dishRepo, UserManager<AppUser> userManager)
        {
            _commentRepo = commentRepo;
            _dishRepo = dishRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = await Task.WhenAll(comments.Select(c => c.ToCommentDtoWithUsernameAsync(_userManager)));

            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            var commentDto = await comment.ToCommentDtoWithUsernameAsync(_userManager);

            return Ok(commentDto);
        }

        [HttpPost]
        [Route("{dishId:int}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int dishId, CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dish = await _dishRepo.GetByIdAsync(dishId);
            if(dish == null) return NotFound("Dish not found");

            var userId = User.GetUserId();
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null) return NotFound("User not found");

            var commentModel = createCommentDto.ToCommentFromCreate(dish.Id);
            commentModel.AppUserId = appUser.Id;
            await _commentRepo.CreateAsync(commentModel);

            var commentDto = await commentModel.ToCommentDtoWithUsernameAsync(_userManager);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var commentModel = await _commentRepo.GetByIdAsync(id);
            if (commentModel == null) return NotFound("Comment not found");

            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && commentModel.AppUserId != userId) return Forbid();

            commentModel = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if (commentModel == null) return NotFound("Comment not found");

            var commentDto = await commentModel.ToCommentDtoWithUsernameAsync(_userManager);
            return Ok(commentDto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var commentModel = await _commentRepo.GetByIdAsync(id);
            if (commentModel == null) return NotFound("Comment does not exist");

            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && commentModel.AppUserId != userId)
                return Forbid();


            commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null) return NotFound("Comment does not exist");

            var commentDto = await commentModel.ToCommentDtoWithUsernameAsync(_userManager);
            return Ok(commentDto);
        }
    }
}
