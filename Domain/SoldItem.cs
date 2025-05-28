namespace ffxiv_apps.Domain;

public class SoldItem
{
    public int? Id { get; init; }
    public string? ItemName { get; init; }
    public int? ItemId { get; init; }
    public int? Quantity { get; init; }
    public int? Price { get; init; }
    public DateOnly? SoldDate { get; init; }
}
