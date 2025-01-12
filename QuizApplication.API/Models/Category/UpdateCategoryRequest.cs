using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Category
{
    public class UpdateCategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        [Url]
        public string? IconUrl { get; set; }
    }
}
