namespace Razor_WebApplication.Models;

public class StoreOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Chờ xác nhận";
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
