using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public record UserDto
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? ProfilePictureUrl { get; init; }
        public UserRole Role { get; init; }
        public UserStatus Status { get; init; }
    }
}
