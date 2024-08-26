using System.Collections;
using TMPro;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private IPickupable _pickupableStrategy;
    public IPickupable PickupableStrategy => _pickupableStrategy;

    private void Update()
    {
        transform.Translate(GameManager.Instance.MovementSpeed() * Time.deltaTime * Vector3.down);
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        _pickupableStrategy.OnMessageShown -= OnMessageShown;
    }

    public void SetPickupable(IPickupable pickupable)
    {
        _pickupableStrategy = pickupable;
        _pickupableStrategy.OnMessageShown += OnMessageShown;
    }

    private void OnMessageShown(string message)
    {
        StartCoroutine(ShowText(message));
    }

    private IEnumerator ShowText(string message)
    {
        _text.gameObject.SetActive(true);
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
        _text.text = message;
        yield return new WaitForSeconds(1f);
        while (_text.color.a > 0)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - 0.005f);
            yield return null;
        }
        _text.gameObject.SetActive(false);
    }

    public void Collect(Player player)
    {
        if (_pickupableStrategy != null)
        {
            bool canPickUp = _pickupableStrategy.CanPickUp(player);
            _pickupableStrategy.ApplyEffect(player);
            if (canPickUp)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.CurrentPickupable = this;
            Collect(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.CurrentPickupable = null;
            Collect(player);
        }
    }
}
