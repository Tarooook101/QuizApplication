﻿namespace QuizApplication.API.Models.Common
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public ErrorResponse(string message) => Message = message;
    }
}
