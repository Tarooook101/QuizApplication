using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Tag
{
    public class UpdateTagRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
