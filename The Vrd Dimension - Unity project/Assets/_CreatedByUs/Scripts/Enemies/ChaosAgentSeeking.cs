using Assets.Scripts.Game;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(AccelerateTowardsPoint))]
    [RequireComponent(typeof(Collider))]
    public class ChaosAgentSeeking : MonoBehaviour
    {
        private Rigidbody rb;
        private AccelerateTowardsPoint mover;
        public GameObject target = null;
        private Grabbable targetGrabbable = null;
        private ChaosAgentDying dyingBehaviour;
        private AgentScaler scaler;
        private AudioSource audioSource;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mover = GetComponent<AccelerateTowardsPoint>();
            dyingBehaviour = GetComponent<ChaosAgentDying>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            targetGrabbable = null;
            target = null;
            GetComponent<Collider>().enabled = true;
        }

        public void OnDisable()
        {
            if(target!=null)
            {
                target.GetComponent<MergableObject>().targetedByAgent = false;
            }
        }

        public void FixedUpdate()
        {
            if(target == null || !target.activeSelf || targetGrabbable.IsGrabbed)
            {
                PickNewTarget();
            }
            else
            {
                mover.targetPoint = target.transform.position;
            }
        }

        private void PickNewTarget()
        {
            MergableObject[] tetras = MergableObject.All;

            if (target!=null)
            {
                target.GetComponent<MergableObject>().targetedByAgent = false;
            }

            MergableObject tetra = GameManager.Instance.GetBreakableTetra();
            if (tetra != null)
            {
                target = tetra.gameObject;
                mover.targetPoint = target.transform.position;
                mover.enabled = true;
                targetGrabbable = target.GetComponent<Grabbable>();
                tetra.targetedByAgent = true;
            }
            else
            {
                Debug.Log("Chaos agent could not find valid target. Dying.");
                target  = null;
                mover.enabled = false;
                Die();
            }
        }

        private void Die()
        {
            this.enabled = false;
            dyingBehaviour.enabled = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            MergableObject tetra = collision.gameObject.GetComponent<MergableObject>();
            if(tetra!=null && tetra.gameObject.GetComponent<ObjectSize>().Size>1)
            {
                tetra.Split();
                audioSource.PlayOneShot(Utility.SoundEffectClips.instance.chaosBreak);
                Debug.Log("Chaos agent collided.");
                Die();
            }
        }
    }
}
