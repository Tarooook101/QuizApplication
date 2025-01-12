using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Configurations
{
    public class AccessControlConverter : ValueConverter<AccessControl, string>
    {
        public AccessControlConverter()
            : base(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
                v => JsonSerializer.Deserialize<AccessControl>(v, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }) ?? new AccessControl())
        {
        }
    }
}
