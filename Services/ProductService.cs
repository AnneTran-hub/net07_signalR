using btb23.Hubs;
using btb23.Models;
using Microsoft.AspNetCore.SignalR;

namespace btb23.Services;

public interface IProductService
{
    IReadOnlyList<ProductDto> GetAll();
    ProductDto? GetById(int id);
    Task AddAsync(ProductDto product);
    Task UpdateAsync(ProductDto product);
    Task DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly object _sync = new();
    private readonly List<ProductDto> _products = new();
    private readonly IHubContext<ProductHub> _hub;
    private int _nextId = 1;

    public ProductService(IHubContext<ProductHub> hub)
    {
        _hub = hub;
        Seed();
    }

    public IReadOnlyList<ProductDto> GetAll()
    {
        lock (_sync)
        {
            return _products.Select(Clone).ToList();
        }
    }

    public ProductDto? GetById(int id)
    {
        lock (_sync)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product is null ? null : Clone(product);
        }
    }

    public async Task AddAsync(ProductDto product)
    {
        lock (_sync)
        {
            product.Id = _nextId++;
            product.Category = ResolveCategory(product.Name, product.Category);
            product.Image = ProductImage.CssClass(product.Name, product.Category);
            _products.Add(Clone(product));
        }
        await NotifyChangedAsync();
    }

    public async Task UpdateAsync(ProductDto product)
    {
        lock (_sync)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing is null)
            {
                return;
            }

            existing.Name = product.Name;
            existing.Alias = product.Alias;
            existing.Price = product.Price;
            existing.Description = product.Description;
            existing.Category = ResolveCategory(product.Name, product.Category);
            existing.Image = ProductImage.CssClass(existing.Name, existing.Category);
        }
        await NotifyChangedAsync();
    }

    public async Task DeleteAsync(int id)
    {
        lock (_sync)
        {
            _products.RemoveAll(p => p.Id == id);
        }
        await NotifyChangedAsync();
    }

    private Task NotifyChangedAsync() => _hub.Clients.All.SendAsync("ProductsChanged");

    private static string ResolveCategory(string name, string currentCategory)
    {
        if (!string.IsNullOrWhiteSpace(currentCategory))
        {
            return currentCategory;
        }

        if (name.Contains("Nike", StringComparison.OrdinalIgnoreCase)) return "Nike";
        if (name.Contains("Converse", StringComparison.OrdinalIgnoreCase)) return "Converse";
        return "Adidas";
    }

    private static ProductDto Clone(ProductDto product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Alias = product.Alias,
        Price = product.Price,
        Description = product.Description,
        Category = product.Category,
        Image = product.Image
    };

    private void Seed()
    {
        var seed = new (int Id, string Name, string Alias, decimal Price, string Category)[]
        {
            (1, "Vans Black", "vans-black", 200, "Adidas"),
            (2, "Vans Old School", "vans-old-school", 200, "Adidas"),
            (3, "Converse Chuck Taylor", "converse-chuck-taylor", 250, "Converse"),
            (4, "Nike Adapt BB", "nike-adapt-bb", 350, "Nike"),
            (6, "Nike Air Max 97", "nike-air-max-97", 350, "Nike"),
            (7, "Nike Air Max 97 Blue", "nike-air-max-97-blue", 350, "Nike"),
            (8, "Converse All Star", "converse-all-star", 250, "Converse"),
            (9, "Vans Classic", "vans-classic", 280, "Adidas"),
            (10, "Nike Air Force 1", "nike-air-force-1", 280, "Nike"),
            (11, "Nike Zoom Pegasus", "nike-zoom-pegasus", 190, "Nike"),
            (12, "Vans Sk8-Hi", "vans-sk8-hi", 120, "Adidas"),
            (13, "Vans Authentic", "vans-authentic", 90, "Adidas"),
        };

        foreach (var s in seed)
        {
            _products.Add(new ProductDto
            {
                Id = s.Id,
                Name = s.Name,
                Alias = s.Alias,
                Price = s.Price,
                Category = s.Category,
                Image = ProductImage.CssClass(s.Name, s.Category),
                Description = $"{s.Name} chính hãng, thiết kế thời trang, chất lượng cao."
            });
        }

        _nextId = _products.Max(p => p.Id) + 1;
    }
}
