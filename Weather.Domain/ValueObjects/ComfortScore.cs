namespace Weather.Domain.ValueObjects;

public class ComfortScore
{
    public double Value { get; init; }
    public string Description { get; init; } = default!;
}