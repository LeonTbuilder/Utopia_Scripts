using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;

public class Hero_Air_Attk_Script : MonoBehaviour, I_HeroAttack
{
    float speed = 1f;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    float pushForce = 2f;

    private bool collided;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private AudioSource audioSource;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>()?.main.duration ?? 0f);
        }

        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.AIR_ATTACK, audioSource);
    }

    void FixedUpdate()
    {
        if (!collided && rb != null)
        {
            rb.position += moveDirection * (speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collided)
        {
            collided = true;
            speed = 0;
            rb.isKinematic = true;

            HandleTrails();

            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity);
                Destroy(hitVFX, hitVFX.GetComponent<ParticleSystem>()?.main.duration ?? 0f);
            }

            Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.HIT_BLAST, audioSource);
            audioSource.Stop();

            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Mov_Trap") || collision.gameObject.CompareTag("Bulder"))
            {
                PushBack(collision.gameObject);
                PlayerPrefs.SetInt("SkillsUsed", PlayerPrefs.GetInt("SkillsUsed", 0) + 1);
            }

            StartCoroutine(DestroyParticle(0f));
        }
    }

    private void HandleTrails()
    {
        foreach (var trail in trails)
        {
            if (trail != null)
            {
                trail.transform.parent = null;
                var ps = trail.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }
    }

    private void PushBack(GameObject target)
    {
        var targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Vector3 pushDirection = target.transform.position - transform.position;
            pushDirection.y = 0;
            targetRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
