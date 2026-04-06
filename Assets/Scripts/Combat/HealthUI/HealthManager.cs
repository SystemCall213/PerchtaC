using System.Collections.Generic;
using Combat.Interfaces;
using UnityEngine;
using Zenject;

namespace Combat.HealthUI
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private GameObject entity;
        [SerializeField] private GameObject healthKnobPrefab;
    
        private List<GameObject> healthKnobs = new List<GameObject>();
        private IHealth health;

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
            for (int i = 0; i < maxHealth; i++)
            {
                GameObject knob = Instantiate(healthKnobPrefab, transform);
                healthKnobs.Add(knob);
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
            for (int i = 0; i < damage && healthKnobs.Count > 0; i++)
            {
                GameObject knob = healthKnobs[healthKnobs.Count - 1];
                healthKnobs.RemoveAt(healthKnobs.Count - 1);
                Destroy(knob);
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
