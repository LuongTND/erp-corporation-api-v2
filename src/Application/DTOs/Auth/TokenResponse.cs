namespace Application.DTOs.Auth;

public record TokenResponse(
    string Token,
    DateTime Expiry,
    string EmployeeCode,
    string FullName,
    Guid UserId,
    string RefreshToken,
    DateTime RefreshTokenExpiry);
