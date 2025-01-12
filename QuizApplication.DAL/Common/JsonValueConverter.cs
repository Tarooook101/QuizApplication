using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public static class JsonValueConverter
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static ValueConverter<Dictionary<string, string>, string> DictionaryStringConverter =>
            new(
                v => JsonSerializer.Serialize(v, Options),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, Options) ?? new Dictionary<string, string>()
            );

        public static ValueConverter<T, string> Create<T>() where T : class, new() =>
            new(
                v => JsonSerializer.Serialize(v, Options),
                v => JsonSerializer.Deserialize<T>(v, Options) ?? new T()
            );

        public static ValueComparer<Dictionary<string, string>> DictionaryComparer =>
            new(
                (c1, c2) => c1 != null && c2 != null && JsonSerializer.Serialize(c1, Options) == JsonSerializer.Serialize(c2, Options),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(c, Options), Options)!
            );

        public static ValueComparer<T> CreateComparer<T>() where T : class =>
            new(
                (c1, c2) => c1 != null && c2 != null && JsonSerializer.Serialize(c1, Options) == JsonSerializer.Serialize(c2, Options),
                c => c.GetHashCode(),
                c => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(c, Options), Options)!
            );
    }
}
