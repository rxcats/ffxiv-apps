namespace ffxiv_apps.Game;

public class Controls
{
    private readonly Player player;

    public Controls(Player player)
    {
        this.player = player;
    }

    public void KeyDown(string key)
    {
        switch (key)
        {
            case "KeyA":
            case "ArrowLeft":
                if (!player.IsJumping)
                {
                    player.WalkLeft();
                }

                break;

            case "Space":
            case "KeyW":
            case "ArrowUp":
                player.Jump();
                break;

            case "KeyD":
            case "ArrowRight":
                if (!player.IsJumping)
                {
                    player.WalkRight();
                }

                break;
        }
    }

    public void KeyUp(string key)
    {
        switch (key)
        {
            case "KeyA":
            case "ArrowLeft":
                player.Stop();
                break;

            case "KeyD":
            case "ArrowRight":
                player.Stop();
                break;
        }
    }
}
