using Microsoft.AspNetCore.Identity;
using Restaurant8.Dtos.Comment;
using Restaurant8.Models;

namespace Restaurant8.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Content = commentModel.Content,
                CreatedDate = commentModel.CreatedDate,
                UpdatedDate = commentModel.UpdatedDate,
                DishId = commentModel.DishId,
                AppUserId = commentModel.AppUserId,
            };
        }

        public static async Task<CommentDto> ToCommentDtoWithUsernameAsync(this Comment comment, UserManager<AppUser> userManager)
        {
            var user = await userManager.FindByIdAsync(comment.AppUserId);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                UpdatedDate = comment.UpdatedDate,
                DishId = comment.DishId,
                AppUserId = comment.AppUserId,
                Username = comment.AppUser?.UserName ?? "Unknown"
            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int dishId)
        {
            return new Comment
            {
                Content = commentDto.Content,
                DishId = dishId,
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentDto commentDto)
        {
            return new Comment
            {
                Content = commentDto.Content,
            };
        }
    }
}
