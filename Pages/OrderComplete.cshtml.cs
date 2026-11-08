using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_WebApplication.Pages;

public class OrderCompleteModel : PageModel
{
    public string OrderId { get; private set; } = "";

    public void OnGet(int id) => OrderId = id.ToString("D5");
}
