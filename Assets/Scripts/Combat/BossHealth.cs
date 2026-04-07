using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Interfaces;
using UnityEngine;

public class BossHealth : MonoBehaviour, IHealth
{
    [SerializeField] private int maxHealth;
    
    public event Action<int> OnDamage;
    public event Action<int> OnHeal;
    public event Action OnDeath;
    
    public void TakeDamage(int damage)
    {
        throw new NotImplementedException();
    }

    public void Heal(int heal)
    {
        throw new NotImplementedException();
    }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public bool IsDead()
    {
        throw new NotImplementedException();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
