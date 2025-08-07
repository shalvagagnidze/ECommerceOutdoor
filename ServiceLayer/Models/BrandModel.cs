namespace ServiceLayer.Models;

public class BrandModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Origin { get; set; }
    public string? Description { get; set; }
    public List<string>? Images { get; set; }
}
