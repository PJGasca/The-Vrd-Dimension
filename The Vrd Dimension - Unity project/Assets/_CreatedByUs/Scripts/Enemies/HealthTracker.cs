using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Enemies
{
    public class HealthTracker : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth;

        private float health;

        public event System.Action<float> OnHealed;

        public event System.Action<float> OnDamaged;

        public event System.Action<GameObject> OnDestroyed;

        public void OnEnable()
        {
            health = maxHealth;
        }

        public void InflictDamage(float damage)
        {
            health -= damage;

            if(OnDamaged!=null)
            {
                OnDamaged(health);
            }

            if(health <= 0 && OnDestroyed != null)
            {
                OnDestroyed(gameObject);
            }
        }

        public void Heal(float healing)
        {
            health += healing;

            if(health > maxHealth)
            {
                health = maxHealth;
            }

            if (OnHealed != null)
            {
                OnHealed(health);
            }
        }

    }
}

