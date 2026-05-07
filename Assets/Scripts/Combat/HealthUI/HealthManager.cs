using System.Collections.Generic;
using Combat.Interfaces;
using CoreLoop.Interfaces;
using CoreLoop.States;
using UnityEngine;
using Zenject;

namespace Combat.HealthUI
{
    public enum HealthManagerType
    {
        Player,
        Boss
    }
    
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private GameObject entity;
        [SerializeField] private GameObject healthKnobPrefab;
        [SerializeField] private List<HealthPieceUI> healthPieces;
        [SerializeField] private HealthManagerType managerType = HealthManagerType.Player;
        
        private List<GameObject> healthKnobs = new List<GameObject>();
        private IHealth health;
        
        [Inject] private readonly ISceneLoader sceneLoader;
        private void Start()
        {
            if (entity == null)
            {
                Debug.LogError("Health component not assigned to HealthManager!");
                return;
            }
            health = entity.GetComponent<IHealth>();

            InitializeHealthKnobs();
            SubscribeToHealthEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromHealthEvents();
        }

        private void InitializeHealthKnobs()
        {
            int maxHealth = health.GetMaxHealth();

            // Clear any existing knobs
            ClearHealthKnobs();

            // Instantiate new knobs
            if (managerType == HealthManagerType.Boss)
            {
                for (int i = 0; i < maxHealth; i++)
                {
                    GameObject knob = Instantiate(healthKnobPrefab, transform);
                    healthKnobs.Add(knob);
                }
            }
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
            // Remove health knobs equal to damage taken
            if (managerType == HealthManagerType.Boss)
            {
                for (int i = 0; i < damage && healthKnobs.Count > 0; i++)
                {
                    GameObject knob = healthKnobs[healthKnobs.Count - 1];
                    healthKnobs.RemoveAt(healthKnobs.Count - 1);
                    Destroy(knob);
                }
            }
            else
            {
                // For player, update health pieces instead of removing knobs
                for (int i = 0; i < damage && healthPieces.Count > 0; i++)
                {
                    HealthPieceUI piece = healthPieces[healthPieces.Count - 1];
                    healthPieces.RemoveAt(healthPieces.Count - 1);
                    piece.Destroy();
                }
            }
        }

        private void OnHealthHealed(int heal)
        {
            for (int i = 0; i < heal; i++)
            {
                GameObject knob = Instantiate(healthKnobPrefab, transform);
                healthKnobs.Add(knob);
            }
        }

        private void OnHealthDeath()
        {
            ClearHealthKnobs();
            
            if (managerType == HealthManagerType.Boss)
            {
                // Boss died - return to main menu
                sceneLoader.UnloadCombatScene();
            }
            else
            {
                // Player died - just unload combat scene
                sceneLoader.LoadMainMenu();
            }
        }

        private void ClearHealthKnobs()
        {
            foreach (GameObject knob in healthKnobs)
            {
                Destroy(knob);
            }
            healthKnobs.Clear();
        }
    }
}
