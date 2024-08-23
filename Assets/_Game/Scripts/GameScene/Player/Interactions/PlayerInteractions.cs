using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask _interact;
    [SerializeField] private float _maxRange = 5f;
    [SerializeField] private float _placementRadius = 0.35f;
    [SerializeField] private SpriteRenderer _interactionZone;
    public float MaxRange { get { return _maxRange; } set { _maxRange = value; _interactionZone.transform.localScale = Vector3.one * _maxRange * 2; } }

    private KeyboardInputHandler _keyboardInputHandler = new();

    private void Awake()
    {
        _interactionZone.transform.localScale = Vector3.one * _maxRange * 2;
    }

    private void Update()
    {
        _keyboardInputHandler.HandleInteraction();
    }
}
