using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        string? DeletedBy { get; set; }
        DateTimeOffset? DeletedAt { get; set; }
    }
}
