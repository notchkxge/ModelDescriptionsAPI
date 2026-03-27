namespace ModelDescriptionsApi.Models;

public class Asset3d
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}
