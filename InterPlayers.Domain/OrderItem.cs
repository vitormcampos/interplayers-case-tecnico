using InterPlayers.Domain.Exceptions;

namespace InterPlayers.Domain;

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int? ProductId { get; private set; }
    public Product Product { get; set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal SubTotal => Quantity * UnitPrice;

    protected OrderItem() { }

    public OrderItem(int orderId, int? productId, int quantity, decimal unitPrice)
    {
        SetOrderId(orderId);
        SetProductId(productId);
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    /// <summary>
    /// Updates the product information with the specified product identifier, quantity, and unit price.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to update. Specify null to clear the current product selection.</param>
    /// <param name="quantity">The quantity of the product. Must be zero or greater.</param>
    /// <param name="unitPrice">The unit price of the product. Must be zero or greater.</param>
    public void Update(int? productId, int quantity, decimal unitPrice)
    {
        SetProductId(productId);
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    /// <summary>
    /// Sets the order identifier for the current instance.
    /// </summary>
    /// <param name="orderId">The unique identifier to assign to the order. Must be greater than zero.</param>
    /// <exception cref="DomainValidationException">Thrown when <paramref name="orderId"/> is less than or equal to zero.</exception>
    private void SetOrderId(int orderId)
    {
        if (orderId <= 0)
            throw new DomainValidationException("OrderId must be greater than zero.");

        OrderId = orderId;
    }

    /// <summary>
    /// Sets the product identifier for the current instance.
    /// </summary>
    /// <param name="productId">The product identifier to assign. Must be greater than zero if provided; otherwise, null to indicate no product.</param>
    /// <exception cref="DomainValidationException">Thrown when the specified productId is less than or equal to zero.</exception>
    private void SetProductId(int? productId)
    {
        if (productId.HasValue && productId <= 0)
            throw new DomainValidationException(
                "ProductId must be greater than zero when provided."
            );

        ProductId = productId;
    }

    /// <summary>
    /// Sets the quantity for the current instance after validating that it is greater than zero.
    /// </summary>
    /// <param name="quantity">The quantity value to set. Must be greater than zero.</param>
    /// <exception cref="DomainValidationException">Thrown when the specified quantity is less than or equal to zero.</exception>
    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Quantity must be greater than zero.");

        Quantity = quantity;
    }

    /// <summary>
    /// Sets the unit price for the item.
    /// </summary>
    /// <param name="unitPrice">The unit price to assign. Must be greater than zero.</param>
    /// <exception cref="DomainValidationException">Thrown if <paramref name="unitPrice"/> is less than or equal to zero.</exception>
    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new DomainValidationException("UnitPrice must be greater than zero.");

        UnitPrice = unitPrice;
    }
}
