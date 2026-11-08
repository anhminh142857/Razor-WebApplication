using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Pages.Product;

public class DetailsModel(StoreDbContext db) : PageModel
{
    public Razor_WebApplication.Models.Product? Product { get; private set; }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public int Quantity { get; set; } = 1;

    [BindProperty]
    public int Size { get; set; } = 40;

    public IReadOnlyList<int> AvailableSizes => ShoeSizes.All;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Product = await db.Products.FindAsync(id);
        return Product is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync()
    {
        Product = await db.Products.FindAsync(Id);
        if (Product is null)
        {
            return NotFound();
        }

        if (!ShoeSizes.Contains(Size))
        {
            TempData["Message"] = "Vui lòng chọn size giày hợp lệ.";
            return RedirectToPage(new { id = Id });
        }

        var cart = CartSession.Get(HttpContext.Session);
        var productKeyPrefix = $"{Id}:";
        var quantityInCart = cart.Where(line => line.Key.StartsWith(productKeyPrefix, StringComparison.Ordinal))
            .Sum(line => line.Value);
        if (Quantity < 1 || Quantity > Product.Stock - quantityInCart)
        {
            TempData["Message"] = "Số lượng vượt quá số sản phẩm còn trong kho.";
            return RedirectToPage(new { id = Id });
        }

        var key = CartSession.MakeKey(Id, Size);
        cart[key] = cart.GetValueOrDefault(key) + Quantity;
        CartSession.Save(HttpContext.Session, cart);
        TempData["Message"] = $"Đã thêm sản phẩm size {Size} vào giỏ hàng.";
        return RedirectToPage(new { id = Id });
    }
}
