using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CateringBackend.Users.Producer.Queries;
using System;
using CateringBackend.Users.Producer.Commands;

namespace CateringBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProducerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginProducer([FromBody] LoginProducerQuery loginQuery)
        {
            var result = await _mediator.Send(loginQuery);
            return string.IsNullOrWhiteSpace(result) ? BadRequest("Niepowodzenie logowania") : Ok(result);
        }

        [HttpPost("orders/{orderId}/complete")]
        [Authorize(Roles = "producer")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid orderId)
        {
            var result = await _mediator.Send(new CompleteOrderCommand(orderId));

            if (!result.orderExists) return NotFound("Podane zamównienie nie istnieje");
            if (!result.orderCompleted) return BadRequest("Niepowodzenie potwierdzenia wykonania zamówienia");
            return Ok("Powodzenie potwierdzenia wykonania zamówienia");
        }

        [HttpPost("orders/{complaintId}/answer-complaint")]
        [Authorize(Roles = "producer")]
        public async Task<IActionResult> AnswerComplaint([FromRoute] Guid complaintId,
            [FromBody] AnswerComplaintCommand answerComplaintCommand)
        {
            var result = await _mediator.Send(new AnswerComplaintWithIdCommand(answerComplaintCommand, complaintId));

            if (!result.complaintExists) return NotFound($"Podanana reklamacja nie istnieje - {result.errorMessage}");
            if (!result.complaintAnswered) return BadRequest($"Zapisanie nie powiodło się - {result.errorMessage}");
            return CreatedAtAction(nameof(AnswerComplaint), "Zapisano odpowiedź do reklamacji");
        }

        [HttpGet("orders/complaints")]
        [Authorize(Roles = "producer")]
        public async Task<IActionResult> GetOrdersComplaints()
        {
            var result = await _mediator.Send(new GetOrdersComplaintsQuery());
            return Ok(result);
        }

        [HttpGet("orders")]
        [Authorize(Roles = "producer")]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery getOrdersQuery)
        {
            var result = await _mediator.Send(getOrdersQuery);
            return Ok(result);
        }
    }
}