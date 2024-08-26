using System;

public class BulletPickup : IPickupable
{
    private int _bulletAmount;

    public event Action<string> OnMessageShown;

    public BulletPickup(int amount)
    {
        _bulletAmount = amount;
    }

    public void ApplyEffect(Player player)
    {
        player.AddAmmo(_bulletAmount);
        AudioManager.Instance.Play(SoundType.BulletPickup);
    }

    public bool CanPickUp(Player player)
    {
        return true;
    }
}
