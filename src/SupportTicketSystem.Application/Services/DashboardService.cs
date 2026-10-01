namespace SupportTicketSystem.Application.Services;

using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Common.Interfaces;
using SupportTicketSystem.Application.Dtos;
using SupportTicketSystem.Domain.Enums;

public class DashboardService
{
    private readonly IApplicationDbContext _context;

    public DashboardService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetricsDto> GetMetricsAsync()
    {
        var tickets = await _context.Tickets.AsNoTracking().ToListAsync();

        var total = tickets.Count;
        var open = tickets.Count(t => t.Status == TicketStatus.Open);
        var inProgress = tickets.Count(t => t.Status == TicketStatus.InProgress);
        var resolved = tickets.Count(t => t.Status == TicketStatus.Resolved);
        var breached = tickets.Count(t => t.IsSlaBreached);

        var resolvedTickets = tickets.Where(t => t.ResolvedAt.HasValue).ToList();
        var avgResolutionHours = resolvedTickets.Any()
            ? resolvedTickets.Average(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours)
            : 0;

        return new DashboardMetricsDto(
            total,
            open,
            inProgress,
            resolved,
            breached,
            Math.Round(avgResolutionHours, 2)
        );
    }
}
