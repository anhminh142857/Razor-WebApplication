using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;

namespace Razor_WebApplication.Pages;

public class IndexModel(StoreDbContext db) : PageModel
{
    public IReadOnlyList<Razor_WebApplication.Models.Product> Products { get; private set; } = [];
    public int ProductCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "newest";

    public async Task OnGetAsync()
    {
        var products = db.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var search = SearchQuery.Trim();
            products = products.Where(product =>
                product.Name.Contains(search) ||
                product.Category.Contains(search) ||
                product.Description.Contains(search));
        }
        if (MinPrice.HasValue)
        {
            products = products.Where(product => product.Price >= MinPrice.Value);
        }
        if (MaxPrice.HasValue)
        {
            products = products.Where(product => product.Price <= MaxPrice.Value);
        }

        products = Sort switch
        {
            "price-asc" => products.OrderBy(product => product.Price).ThenBy(product => product.Id),
            "price-desc" => products.OrderByDescending(product => product.Price).ThenBy(product => product.Id),
            _ => products.OrderByDescending(product => product.Id)
        };

        Products = await products.ToListAsync();
        ProductCount = Products.Count;
    }
}
