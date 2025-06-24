using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; private set; } = string.Empty;

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(true, value, string.Empty);
    public static Result<T> Failure<T>(string error) => new(false, default!, error);
}

public class Result<T> : Result
{
    public T Value { get; private set; }

    protected internal Result(bool isSuccess, T value, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
}