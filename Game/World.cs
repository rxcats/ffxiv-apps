namespace ffxiv_apps.Game;

public class World
{
    private readonly Player player;

    public World(Player player)
    {
        this.player = player;
    }

    public void ApplyPhysics()
    {
        player.Update();
    }
}
