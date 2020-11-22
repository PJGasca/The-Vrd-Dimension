using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Enemies
{
    public class HealthTracker : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth;

        private float health;

        public event System.Action<GameObject> OnHealed;

        public event System.Action<GameObject> OnDamaged;

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
                OnDamaged(gameObject);
            }

            if(health <= 0)
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
                OnHealed(gameObject);
            }
        }

    }
}

