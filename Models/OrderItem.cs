namespace Razor_WebApplication.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public StoreOrder Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Size { get; set; } = 40;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
