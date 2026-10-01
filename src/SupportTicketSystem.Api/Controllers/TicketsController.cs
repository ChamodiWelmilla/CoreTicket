namespace SupportTicketSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Application.Dtos;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    public TicketsController(TicketService ticketService) => _ticketService = ticketService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
    {
        var userId = Guid.NewGuid();
        var result = await _ticketService.CreateTicketAsync(request, userId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] TicketStatus? status, [FromQuery] TicketPriority? priority)
    {
        var tickets = await _ticketService.GetTicketsAsync(status, priority);
        return Ok(tickets);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTicketStatusRequest request)
    {
        var userId = Guid.NewGuid();
        await _ticketService.UpdateStatusAsync(id, request, userId);
        return NoContent();
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> AssignAgent(Guid id, [FromBody] AssignAgentRequest request)
    {
        await _ticketService.AssignAgentAsync(id, request.AgentId);
        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentRequest request)
    {
        var userId = Guid.NewGuid();
        await _ticketService.AddCommentAsync(id, request, userId);
        return NoContent();
    }
}
