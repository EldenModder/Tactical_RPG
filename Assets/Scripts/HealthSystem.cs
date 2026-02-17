using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDeath;
    public event EventHandler OnDamaged;
    public int Health { get; private set; }
    [SerializeField] private int maxHealth = 100;
    
    void Awake()
    {
        Health = maxHealth;
    }


    public void Damage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
        OnDamaged?.Invoke(this, EventArgs.Empty);
        if (Health <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetNormalizedHealth() => (float)Health / maxHealth;
}
