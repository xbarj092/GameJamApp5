using System;

public class ShieldPickup : IPickupable
{
    private const string MESSAGE = "Shield already active";

    public event Action<string> OnMessageShown;

    public void ApplyEffect(Player player)
    {
        if (CanPickUp(player))
        {
            player.IncreaseShield();
            AudioManager.Instance.Play(SoundType.ShieldPickup);
        }
        else
        {
            OnMessageShown?.Invoke(MESSAGE);
        }
    }

    public bool CanPickUp(Player player)
    {
        return !player.Shield;
    }
}
