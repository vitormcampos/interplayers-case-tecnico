using InterPlayers.Domain.Exceptions;

namespace InterPlayers.Domain;

public class Order
{
    public int Id { get; private set; }
    public decimal Total { get; private set; } // handled by database trigger
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() { }

    public Order(IEnumerable<OrderItem>? items = null)
    {
        if (items != null)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }
        RecalculateTotal();
    }

    /// <summary>
    /// Adds the specified item to the order. If an item with the same product identifier already exists, increases its
    /// quantity and updates its unit price.
    /// </summary>
    /// <param name="item">The order item to add. Cannot be null. If the item's ProductId matches an existing item, the quantities are
    /// combined and the unit price is updated.</param>
    /// <exception cref="DomainValidationException">Thrown if the item parameter is null.</exception>
    public void AddItem(OrderItem item)
    {
        if (item is null)
            throw new DomainValidationException(nameof(item));

        if (item.ProductId is not null)
        {
            var existing = _items.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existing != null)
            {
                existing.Update(
                    existing.ProductId,
                    existing.Quantity + item.Quantity,
                    item.UnitPrice
                );

                RecalculateTotal();
                return;
            }
        }

        _items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Removes the item with the specified identifier from the collection.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to remove.</param>
    /// <exception cref="DomainValidationException">Thrown if no item with the specified identifier exists in the collection.</exception>
    public void RemoveItem(int itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
            throw new DomainValidationException("Item not found.");

        _items.Remove(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Updates the details of an existing item in the collection with the specified values.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to update.</param>
    /// <param name="productId">The identifier of the new product to associate with the item.</param>
    /// <param name="quantity">The new quantity to set for the item. Must be a non-negative value.</param>
    /// <param name="unitPrice">The new unit price to set for the item. Must be a non-negative value.</param>
    /// <exception cref="DomainValidationException">Thrown if an item with the specified itemId does not exist.</exception>
    public void UpdateItem(int itemId, int productId, int quantity, decimal unitPrice)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
            throw new DomainValidationException("Item not found.");

        item.Update(productId, quantity, unitPrice);
        RecalculateTotal();
    }

    /// <summary>
    /// Recalculates the total amount for the order based on the current items.
    /// </summary>
    /// <exception cref="DomainValidationException">Thrown if the recalculated total is negative.</exception>
    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.SubTotal);

        if (Total < 0)
            throw new DomainValidationException("Order total cannot be negative.");
    }
}
