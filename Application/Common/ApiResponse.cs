namespace Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];

    // ── Factory helpers ───────────────────────────────────────────────────────

    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors ?? [] };

    public static ApiResponse<T> Fail(string message, string error)
        => Fail(message, [error]);
}
