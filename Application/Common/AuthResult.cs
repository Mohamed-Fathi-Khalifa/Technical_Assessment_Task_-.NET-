namespace Application.Common;

public record AuthResult(int UserId, string Email, string Name, string Token);
