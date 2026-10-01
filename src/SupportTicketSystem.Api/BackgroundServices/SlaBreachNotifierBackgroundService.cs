namespace SupportTicketSystem.Api.BackgroundServices;

using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Common.Interfaces;
using SupportTicketSystem.Domain.Enums;

public class SlaBreachNotifierBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlaBreachNotifierBackgroundService> _logger;

    public SlaBreachNotifierBackgroundService(IServiceProvider serviceProvider, ILogger<SlaBreachNotifierBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Breach Background Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var now = DateTime.UtcNow;

                var breachedTickets = await db.Tickets
                    .Where(t => !t.IsSlaBreached &&
                                t.Status != TicketStatus.Resolved &&
                                t.Status != TicketStatus.Closed &&
                                t.TargetResolutionTime < now)
                    .ToListAsync(stoppingToken);

                if (breachedTickets.Any())
                {
                    foreach (var ticket in breachedTickets)
                    {
                        ticket.IsSlaBreached = true;
                        _logger.LogWarning("SLA BREACH ALERT: Ticket {TicketNumber} passed resolution target!", ticket.TicketNumber);
                    }
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SLA breaches.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
