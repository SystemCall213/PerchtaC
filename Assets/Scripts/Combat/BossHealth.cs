using System;
using Combat.Interfaces;
using Glyph;
using UnityEngine;
using Zenject;

public class BossHealth : MonoBehaviour, IHealth
{
    [Inject] private readonly GlyphFacade glyphFacade;
    [SerializeField] private int maxHealth;
    private int health;
    public event Action<int> OnDamage;
    public event Action<int> OnHeal;
    public event Action OnDeath;

    private void OnEnable()
    {
        glyphFacade.OnGlyphPainted += TakeDamage;
    }

    private void OnDisable()
    {
        glyphFacade.OnGlyphPainted -= TakeDamage;
    }

    private void Awake()
    {
        health = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;

        OnDamage?.Invoke(damage);

        if (IsDead())
        {
            Die();
        }
    }

    public void Heal(int heal)
    {
        throw new NotImplementedException();
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
