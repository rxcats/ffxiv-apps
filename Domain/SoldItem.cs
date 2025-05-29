namespace ffxiv_apps.Domain;

public class SoldItem
{
    public int? Id { get; init; }
    public string? ItemName { get; init; }
    public int? ItemId { get; init; }
    public int? ItemIcon { get; init; }
    public int? Quantity { get; init; }
    public int? Price { get; init; }
    public DateOnly? SoldDate { get; init; }

    public string ItemIconUrl
        => ItemId == 0
            ? "https://demos.blazorbootstrap.com/images/placeholder.png"
            : $"https://ff14.tar.to/assets/img/icon/{IconGroup}/{ItemIcon:D6}_hr1.png";

    string IconGroup
    {
        get
        {
            var str = $"{ItemIcon:D6}";

            char[] charArray = str.ToCharArray();

            for (int i = 3; i < charArray.Length; i++)
            {
                charArray[i] = '0';
            }

            return new string(charArray);
        }
    }
}
