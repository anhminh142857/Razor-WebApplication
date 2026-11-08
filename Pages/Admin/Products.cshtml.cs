using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;

namespace Razor_WebApplication.Pages.Admin;

public class ProductsModel(StoreDbContext db) : PageModel
{
    public List<Models.Product> Products { get; private set; } = [];
    public string? Message { get; private set; }
    public bool IsError { get; private set; }

    [BindProperty] public int Id { get; set; }
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Category { get; set; } = "";
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public int Stock { get; set; }
    [BindProperty] public string ImageUrl { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";

    public async Task OnGetAsync(int? id = null)
    {
        await LoadProductsAsync();
        if (id is not null)
        {
            var product = await db.Products.FindAsync(id.Value);
            if (product is not null)
            {
                Id = product.Id;
                Name = product.Name;
                Category = product.Category;
                Price = product.Price;
                Stock = product.Stock;
                ImageUrl = product.ImageUrl;
                Description = product.Description;
            }
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (Price <= 0 || Stock < 0 || string.IsNullOrWhiteSpace(Name))
        {
            Message = "Vui lòng nhập thông tin hợp lệ cho sản phẩm.";
            IsError = true;
            await LoadProductsAsync();
            return Page();
        }

        var product = Id == 0 ? new Models.Product() : await db.Products.FindAsync(Id);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = Name.Trim();
        product.Category = Category.Trim();
        product.Price = Price;
        product.Stock = Stock;
        product.ImageUrl = ImageUrl.Trim();
        product.Description = Description.Trim();
        if (Id == 0) db.Products.Add(product);
        await db.SaveChangesAsync();
        TempData["Message"] = "Đã lưu thông tin sản phẩm.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        if (await db.OrderItems.AnyAsync(item => item.ProductId == id))
        {
            TempData["Message"] = "Không thể xóa sản phẩm đã có trong đơn hàng.";
            TempData["IsError"] = true;
            return RedirectToPage();
        }
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        TempData["Message"] = "Đã xóa sản phẩm.";
        return RedirectToPage();
    }

    private async Task LoadProductsAsync()
    {
        Products = await db.Products.OrderBy(product => product.Id).ToListAsync();
        Message ??= TempData["Message"] as string;
        IsError |= TempData["IsError"] as bool? ?? false;
    }
}
