
using System.ComponentModel.DataAnnotations;
namespace QuizApplication.API.Models.Category
{
    /// <summary>
    /// Request model for creating a new category
    /// </summary>
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        [Url]
        public string? IconUrl { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
