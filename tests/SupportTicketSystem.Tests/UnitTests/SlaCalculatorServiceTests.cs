namespace SupportTicketSystem.Tests.UnitTests;

using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Enums;
using Xunit;

public class SlaCalculatorServiceTests
{
    private readonly SlaCalculatorService _sut = new();

    [Theory]
    [InlineData(TicketPriority.Urgent, 1)]
    [InlineData(TicketPriority.High, 4)]
    [InlineData(TicketPriority.Medium, 24)]
    [InlineData(TicketPriority.Low, 48)]
    public void CalculateTargetResolutionTime_ShouldAddCorrectHours(TicketPriority priority, int expectedHours)
    {
        var createdAt = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        var result = _sut.CalculateTargetResolutionTime(priority, createdAt);
        Assert.Equal(createdAt.AddHours(expectedHours), result);
    }
}
