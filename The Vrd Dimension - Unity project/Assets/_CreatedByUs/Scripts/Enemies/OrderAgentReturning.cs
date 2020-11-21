using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(MoveTowardsPoint))]
    public class OrderAgentReturning : MonoBehaviour
    {
        private Rigidbody rb;
        private MoveTowardsPoint mover;

        private Vector3 defaultScale;

        private GameObject target = null;

        [SerializeField]
        private float targetObjectGrabRange;

        private Grabbable targetGrabbable;

        // Start is called before the first frame update
        void Awake()
        {
            defaultScale = transform.localScale;
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<MoveTowardsPoint>();
        }

        private void OnEnable()
        {
            // Put a bit of spin on it
            rb.AddRelativeTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
        }

        public void FixedUpdate()
        {
            if(target == null || !target.activeSelf || targetGrabbable.IsGrabbed)
            {
                PickNewTarget();
            }
            else if(Vector3.Distance(transform.position, target.transform.position) < targetObjectGrabRange)
            {
                Grabbable grabbable = target.GetComponent<Grabbable>();
                if (!grabbable.IsGrabbed)
                {
                    target.GetComponent<Grabbable>().Grab(transform);
                }
            }
        }

        private void PickNewTarget()
        {
            Tetrahedron[] tetras = Tetrahedron.All;
            foreach(Tetrahedron tetra in tetras)
            {
                if(tetra.IsDisplaced())
                {
                    target = tetra.gameObject;
                    mover.targetPoint = target.transform.position;
                    mover.enabled = true;
                    targetGrabbable = target.GetComponent<Grabbable>();
                    break;
                }
            }

            // Nothing to pick? My work here is done!
            if (target == null)
            {
                mover.enabled = false;
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Dieing");
        }


        /*
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
           }*/
    }
}
