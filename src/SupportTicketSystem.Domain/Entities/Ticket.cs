namespace SupportTicketSystem.Domain.Entities;

using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Exceptions;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public bool IsSlaBreached { get; set; } = false;

    public DateTime TargetResolutionTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public Guid? AssignedAgentId { get; set; }
    public User? AssignedAgent { get; set; }

    public ICollection<TicketHistory> HistoryLogs { get; set; } = new List<TicketHistory>();

    public void TransitionTo(TicketStatus newStatus, Guid changedByUserId, string note)
    {
        if (Status == TicketStatus.Closed)
            throw new DomainException("Cannot change status of a closed ticket.");

        if (Status == newStatus)
            return;

        HistoryLogs.Add(new TicketHistory
        {
            TicketId = Id,
            OldStatus = Status,
            NewStatus = newStatus,
            ChangedByUserId = changedByUserId,
            Note = note,
            Timestamp = DateTime.UtcNow
        });

        Status = newStatus;

        if (newStatus == TicketStatus.Resolved || newStatus == TicketStatus.Closed)
        {
            ResolvedAt = DateTime.UtcNow;
        }
    }
}
