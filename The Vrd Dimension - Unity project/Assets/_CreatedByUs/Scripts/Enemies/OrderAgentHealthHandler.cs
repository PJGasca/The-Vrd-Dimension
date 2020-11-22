using UnityEngine;
using System.Collections;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(HealthTracker))]
    public class OrderAgentHealthHandler : MonoBehaviour
    {
        private HealthTracker health;

        private float damageEffectTimer;

        //private float healEffectTimer;

        private static readonly float DAMAGE_COOLDOWN = 0.25f;

        //private static readonly float HEAL_COOLDOWN = 0.25f;

        private Coroutine damageEffectRoutine;

        //private Coroutine healEffectRoutine;

        //[SerializeField]
        //private GameObject damageParticles;

        //[SerializeField]
        //private GameObject healingParticles;

        public void OnEnable()
        {
            health = GetComponent<HealthTracker>();
            //health.OnDamaged += OnDamaged;
            health.OnDestroyed += OnDestroyed;
            //health.OnHealed += OnHealed;
        }

        public void OnDisable()
        {
            //damageParticles.SetActive(false);
            //healingParticles.SetActive(false);
            //health.OnDamaged -= OnDamaged;
            health.OnDestroyed -= OnDestroyed;
            //health.OnHealed -= OnHealed;
        }

     /*   private void OnDamaged(float newHealth)
        {
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
            
        }*/

        private void OnDestroyed(GameObject obj)
        {
            health.OnDestroyed -= OnDestroyed;
            GameObject particles = ObjectPool.Instance.GetObjectForType("AgentExplosion");
            particles.transform.position = transform.position;
            GetComponent<OrderAgentDying>().enabled = true;
            GetComponent<OrderAgentSeeking>().enabled = false;
            GetComponent<OrderAgentReturning>().enabled = false;
        }


        /*private IEnumerator DamageEffect()
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
        }*/
    }
}
