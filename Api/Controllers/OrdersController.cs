using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiEcommerce.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation("Adiciona um novo pedido")]
    public async Task<IActionResult> Create([FromBody] Application.Orders.Commands.CreateOrder.CreateOrderCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Created($"/api/v1/orders/{id}", id);
    }

    [HttpGet]
    [SwaggerOperation("Retorna um ou mais pedidos a partir do seu status, ou todos caso nenuhm filtro seja adicionado")]
    public async Task<IActionResult> Get([FromQuery] Domain.Enums.OrderStatus? status)
    {
        var result = await _mediator.Send(new Application.Orders.Queries.GetOrders.GetOrdersQuery(status));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Retorna um único pedido a partir do seu ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _mediator.Send(new Application.Orders.Queries.GetOrdersById.GetOrdersByIdQuery(id));
        if (order is null) return NotFound();
        return Ok(order);
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Atualiza um único pedido e seus itens")]
    public async Task<IActionResult> Update(int id, [FromBody] Application.Orders.Commands.UpdateOrder.UpdateOrderCommand cmd)
    {
        if (id != cmd.Id) return BadRequest();

        var result = await _mediator.Send(cmd);
        return result ? Ok() : NotFound();
    }

    [HttpPatch("{id}/cancel")]
    [SwaggerOperation("Cancela um único pedido através do ID, alterando o seu status para tal")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _mediator.Send(new Application.Orders.Commands.CancelOrder.CancelOrderCommand(id));
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Remove um pedido específico através do ID")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new Application.Orders.Commands.DeleteOrder.DeleteOrderCommand(id));
        return result ? Ok() : NotFound();
    }
}
