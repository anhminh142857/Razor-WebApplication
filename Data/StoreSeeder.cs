using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Data;

public static class StoreSeeder
{
    public static async Task SeedAsync(StoreDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        await EnsureOrderItemSizeColumnAsync(db);

        if (!await db.Users.AnyAsync())
        {
            var hasher = new PasswordHasher<AppUser>();
            var admin = new AppUser { FullName = "Quản trị viên", Email = "admin@shoestore.vn", Role = "Admin" };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
            var customer = new AppUser { FullName = "Nguyễn Minh Anh", Email = "user@shoestore.vn", Role = "User" };
            customer.PasswordHash = hasher.HashPassword(customer, "User@123");
            db.Users.AddRange(admin, customer);
        }

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { Name = "Cloud Runner", Category = "Chạy bộ", Price = 1890000, Stock = 18, Description = "Êm nhẹ cho mọi cung đường chạy hằng ngày.", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=85" },
                new Product { Name = "Street Classic", Category = "Thời trang", Price = 1490000, Stock = 24, Description = "Thiết kế tối giản, dễ phối đồ mỗi ngày.", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?auto=format&fit=crop&w=900&q=85" },
                new Product { Name = "Court Pro", Category = "Thể thao", Price = 2190000, Stock = 12, Description = "Bám sân tốt, hỗ trợ chuyển hướng linh hoạt.", ImageUrl = "https://images.unsplash.com/photo-1600185365483-26d7a4cc7519?auto=format&fit=crop&w=900&q=85" },
                new Product { Name = "Urban High", Category = "Thời trang", Price = 1750000, Stock = 15, Description = "Phom cao cổ cá tính với chất liệu thoáng khí.", ImageUrl = "https://images.unsplash.com/photo-1552346154-21d32810aba3?auto=format&fit=crop&w=900&q=85" },
                new Product { Name = "Trail Explorer", Category = "Outdoor", Price = 2450000, Stock = 9, Description = "Sẵn sàng cho những chuyến đi khám phá cuối tuần.", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=85" },
                new Product { Name = "Daily Comfort", Category = "Thời trang", Price = 1290000, Stock = 30, Description = "Thoải mái cả ngày với đế lót mềm mại.", ImageUrl = "https://images.unsplash.com/photo-1495555961986-6d4c1ecb7be3?auto=format&fit=crop&w=900&q=85" });
        }

        await db.SaveChangesAsync();
    }

    private static async Task EnsureOrderItemSizeColumnAsync(StoreDbContext db)
    {
        await db.Database.OpenConnectionAsync();
        try
        {
            var connection = db.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA table_info('OrderItems')";
            await using var reader = await command.ExecuteReaderAsync();
            var hasSizeColumn = false;
            while (await reader.ReadAsync())
            {
                if (string.Equals(reader.GetString(reader.GetOrdinal("name")), "Size", StringComparison.OrdinalIgnoreCase))
                {
                    hasSizeColumn = true;
                    break;
                }
            }
            await reader.CloseAsync();

            if (!hasSizeColumn)
            {
                await db.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE \"OrderItems\" ADD COLUMN \"Size\" INTEGER NOT NULL DEFAULT 40");
            }
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }
}
