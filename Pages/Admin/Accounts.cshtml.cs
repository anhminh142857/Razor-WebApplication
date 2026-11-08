using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Data;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Pages.Admin;

public class AccountsModel(StoreDbContext db, PasswordHasher<AppUser> passwordHasher) : PageModel
{
    public List<AppUser> Accounts { get; private set; } = [];
    public string? Message { get; private set; }
    public bool IsError { get; private set; }

    [BindProperty] public string FullName { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string Role { get; set; } = "User";

    public async Task OnGetAsync() => await LoadAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        var normalizedEmail = Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(FullName) || Password.Length < 8 || Role is not ("Admin" or "User"))
        {
            Message = "Vui lòng nhập tên, mật khẩu ít nhất 8 ký tự và vai trò hợp lệ.";
            IsError = true;
            await LoadAsync();
            return Page();
        }
        if (await db.Users.AnyAsync(user => user.Email == normalizedEmail))
        {
            Message = "Email này đã được sử dụng.";
            IsError = true;
            await LoadAsync();
            return Page();
        }

        var account = new AppUser { FullName = FullName.Trim(), Email = normalizedEmail, Role = Role };
        account.PasswordHash = passwordHasher.HashPassword(account, Password);
        db.Users.Add(account);
        await db.SaveChangesAsync();
        TempData["Message"] = "Đã tạo tài khoản mới.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Accounts = await db.Users.OrderBy(user => user.Role).ThenBy(user => user.FullName).ToListAsync();
        Message ??= TempData["Message"] as string;
        IsError |= TempData["IsError"] as bool? ?? false;
    }
}
