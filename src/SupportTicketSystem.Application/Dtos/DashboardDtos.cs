namespace SupportTicketSystem.Application.Dtos;

public record DashboardMetricsDto(
    int TotalTickets,
    int OpenTicketsCount,
    int InProgressTicketsCount,
    int ResolvedTicketsCount,
    int SlaBreachedCount,
    double AverageResolutionTimeHours
);
