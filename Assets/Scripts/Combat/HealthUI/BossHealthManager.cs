using System.Collections.Generic;
using Combat.Interfaces;
using CoreLoop.Interfaces;
using UnityEngine;
using Zenject;

namespace Combat.HealthUI
{
    public class BossHealthManager : MonoBehaviour
    {
        [SerializeField] private GameObject entity;
        [SerializeField] private List<BossHealthPieceUI> healthPieces;
        
        [Inject] private readonly ISceneLoader sceneLoader;
        private IHealth health;
        
        private void Start()
        {
            if (entity == null)
            {
                Debug.LogError("Health component not assigned to HealthManager!");
                return;
            }

            health = entity.GetComponent<IHealth>();
            health.SetMaxHealth(healthPieces.Count);
            SubscribeToHealthEvents();
        }
        
        private void SubscribeToHealthEvents()
        {
            health.OnDamage += OnHealthDamaged;
            health.OnHeal += OnHealthHealed;
            health.OnDeath += OnHealthDeath;
        }

        private void UnsubscribeFromHealthEvents()
        {
            if (health != null)
            {
                health.OnDamage -= OnHealthDamaged;
                health.OnHeal -= OnHealthHealed;
                health.OnDeath -= OnHealthDeath;
            }
        }

        private void OnHealthDamaged(int damage)
        {
            for (int i = 0; i < damage && healthPieces.Count > 0; i++)
            {
                BossHealthPieceUI piece = healthPieces[i];
                healthPieces.RemoveAt(i);
                piece.Destroy();
            }
        }

        private void OnHealthHealed(int obj)
        {
            throw new System.NotImplementedException();
        }

        private void OnHealthDeath()
        {
            sceneLoader.LoadNextLevel();
        }
    }
}