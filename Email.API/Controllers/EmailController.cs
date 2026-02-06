using Email.API.Messaging;
using Email.Contracts;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Email.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly RabbitmqEmailProducer _publisher;

    public EmailController()
    {
        _publisher = new RabbitmqEmailProducer();
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailMessageV1 message)
    {
        if (message == null)
            return BadRequest("Mensagem inválida.");

        await _publisher.PublishAsync(message);

        return Accepted(new { message = "Email enfileirado com sucesso" });
    }

    [HttpPost("send-invalid")]
    public async Task<IActionResult> SendInvalid()
    {
        var invalidJson = "{ invalid json ";
        var _factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        var body = Encoding.UTF8.GetBytes(invalidJson);

        await channel.BasicPublishAsync(
                exchange: "",
                mandatory: false,
                routingKey: "email.send",
                body:body);

        return Ok("Mensagem inválida enviada");
    }

}
