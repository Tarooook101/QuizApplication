using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public record RefreshTokenRequest
    {
        public required string RefreshToken { get; init; }
    }
}
