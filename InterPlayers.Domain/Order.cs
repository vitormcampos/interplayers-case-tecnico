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
    /// Adiciona Itens ao pedido, checando se há duplicatas e mesclando
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DomainValidationException"></exception>
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
    /// Remove um item do pedido e recalcula o valor total
    /// </summary>
    /// <param name="itemId"></param>
    /// <exception cref="DomainValidationException"></exception>
    public void RemoveItem(int itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
            throw new DomainValidationException("Item not found.");

        _items.Remove(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Atualiza um item do pedido e recalcula o valor total
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    /// <exception cref="DomainValidationException"></exception>
    public void UpdateItem(int itemId, int quantity, decimal unitPrice)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
            throw new DomainValidationException("Item not found.");

        item.Update(item.ProductId, quantity, unitPrice);
        RecalculateTotal();
    }

    /// <summary>
    /// Calcula o valor total do pedido, usado para controle interno, o total do pedido é recalculado no banco de dados vai trigger
    /// </summary>
    /// <exception cref="DomainValidationException"></exception>
    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.SubTotal);

        if (Total < 0)
            throw new DomainValidationException("Order total cannot be negative.");
    }
}
