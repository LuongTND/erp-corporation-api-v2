using System.Text.Json.Serialization;

namespace Contract;

public class ApiResponse<T>
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; init; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("errors")]
    public IDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
        => new() { IsSuccess = true, StatusCode = statusCode, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, IDictionary<string, string[]>? errors = null)
        => new() { IsSuccess = false, StatusCode = statusCode, Message = message, Errors = errors };
}
