using UnityEngine;
using System.Collections;
using Assets.Scripts.Utility;

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

        private float healEffectTimer;

        private static readonly float DAMAGE_COOLDOWN = 0.25f;

        private static readonly float HEAL_COOLDOWN = 0.25f;

        private Coroutine damageEffectRoutine;

        private Coroutine healEffectRoutine;

        [SerializeField]
        private Color damageColor;

        [SerializeField]
        private Color healingColor;

        [SerializeField]
        private GameObject damageParticles;

        [SerializeField]
        private GameObject healingParticles;

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
            healingParticles.SetActive(false);
            health.OnDamaged -= OnDamaged;
            health.OnDestroyed -= OnDestroyed;
            health.OnHealed -= OnHealed;
        }

        private void OnDamaged(float newHealth)
        {
           // SetScaleFromHealth(newHealth);

            damageEffectTimer = 0;

            if (healEffectRoutine != null)
            {
                StopCoroutine(healEffectRoutine);
                healingParticles.SetActive(false);
            }

            if(damageEffectRoutine == null)
            {
                damageEffectRoutine = StartCoroutine(DamageEffect());
            }
            
        }

        private void OnDestroyed(GameObject obj)
        {
            health.OnDestroyed -= OnDestroyed;
            GameObject particles = ObjectPool.Instance.GetObjectForType("AgentExplosion");
            particles.transform.position = transform.position;
            GetComponent<ChaosAgentDying>().enabled = true;
            GetComponent<ChaosAgentSeeking>().enabled = false;
        }

        private void OnHealed(float newHealth)
        {
            SetScaleFromHealth(newHealth);

            healEffectTimer = 0;

            if (damageEffectRoutine != null)
            {
                StopCoroutine(damageEffectRoutine);
                damageParticles.SetActive(false);
            }

            if (healEffectRoutine == null)
            {
                healEffectRoutine = StartCoroutine(HealEffect());
            }
        }

        private void SetScaleFromHealth(float health)
        {
            float scale = Mathf.Min(health + 25, 100f) / 100;
            transform.localScale = Vector3.one * scale;
        }

        private IEnumerator DamageEffect()
        {
            sprite.color = damageColor;
            //damageParticles.SetActive(true);

            while (damageEffectTimer < DAMAGE_COOLDOWN)
            {
                yield return null;
                damageEffectTimer += Time.deltaTime;
            }

            sprite.color = Color.white;
            damageParticles.SetActive(false);
            damageEffectRoutine = null;
        }

        private IEnumerator HealEffect()
        {
            sprite.color = healingColor;
            healingParticles.SetActive(true);

            while (healEffectTimer < HEAL_COOLDOWN)
            {
                yield return null;
                healEffectTimer += Time.deltaTime;
            }

            sprite.color = Color.white;
            healingParticles.SetActive(false);
            healEffectRoutine = null;
        }
    }
}
