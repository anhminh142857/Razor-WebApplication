using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Pages.Admin;

public class OrdersModel(StoreDbContext db) : PageModel
{
    public List<StoreOrder> Orders { get; private set; } = [];
    public string? Message { get; private set; }
    private static readonly string[] AllowedStatuses = ["Chờ xác nhận", "Đang giao", "Hoàn thành", "Đã hủy"];

    public async Task OnGetAsync()
    {
        Orders = await db.Orders.Include(order => order.User).Include(order => order.Items).ThenInclude(item => item.Product)
            .OrderByDescending(order => order.CreatedAt).ToListAsync();
        Message = TempData["Message"] as string;
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
    {
        if (!AllowedStatuses.Contains(status, StringComparer.Ordinal))
        {
            return BadRequest();
        }
        var order = await db.Orders.FindAsync(id);
        if (order is null) return NotFound();
        order.Status = status;
        await db.SaveChangesAsync();
        TempData["Message"] = $"Đã cập nhật trạng thái đơn hàng #{id:D5}.";
        return RedirectToPage();
    }
}
