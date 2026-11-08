using System.Text.Json;
using Razor_WebApplication.Models;

namespace Razor_WebApplication.Data;

public static class CartSession
{
    private const string Key = "shoe-store-cart";

    public static Dictionary<string, int> Get(ISession session)
    {
        var json = session.GetString(Key);
        if (string.IsNullOrEmpty(json))
        {
            return [];
        }

        var savedCart = JsonSerializer.Deserialize<Dictionary<string, int>>(json)
            ?? throw new InvalidDataException("Dữ liệu giỏ hàng không hợp lệ.");
        var cart = new Dictionary<string, int>();
        foreach (var (key, quantity) in savedCart)
        {
            if (int.TryParse(key, out var legacyProductId) && legacyProductId > 0)
            {
                cart[MakeKey(legacyProductId, 40)] = quantity;
                continue;
            }

            var parts = key.Split(':', 2);
            if (parts.Length != 2 || !int.TryParse(parts[0], out var productId) ||
                !int.TryParse(parts[1], out var size) || productId <= 0 || !ShoeSizes.Contains(size))
            {
                throw new InvalidDataException("Dữ liệu giỏ hàng không hợp lệ.");
            }

            cart[key] = quantity;
        }

        return cart;
    }

    public static string MakeKey(int productId, int size) => $"{productId}:{size}";

    public static (int ProductId, int Size) ParseKey(string key)
    {
        var parts = key.Split(':', 2);
        if (parts.Length != 2 || !int.TryParse(parts[0], out var productId) ||
            !int.TryParse(parts[1], out var size) || productId <= 0 || !ShoeSizes.Contains(size))
        {
            throw new InvalidDataException("Dữ liệu giỏ hàng không hợp lệ.");
        }

        return (productId, size);
    }

    public static void Save(ISession session, Dictionary<string, int> cart) =>
        session.SetString(Key, JsonSerializer.Serialize(cart));
}
