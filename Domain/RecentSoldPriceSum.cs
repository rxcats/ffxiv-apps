namespace ffxiv_apps.Domain;

public class RecentSoldPriceSum
{
    public string YearMonth { get; init; } = null!;
    public int PriceSum { get; init; }

    public string YearMonthFormat => YearMonth.Replace("-", "/");
}
