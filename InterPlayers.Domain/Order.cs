namespace InterPlayers.Domain;

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public IList<OrderItem> Items { get; set; } = [];
}
