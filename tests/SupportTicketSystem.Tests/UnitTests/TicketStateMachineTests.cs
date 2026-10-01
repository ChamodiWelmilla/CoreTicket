namespace SupportTicketSystem.Tests.UnitTests;

using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Exceptions;
using Xunit;

public class TicketStateMachineTests
{
    [Fact]
    public void TransitionTo_ShouldAddHistoryLog_WhenStatusChanges()
    {
        var ticket = new Ticket { Status = TicketStatus.Open };
        var userId = Guid.NewGuid();

        ticket.TransitionTo(TicketStatus.InProgress, userId, "Starting investigation");

        Assert.Equal(TicketStatus.InProgress, ticket.Status);
        Assert.Single(ticket.HistoryLogs);
    }

    [Fact]
    public void TransitionTo_ShouldThrowDomainException_WhenTicketIsClosed()
    {
        var ticket = new Ticket { Status = TicketStatus.Closed };

        Assert.Throws<DomainException>(() =>
            ticket.TransitionTo(TicketStatus.InProgress, Guid.NewGuid(), "Reopening ticket"));
    }
}
