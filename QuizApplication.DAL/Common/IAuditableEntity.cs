using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
        DateTimeOffset CreatedAt { get; set; }  // Changed to DateTimeOffset for better timezone handling
        string? LastModifiedBy { get; set; }    // Made nullable since it won't always have a value
        DateTimeOffset? LastModifiedAt { get; set; }
    }
}
