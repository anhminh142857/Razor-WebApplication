using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;

namespace Razor_WebApplication.Pages.Admin;

public class CustomersModel(StoreDbContext db) : PageModel
{
    public List<CustomerRow> Customers { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var users = await db.Users.Where(user => user.Role == "User").ToListAsync();
        var orderTotals = await db.Orders.GroupBy(order => order.UserId)
            .Select(group => new { UserId = group.Key, OrderCount = group.Count(), TotalSpent = group.Sum(order => order.Total) })
            .ToDictionaryAsync(group => group.UserId);

        Customers = users.Select(user =>
        {
            var totals = orderTotals.GetValueOrDefault(user.Id);
            return new CustomerRow(user.FullName, user.Email, user.CreatedAt, totals?.OrderCount ?? 0, totals?.TotalSpent ?? 0);
        }).OrderBy(customer => customer.FullName).ToList();
    }
}

public record CustomerRow(string FullName, string Email, DateTime CreatedAt, int OrderCount, decimal TotalSpent);
