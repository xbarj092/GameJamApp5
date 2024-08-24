public class BulletPickup : IPickupable
{
    private int _bulletAmount;

    public BulletPickup(int amount)
    {
        _bulletAmount = amount;
    }

    public void ApplyEffect(Player player)
    {
        player.AddAmmo(_bulletAmount);
        AudioManager.Instance.Play(SoundType.BulletPickup);
    }
}
