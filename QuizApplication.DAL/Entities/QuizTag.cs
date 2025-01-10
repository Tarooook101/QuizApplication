using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class QuizTag : BaseEntity<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
    }
}
