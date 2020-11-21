using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Objects.Grabbable))]
    public class OrderAgent : MonoBehaviour
    {
        [Header("Health and Damage")]
        [SerializeField]
        [Tooltip("if you hold them in beam continuously damaging them, how long until they die")]
        float timeToDeath;
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("what percentage of its default size it needs to reach to count as dead")]
        float percentageAtDeath;
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("percent damage taken from enemy collision")]
        float enemyDamagePercentage;
        private Vector3 defaultScale;

        [Header("Movement")]
        [SerializeField]
        float maxSpeed;
        [SerializeField]
        float timeToMaxSpeed;
        [SerializeField]
        float directionChangeMultiplier;

        HashSet<Collider> ignoredColliders = new HashSet<Collider>();

        bool caughtInBeam;
        IEnumerator releaseFromBeam;

        Transform target;

        Rigidbody rb;
        Collider coll;

        // Start is called before the first frame update
        void Awake()
        {
            defaultScale = transform.localScale;
            gameObject.layer = LayerMask.NameToLayer("OrderAgent");
            gameObject.tag = "EnemyAgent";
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<BoxCollider>();
            SetStartValues();
        }

        private void OnEnable()
        {
            SetStartValues();
        }

        void SetStartValues()
        {
            rb.velocity = Vector3.zero;
            transform.localScale = defaultScale;
            caughtInBeam = false;

            // decide whether to target random order agent or random merged shape
            // if shape, weighted towards the last shape the player broke
            // set ^ as the target

            // also work out what location to spawn from

            if (ignoredColliders.Contains(target.GetComponent<Collider>()))
            {
                Physics.IgnoreCollision(coll, target.GetComponent<Collider>(), false);
            }
        }

        private void OnDisable()
        {
            Utility.ObjectPool.Instance.PoolObject(gameObject);
            if (releaseFromBeam != null) StopCoroutine(releaseFromBeam);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!caughtInBeam)
            {
                Movement();
            }
        }

        void Movement()
        {
            Vector3 input;
            float xInput = target.position.x < transform.position.x ? -1 : 1;
            float yInput = target.position.y < transform.position.y ? -1 : 1;
            float zInput = target.position.z < transform.position.z ? -1 : 1;
            input = new Vector3(xInput, yInput, zInput);

            float xSpeed = maxSpeed / timeToMaxSpeed * input.x * Time.fixedDeltaTime;
            float ySpeed = maxSpeed / timeToMaxSpeed * input.y * Time.fixedDeltaTime;
            float zSpeed = maxSpeed / timeToMaxSpeed * input.z * Time.fixedDeltaTime;

            // change current speed
            rb.velocity += new Vector3(
                input.x == rb.velocity.x / Mathf.Abs(rb.velocity.x) ? xSpeed : xSpeed * directionChangeMultiplier,
                input.y == rb.velocity.y / Mathf.Abs(rb.velocity.y) ? ySpeed : ySpeed * directionChangeMultiplier,
                input.z == rb.velocity.z / Mathf.Abs(rb.velocity.z) ? zSpeed : zSpeed * directionChangeMultiplier
                );

            // limit to max speed
            rb.velocity = new Vector3(
                Mathf.Abs(rb.velocity.x) > maxSpeed ? maxSpeed * input.x : rb.velocity.x,
                Mathf.Abs(rb.velocity.y) > maxSpeed ? maxSpeed * input.y : rb.velocity.y,
                Mathf.Abs(rb.velocity.z) > maxSpeed ? maxSpeed * input.z : rb.velocity.z);
        }

        public void InBeam(bool attract) // if false, is repelling
        {
            caughtInBeam = true;

            if (releaseFromBeam != null) StopCoroutine(releaseFromBeam);
            releaseFromBeam = Utility.Helpers.instance.WaitOneFrame(release => caughtInBeam = false);
            StartCoroutine(releaseFromBeam);

            if (!attract) // repelling damages
            {
                transform.localScale -= defaultScale * (1 - percentageAtDeath) / timeToDeath * Time.deltaTime;
                if (transform.localScale.x <= defaultScale.x * percentageAtDeath)
                {
                    Die(true);
                }
            }
            else // attracting damages
            {
                if (transform.localScale.x < defaultScale.x) // should be enough to just check for one value
                {
                    transform.localScale += defaultScale * (1 - percentageAtDeath) / timeToDeath * Time.deltaTime;
                }
                else if (transform.localScale.x > defaultScale.x)
                {
                    transform.localScale = defaultScale;
                }
            }
        }

        void Die(bool killedByPlayer = false)
        {
            // particles, sound, etc
            gameObject.SetActive(false);
        }

        public void TakeEnemyDamage()
        {
            transform.localScale -= defaultScale * (1 - enemyDamagePercentage);
            if (transform.localScale.x <= defaultScale.x * percentageAtDeath)
            {
                Die();
            }
        }

        private void OnCollisionEnter(Collision c)
        {// chaos agents damage whatever they collide with, even if not actual target
         // order agents pass through colliders that aren't their targets
            if (c.transform == target)
            {
                if (c.gameObject.CompareTag("EnemyAgent"))
                {
                    c.gameObject.GetComponent<ChaosAgent>().TakeEnemyDamage();
                }
                else
                {
                    // break up object
                }
                Die();
            }
            else
            {
                if (c.gameObject.CompareTag("EnemyAgent"))
                {
                    TakeEnemyDamage();
                }
                else
                {
                    ignoredColliders.Add(c.collider);
                    Physics.IgnoreCollision(coll, c.collider, true);
                }
            }
        }
    }
}
