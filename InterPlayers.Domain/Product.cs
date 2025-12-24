namespace InterPlayers.Domain;

public class Product
{
    public int Id { get; }
    public string Name { get; private set; } = "";
    public decimal Price { get; private set; }

    public Product(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The product name is required.");
        }

        Name = name;
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException("The product price must be greater than zero.");
        }

        Price = price;
    }
}
