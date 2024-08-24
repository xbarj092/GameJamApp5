public class ShieldPickup : IPickupable
{
    public void ApplyEffect(Player player)
    {
        player.IncreaseShield();
        AudioManager.Instance.Play(SoundType.ShieldPickup);
    }
}
