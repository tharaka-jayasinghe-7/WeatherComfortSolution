namespace Weather.Domain.Entities;

public class City
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Country { get; init; } = default!;
}