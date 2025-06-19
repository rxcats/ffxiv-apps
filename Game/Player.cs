namespace ffxiv_apps.Game;

public class Player
{
    private int top = WorldSettings.Floor;
    private int left = 0;
    private int forceUp = 0;
    private int forceRight = 0;
    private int direction = 1;

    public int Top => top;

    public int Left => left;

    public bool IsJumping => forceUp > 0;
    public bool IsWalking => forceRight != 0;
    public bool IsWalkingLeft => direction < 0;
    public bool IsWalkingRight => direction > 0;

    public void Jump()
    {
        if (forceUp == 0)
        {
            forceUp += WorldSettings.JumpForce;
        }
    }

    public void WalkRight() => forceRight = WorldSettings.Speed;
    public void WalkLeft() => forceRight = -WorldSettings.Speed;

    public void Stop() => forceRight = 0;

    public void Update()
    {
        forceUp -= WorldSettings.Gravity;

        top -= forceUp;

        if (top > WorldSettings.Floor)
        {
            top = WorldSettings.Floor;
            forceUp = 0;
        }

        if (left <= 0 && forceRight < 0)
        {
            forceRight = 0;
        }
        else if (forceRight != 0)
        {
            direction = forceRight;
        }

        left += forceRight;
    }
}
