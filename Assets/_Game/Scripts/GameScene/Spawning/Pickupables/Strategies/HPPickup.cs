public class HpPickup : IPickupable
{
    private int _healthAmount;

    public HpPickup(int amount)
    {
        _healthAmount = amount;
    }

    public void ApplyEffect(Player player)
    {
        player.RestoreHealth(_healthAmount);
        AudioManager.Instance.Play(SoundType.HPPickup);
    }
}
