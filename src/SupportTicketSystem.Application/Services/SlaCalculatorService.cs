namespace SupportTicketSystem.Application.Services;

using SupportTicketSystem.Domain.Enums;

public class SlaCalculatorService
{
    public DateTime CalculateTargetResolutionTime(TicketPriority priority, DateTime createdAt)
    {
        int hoursToResolve = priority switch
        {
            TicketPriority.Urgent => 1,
            TicketPriority.High => 4,
            TicketPriority.Medium => 24,
            TicketPriority.Low => 48,
            _ => 24
        };

        return createdAt.AddHours(hoursToResolve);
    }
}
