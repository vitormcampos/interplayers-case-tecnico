using System.Threading.Tasks;
using InterPlayers.Application.Test.Interfaces.Services;
using InterPlayers.Contracts.DTOs.Products;
using InterPlayers.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InterPlayers.API.Controllers;

[Route("api/v1/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    /// <summary>
    /// Obtem todos os produtos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Lista de produtos",
        Type = typeof(IList<Product>)
    )]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        return Ok(await productService.GetAllAsync());
    }

    /// <summary>
    /// Obtem produto pelo ID
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Produto encontrado",
        Type = typeof(Product)
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Produto não encontrado",
        Type = typeof(string)
    )]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await productService.GetAsync(id);

        return Ok(product);
    }

    /// <summary>
    /// Cria novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Description = "Produto criado",
        Type = typeof(Product)
    )]
    public async Task<ActionResult<Product>> Post([FromBody] CreateProductDto product)
    {
        var newProduct = await productService.AddAsync(product);

        return CreatedAtAction(nameof(GetById), new { Id = newProduct.Id }, newProduct);
    }

    /// <summary>
    /// Atualiza produto existente
    /// </summary>
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Produto atualizado",
        Type = typeof(Product)
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Produto não encontrado",
        Type = typeof(string)
    )]
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> PutAsync(int id, [FromBody] CreateProductDto product)
    {
        return await productService.UpdateAsync(id, product);
    }

    /// <summary>
    /// Remove produto existente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "Produto deletado")]
    [ProducesResponseType(
        StatusCodes.Status404NotFound,
        Description = "Produto não encontrado",
        Type = typeof(string)
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteAsync(id);

        return NoContent();
    }
}
