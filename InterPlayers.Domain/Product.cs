using InterPlayers.Domain.Exceptions;

namespace InterPlayers.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public decimal Price { get; private set; }

    protected Product() { }

    public Product(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
    }

    /// <summary>
    /// Sets the name of the product.
    /// </summary>
    /// <param name="name">The new name to assign to the product. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <exception cref="DomainValidationException">Thrown if <paramref name="name"/> is null, empty, or consists only of white-space characters.</exception>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("The product name is required.");
        }

        Name = name;
    }

    /// <summary>
    /// Sets the price of the product to the specified value.
    /// </summary>
    /// <param name="price">The new price to assign to the product. Must be greater than zero.</param>
    /// <exception cref="DomainValidationException">Thrown when the specified price is less than or equal to zero.</exception>
    public void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new DomainValidationException("The product price must be greater than zero.");
        }

        Price = price;
    }

    /// <summary>
    /// Updates the name and price of the item.
    /// </summary>
    /// <param name="name">The new name to assign to the item. Cannot be null or empty.</param>
    /// <param name="price">The new price to assign to the item. Must be greater than or equal to 0.</param>
    /// <exception cref="DomainValidationException">Thrown when any rule has been violated</exception>
    public void Update(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
    }
}
