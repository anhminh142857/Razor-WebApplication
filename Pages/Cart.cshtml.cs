using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;
using Razor_WebApplication.Models;
using StoreProduct = Razor_WebApplication.Models.Product;

namespace Razor_WebApplication.Pages;

public class CartModel(StoreDbContext db) : PageModel
{
    public List<CartLine> Lines { get; private set; } = [];
    public decimal Total => Lines.Sum(line => line.Product.Price * line.Quantity);
    public string? Message { get; private set; }

    public async Task OnGetAsync() => await LoadLinesAsync();

    public async Task<IActionResult> OnPostUpdateAsync(int productId, int size, int quantity)
    {
        var product = await db.Products.FindAsync(productId);
        if (product is null)
        {
            return NotFound();
        }
        if (!ShoeSizes.Contains(size))
        {
            return BadRequest();
        }

        var cart = CartSession.Get(HttpContext.Session);
        var key = CartSession.MakeKey(productId, size);
        if (quantity <= 0 || product.Stock <= 0)
        {
            cart.Remove(key);
        }
        else
        {
            var otherSizesQuantity = cart.Where(line => line.Key.StartsWith($"{productId}:", StringComparison.Ordinal) && line.Key != key)
                .Sum(line => line.Value);
            var availableQuantity = Math.Max(0, product.Stock - otherSizesQuantity);
            if (availableQuantity == 0)
            {
                cart.Remove(key);
            }
            else
            {
                cart[key] = Math.Min(quantity, availableQuantity);
            }
        }

        CartSession.Save(HttpContext.Session, cart);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        var cart = CartSession.Get(HttpContext.Session);
        if (cart.Count == 0)
        {
            return RedirectToPage();
        }

        var entries = cart.Select(line => (Key: CartSession.ParseKey(line.Key), line.Value)).ToList();
        var productIds = entries.Select(entry => entry.Key.ProductId).Distinct().ToList();
        var products = await db.Products.Where(product => productIds.Contains(product.Id)).ToListAsync();
        var productsById = products.ToDictionary(product => product.Id);
        if (products.Count != productIds.Count || cart.Any(line => line.Value <= 0) ||
            products.Any(product => entries.Where(entry => entry.Key.ProductId == product.Id).Sum(entry => entry.Value) > product.Stock))
        {
            Message = "Một số sản phẩm không còn đủ số lượng. Vui lòng cập nhật giỏ hàng.";
            await LoadLinesAsync();
            return Page();
        }

        var order = new StoreOrder
        {
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Total = entries.Sum(entry => productsById[entry.Key.ProductId].Price * entry.Value),
            Items = entries.Select(entry => new OrderItem
            {
                ProductId = entry.Key.ProductId,
                Size = entry.Key.Size,
                Quantity = entry.Value,
                UnitPrice = productsById[entry.Key.ProductId].Price
            }).ToList()
        };

        await using var transaction = await db.Database.BeginTransactionAsync();
        db.Orders.Add(order);
        foreach (var product in products)
        {
            product.Stock -= entries.Where(entry => entry.Key.ProductId == product.Id).Sum(entry => entry.Value);
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        HttpContext.Session.Remove("shoe-store-cart");
        return RedirectToPage("/OrderComplete", new { id = order.Id });
    }

    private async Task LoadLinesAsync()
    {
        var cart = CartSession.Get(HttpContext.Session);
        var entries = cart.Select(line => (Key: CartSession.ParseKey(line.Key), line.Value)).ToList();
        var productIds = entries.Select(entry => entry.Key.ProductId).Distinct().ToList();
        var products = await db.Products.Where(product => productIds.Contains(product.Id)).ToDictionaryAsync(product => product.Id);
        Lines = entries.Where(entry => products.ContainsKey(entry.Key.ProductId))
            .Select(entry => new CartLine(products[entry.Key.ProductId], entry.Key.Size, entry.Value)).ToList();
        Message ??= products.Count == productIds.Count ? null : "Một số sản phẩm không còn tồn tại trong cửa hàng.";
    }
}

public record CartLine(StoreProduct Product, int Size, int Quantity);
