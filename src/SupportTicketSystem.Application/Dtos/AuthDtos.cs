namespace SupportTicketSystem.Application.Dtos;

using SupportTicketSystem.Domain.Enums;

public record RegisterRequest(string FullName, string Email, string Password, UserRole Role);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string FullName, string Role);
