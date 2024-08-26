using System;

public interface IPickupable
{
    void ApplyEffect(Player player);
    bool CanPickUp(Player player);
    event Action<string> OnMessageShown;
}
