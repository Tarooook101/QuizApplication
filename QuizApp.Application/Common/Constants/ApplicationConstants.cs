namespace QuizApp.Application.Common.Constants;

public static class ApplicationConstants
{
    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const int MinPageNumber = 1;
    }

    public static class Cache
    {
        public const int DefaultCacheExpirationMinutes = 30;
        public const int ShortCacheExpirationMinutes = 5;
        public const int LongCacheExpirationMinutes = 120;
    }

    public static class Validation
    {
        public const int MinPasswordLength = 8;
        public const int MaxNameLength = 100;
        public const int MaxEmailLength = 256;
        public const int MaxDescriptionLength = 1000;
    }
}