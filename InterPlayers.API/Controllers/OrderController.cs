using InterPlayers.Application.Interfaces.Services;
using InterPlayers.Contracts.DTOs.Orders;
using InterPlayers.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InterPlayers.API.Controllers;

[Route("api/v1/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    /// <summary>
    /// Obtem todos os pedidos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Lista de pedidos",
        Type = typeof(IList<Order>)
    )]
    public async Task<ActionResult<IEnumerable<Order>>> Get(
        [FromQuery] int orderId,
        [FromQuery] int productId,
        [FromQuery] string? productName
    )
    {
        return Ok(await orderService.GetAllAsync(orderId, productId, productName));
    }

    /// <summary>
    /// Obtem pedido pelo ID
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Pedido encontrado",
        Type = typeof(Order)
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Pedido não encontrado",
        Type = typeof(string)
    )]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var order = await orderService.GetAsync(id);

        return Ok(order);
    }

    /// <summary>
    /// Cria novo pedido
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Description = "Pedido criado",
        Type = typeof(Order)
    )]
    public async Task<ActionResult<Order>> Post()
    {
        var newOrder = await orderService.AddAsync(new());

        return CreatedAtAction(nameof(GetById), new { Id = newOrder.Id }, newOrder);
    }
}
