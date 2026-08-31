namespace btb23.Services;

public static class ProductImage
{
    public static string CssClass(string name, string category)
    {
        if (name.Contains("old school", StringComparison.OrdinalIgnoreCase)) return "shoe-2";
        if (name.Contains("chuck", StringComparison.OrdinalIgnoreCase)) return "shoe-3";
        if (name.Contains("adapt", StringComparison.OrdinalIgnoreCase)) return "shoe-4";
        if (name.Contains("blue", StringComparison.OrdinalIgnoreCase)) return "shoe-6";
        if (name.Contains("air max", StringComparison.OrdinalIgnoreCase)) return "shoe-5";
        return category == "Nike" ? "shoe-5" : category == "Converse" ? "shoe-3" : "shoe-1";
    }
}
