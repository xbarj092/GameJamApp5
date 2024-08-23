using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    public float CurrHealth { get { return MaxHealth - CurrDamage; } }
    public float CurrDamage { get; private set; }
    public float MaxHealth { get; private set; }

    [field: SerializeField] public UnityEvent<float> OnHealthChange { get; private set; }
    [field: SerializeField] public UnityEvent<float> OnMaxHealthChange { get; private set; }
    [field: SerializeField] public UnityEvent<bool> OnDeath { get; private set; }

    private void Awake() {
        /*OnHealthChange = new UnityEvent<float>();
        OnMaxHealthChange = new UnityEvent<float>();
        OnDeath = new UnityEvent();*/
    }

    public void ResetHealth(float amout) {
        CurrDamage -= amout;
        if(CurrDamage < 0) {
            CurrDamage = 0;
        }

        OnHealthChange.Invoke(CurrDamage);
    }

    public void DealDamage(float amout) {
        if(amout <= 0)
            return;
        CurrDamage += amout;

        OnHealthChange.Invoke(CurrDamage);

        if(CurrHealth <= 0) {
            OnDeath.Invoke(false);
        }
    }

    public void SetMaxHealth(float newMaxHp) {
        MaxHealth = newMaxHp;
        CurrDamage = 0;
    }
}
