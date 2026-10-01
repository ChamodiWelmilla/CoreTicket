namespace SupportTicketSystem.Infrastructure.Services;

using SupportTicketSystem.Application.Common.Interfaces;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
