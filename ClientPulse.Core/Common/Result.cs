using System.Collections.Generic;
using System.Linq;

namespace ClientPulse.Core.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public IReadOnlyList<string> Errors { get; }

    private Result(bool isSuccess, T? data, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors?.ToList() ?? new List<string>();
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public static Result<T> Failure(string error) => new(false, default, new[] { error });
}
