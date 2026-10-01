namespace SupportTicketSystem.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Domain.Entities;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketHistory> TicketHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
