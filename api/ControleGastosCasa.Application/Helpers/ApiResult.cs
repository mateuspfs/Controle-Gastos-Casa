namespace ControleGastosCasa.Application.Helpers;

public class ApiResult<T>
{
    public bool Success { get; init; }
    public List<string> Errors { get; init; } = [];
    public T? Data { get; init; }

    public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };

    public static ApiResult<T> Fail(params string[] errors) => new()
    {
        Success = false,
        Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim()).ToList() ?? new List<string>()
    };
}

