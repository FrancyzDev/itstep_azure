namespace WebApplication2.Models;

public class ItemCreateDto
{
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public IFormFile? ImageFile { get; set; }
}