using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable/Enemy")]
public class EntityInfo : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
}
