namespace SupportTicketSystem.Application.Common.Interfaces;

using SupportTicketSystem.Domain.Entities;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
