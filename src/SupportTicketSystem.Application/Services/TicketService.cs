namespace SupportTicketSystem.Application.Services;

using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Common.Interfaces;
using SupportTicketSystem.Application.Dtos;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Exceptions;

public class TicketService
{
    private readonly IApplicationDbContext _context;
    private readonly SlaCalculatorService _slaCalculator;
    private readonly IDateTimeProvider _dateTime;

    public TicketService(IApplicationDbContext context, SlaCalculatorService slaCalculator, IDateTimeProvider dateTime)
    {
        _context = context;
        _slaCalculator = slaCalculator;
        _dateTime = dateTime;
    }

    public async Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, Guid userId)
    {
        var ticketNumber = $"TICK-{Random.Shared.Next(100000, 999999)}";
        var createdAt = _dateTime.UtcNow;
        var targetResolution = _slaCalculator.CalculateTargetResolutionTime(request.Priority, createdAt);

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedByUserId = userId,
            CreatedAt = createdAt,
            TargetResolutionTime = targetResolution
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        return MapToDto(ticket, user?.Email, null);
    }

    public async Task<List<TicketDto>> GetTicketsAsync(TicketStatus? status = null, TicketPriority? priority = null)
    {
        var query = _context.Tickets
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedAgent)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (priority.HasValue) query = query.Where(t => t.Priority == priority.Value);

        return await query.Select(t => new TicketDto(
            t.Id,
            t.TicketNumber,
            t.Title,
            t.Description,
            t.Priority,
            t.Status,
            t.IsSlaBreached,
            t.TargetResolutionTime,
            t.CreatedAt,
            t.CreatedByUser.Email,
            t.AssignedAgent != null ? t.AssignedAgent.Email : null
        )).ToListAsync();
    }

    public async Task UpdateStatusAsync(Guid ticketId, UpdateTicketStatusRequest request, Guid userId)
    {
        var ticket = await _context.Tickets.Include(t => t.HistoryLogs).FirstOrDefaultAsync(t => t.Id == ticketId)
            ?? throw new DomainException("Ticket not found.");

        ticket.TransitionTo(request.NewStatus, userId, request.Note);
        await _context.SaveChangesAsync();
    }

    public async Task AssignAgentAsync(Guid ticketId, Guid agentId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId)
            ?? throw new DomainException("Ticket not found.");

        var agent = await _context.Users.FirstOrDefaultAsync(u => u.Id == agentId && u.Role == UserRole.Agent)
            ?? throw new DomainException("Agent not found or specified user is not an agent.");

        ticket.AssignedAgentId = agent.Id;
        await _context.SaveChangesAsync();
    }

    public async Task AddCommentAsync(Guid ticketId, AddCommentRequest request, Guid userId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId)
            ?? throw new DomainException("Ticket not found.");

        var comment = new TicketComment
        {
            TicketId = ticket.Id,
            UserId = userId,
            Message = request.Message,
            IsInternalNote = request.IsInternalNote,
            CreatedAt = _dateTime.UtcNow
        };

        _context.TicketComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    private static TicketDto MapToDto(Ticket t, string? creatorEmail, string? agentEmail)
        => new(t.Id, t.TicketNumber, t.Title, t.Description, t.Priority, t.Status, t.IsSlaBreached, t.TargetResolutionTime, t.CreatedAt, creatorEmail ?? "", agentEmail);
}
