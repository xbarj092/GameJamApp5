using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    private IPickupable _pickupableStrategy;

    private void Update()
    {
        transform.Translate(GameManager.Instance.MovementSpeed() * Time.deltaTime * Vector3.down);
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    public void SetPickupable(IPickupable pickupable)
    {
        _pickupableStrategy = pickupable;
    }

    public void Collect(Player player)
    {
        if (_pickupableStrategy != null)
        {
            _pickupableStrategy.ApplyEffect(player);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Collect(player);
        }
    }
}
