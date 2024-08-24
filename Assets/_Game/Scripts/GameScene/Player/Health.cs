using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float CurrHealth 
    { 
        get => MaxHealth - CurrDamage;
    }
    public float CurrDamage { get; private set; }
    public float MaxHealth { get; private set; }

    [field: SerializeField] public UnityEvent<float> OnHeal { get; private set; }
    [field: SerializeField] public UnityEvent<float> OnDamage { get; private set; }
    [field: SerializeField] public UnityEvent<float> OnMaxHealthChange { get; private set; }
    [field: SerializeField] public UnityEvent OnDeath { get; private set; }

    public void Heal(float amout) 
    {
        CurrDamage -= amout;
        if (CurrDamage < 0) 
        {
            CurrDamage = 0;
        }

        OnHeal.Invoke(CurrDamage);
    }

    public void DealDamage(float amout)
    {
        if (amout <= 0)
        {
            return;
        }

        CurrDamage += amout;
        OnDamage.Invoke(CurrDamage);

        if (CurrHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }

    public void SetMaxHealth(float newMaxHp)
    {
        MaxHealth = newMaxHp;
        CurrDamage = 0;
    }
}
