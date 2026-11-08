using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Pages.Admin;

public class IndexModel(StoreDbContext db) : PageModel
{
    public int AccountCount { get; private set; }
    public int ProductCount { get; private set; }
    public int CustomerCount { get; private set; }
    public int AdminCount { get; private set; }
    public List<AppUser> RecentAccounts { get; private set; } = [];

    public async Task OnGetAsync()
    {
        AccountCount = await db.Users.CountAsync();
        ProductCount = await db.Products.CountAsync();
        CustomerCount = await db.Users.CountAsync(user => user.Role == "User");
        AdminCount = await db.Users.CountAsync(user => user.Role == "Admin");
        RecentAccounts = await db.Users.OrderByDescending(user => user.CreatedAt).Take(5).ToListAsync();
    }
}
