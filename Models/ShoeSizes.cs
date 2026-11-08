namespace Razor_WebApplication.Models;

public static class ShoeSizes
{
    public static IReadOnlyList<int> All { get; } = [36, 37, 38, 39, 40, 41, 42, 43, 44, 45];

    public static bool Contains(int size) => All.Contains(size);
}
