namespace InterPlayers.Domain;

public class OrderItem
{
    public required int ProductId { get; set; }
    public required Product Product { get; set; }
    public required int OrderId { get; set; }
    public required Order Order { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
