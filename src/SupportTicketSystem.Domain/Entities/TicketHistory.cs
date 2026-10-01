namespace SupportTicketSystem.Domain.Entities;

using SupportTicketSystem.Domain.Enums;

public class TicketHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public TicketStatus OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
