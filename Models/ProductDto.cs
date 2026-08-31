using System.ComponentModel.DataAnnotations;

namespace btb23.Models;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập alias")]
    public string Alias { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;
}
