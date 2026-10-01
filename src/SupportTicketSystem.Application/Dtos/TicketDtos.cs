namespace SupportTicketSystem.Application.Dtos;

using SupportTicketSystem.Domain.Enums;

public record CreateTicketRequest(string Title, string Description, TicketPriority Priority);

public record UpdateTicketStatusRequest(TicketStatus NewStatus, string Note);

public record TicketDto(
    Guid Id,
    string TicketNumber,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    bool IsSlaBreached,
    DateTime TargetResolutionTime,
    DateTime CreatedAt,
    string CreatedByEmail,
    string? AssignedAgentEmail
);
