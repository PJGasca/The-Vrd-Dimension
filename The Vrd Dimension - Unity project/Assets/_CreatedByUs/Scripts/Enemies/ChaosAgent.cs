using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Assets.Scripts.Objects.Grabbable))]
public class ChaosAgent : MonoBehaviour
{
    [SerializeField] [Tooltip("if you hold them in beam continuously damaging them, how long until they die")]
    float timeToDeath;
    [SerializeField] [Range(0, 1)]
    float percentageAtDeath; [Tooltip("what percentage of its default size it needs to reach to count as dead")]
    Vector3 defaultScale;

    [SerializeField]
    float maxSpeed;
    bool caughtInBeam;

    Transform target;

    // Start is called before the first frame update
    void Awake()
    {
        defaultScale = transform.localScale;
        SetStartValues();
    }

    private void OnEnable()
    {
        SetStartValues();
    }

    private void OnDisable()
    {
        Assets.Scripts.Utility.ObjectPool.Instance.PoolObject(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!caughtInBeam)
        {

        }
    }

    public void InBeam(bool attract) // if false, is repelling
    {
        caughtInBeam = true;
        if (attract) // attracting damages
        {
            transform.localScale -= (defaultScale / timeToDeath) * Time.deltaTime;
            if (transform.localScale.x < defaultScale.x * percentageAtDeath)
            {
                Die();
            }
        }
        else // repelling heals
        {
            if (transform.localScale.x < defaultScale.x) // should be enough to just check for one value
            {
                transform.localScale += (defaultScale / timeToDeath) * Time.deltaTime; // healing is faster than damaging, anti-frustration feature
            }
            else if (transform.localScale.x > defaultScale.x)
            {
                transform.localScale = defaultScale;
            }
        }
    }

    void Die()
    {
        // particles, sound, etc
        gameObject.SetActive(false);
    }

    void SetStartValues()
    {
        transform.localScale = defaultScale;
        caughtInBeam = false;
        // find random merged shape from objects list, weighted towards the last shape the player merged
        // set that as the target
        // also work out what location to spawn from
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.transform == target)
        {
            Die();
            if (c.gameObject.CompareTag("EnemyAgent"))
            {

            }
        }
    }
}
