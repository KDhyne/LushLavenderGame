public class SpeedUp : Collectable
{
    public override void ApplyCollectedEffect(Player player)
    {
        player.SpeedUpLevel++;
    }
}
