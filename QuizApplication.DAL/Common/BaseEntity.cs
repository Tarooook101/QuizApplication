using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletable
    where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public string CreatedBy { get; set; } = null!;  // Non-nullable reference type
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
