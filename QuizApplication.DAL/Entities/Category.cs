using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Category : BaseEntity<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? IconUrl { get; set; }

        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> Subcategories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();

        public bool IsRootCategory => ParentCategoryId == null;
    }
}
