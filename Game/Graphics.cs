namespace ffxiv_apps.Game;

public class Graphics
{
    private readonly Player player;
    private int playerOffset = 0;
    private DateTime lastUpdate = DateTime.MinValue;

    public Graphics(Player player)
    {
        this.player = player;
    }

    public int PlayerOffset => playerOffset;

    public int PlayerDirection =>
        player switch
        {
            { IsWalkingLeft: true } => -1,
            { IsWalkingRight: true } => 1,
            _ => 0
        };

    public void CyclePlayer()
    {
        if (lastUpdate.AddMilliseconds(100) > DateTime.Now) return;

        if (playerOffset > -96)
        {
            playerOffset -= 48;
        }
        else
        {
            playerOffset = 0;
        }

        lastUpdate = DateTime.Now;
    }
}
