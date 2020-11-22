using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(HealthTracker))]
    [RequireComponent(typeof(ChaosAgentDying))]
    [RequireComponent(typeof(ChaosAgentSeeking))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ChaosAgentHealthHandler : MonoBehaviour
    {
        private HealthTracker health;

        private SpriteRenderer sprite;

        private float damageEffectTimer;

        private static readonly float DAMAGE_COOLDOWN = 0.25f;

        private static readonly float HEAL_COOLDOWN = 0.25f;

        private Coroutine damageEffectRoutine;

        private Coroutine healEffectRoutine;

        [SerializeField]
        private Color damageColor;

        [SerializeField]
        private GameObject damageParticles;

        public void OnEnable()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            health = GetComponent<HealthTracker>();
            health.OnDamaged += OnDamaged;
            health.OnDestroyed += OnDestroyed;
            health.OnHealed += OnHealed;
        }

        public void OnDisable()
        {
            damageParticles.SetActive(false);
            health.OnDamaged -= OnDamaged;
            health.OnDestroyed -= OnDestroyed;
            health.OnHealed -= OnHealed;
        }

        private void OnDamaged(float newHealth)
        {
            SetScaleFromHealth(newHealth);

            damageEffectTimer = 0;

            if (healEffectRoutine != null)
            {
                StopCoroutine(healEffectRoutine);
            }

            if(damageEffectRoutine == null)
            {
                damageEffectRoutine = StartCoroutine(damageEffect());
            }
            
        }

        private IEnumerator damageEffect()
        {
            sprite.color = damageColor;
            damageParticles.SetActive(true);

            while (damageEffectTimer < DAMAGE_COOLDOWN)
            {
                yield return null;
                damageEffectTimer += Time.deltaTime;
            }

            sprite.color = Color.white;
            damageParticles.SetActive(false);
            damageEffectRoutine = null;
        }

        private void OnDestroyed(GameObject obj)
        {
            GetComponent<ChaosAgentDying>().enabled = true;
            GetComponent<ChaosAgentSeeking>().enabled = false;
        }

        private void OnHealed(float newHealth)
        {
            SetScaleFromHealth(newHealth);
        }

        private void SetScaleFromHealth(float health)
        {
            float scale = Mathf.Min(health + 25, 100f) / 100;
            transform.localScale = Vector3.one * scale;
        }
    }
}
