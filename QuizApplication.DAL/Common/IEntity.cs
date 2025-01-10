using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>  // Add constraint for better type safety
    {
        TKey Id { get; set; }
    }
}
