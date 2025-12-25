using InterPlayers.Application.Interfaces.Services;
using InterPlayers.Contracts.DTOs.OrderItems;
using InterPlayers.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InterPlayers.API.Controllers;

[Route("api/v1/orders/{orderId}/items")]
[ApiController]
public class OrderItemController : ControllerBase
{
    private readonly IOrderService orderService;
    private readonly IProductService productService;

    public OrderItemController(IOrderService orderService, IProductService productService)
    {
        this.orderService = orderService;
        this.productService = productService;
    }

    /// <summary>
    /// Obtem todos os items do pedido
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Lista de items do pedido",
        Type = typeof(IList<OrderItem>)
    )]
    public async Task<ActionResult<IEnumerable<OrderItem>>> Get(int orderId)
    {
        var order = await orderService.GetAsync(orderId);

        return Ok(order.Items);
    }

    /// <summary>
    /// Obtem item pelo ID
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Item encontrado",
        Type = typeof(OrderItem)
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Item não encontrado",
        Type = typeof(string)
    )]
    public async Task<ActionResult<OrderItem>> GetById(int orderId, int id)
    {
        var order = await orderService.GetAsync(orderId);
        var orderItem = order.Items.FirstOrDefault(i => i.Id == id);

        if (orderItem is null)
        {
            return NotFound();
        }

        return Ok(orderItem);
    }

    /// <summary>
    /// Cria novo item
    /// </summary>
    /// <param name="orderId"></param>
    [HttpPost]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Description = "Item criado",
        Type = typeof(OrderItem)
    )]
    public async Task<ActionResult<OrderItem>> Post(
        int orderId,
        [FromBody] CreateOrderItemDto orderItem
    )
    {
        var order = await orderService.GetAsync(orderId);
        var product = await productService.GetAsync(orderItem.ProductId);

        order.AddItem(new(orderId, orderItem.ProductId, orderItem.Quantity, product.Price));

        var updatedOrder = await orderService.Update(orderId, order);
        var createdOrderItem = updatedOrder.Items.FirstOrDefault(p => p.ProductId == product.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { orderId, Id = createdOrderItem.Id },
            createdOrderItem
        );
    }

    /// <summary>
    /// Atualiza item existente
    /// </summary>
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Item atualizado",
        Type = typeof(OrderItem)
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Item não encontrado",
        Type = typeof(string)
    )]
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderItem>> PutAsync(
        int orderId,
        int id,
        [FromBody] CreateOrderItemDto dto
    )
    {
        var order = await orderService.GetAsync(orderId);
        var product = await productService.GetAsync(dto.ProductId);

        order.UpdateItem(id, dto.ProductId, dto.Quantity, product.Price);

        await orderService.Update(orderId, order);

        return order.Items.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Remove item existente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "Item deletado")]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Item não encontrado",
        Type = typeof(string)
    )]
    public async Task<IActionResult> Delete(int orderId, int id)
    {
        var order = await orderService.GetAsync(orderId);

        order.RemoveItem(id);

        await orderService.Update(orderId, order);

        return NoContent();
    }
}
