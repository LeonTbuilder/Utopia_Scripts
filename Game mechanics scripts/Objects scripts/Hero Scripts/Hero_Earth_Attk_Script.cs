using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;

public class Hero_Earth_Attk_Script : MonoBehaviour, I_HeroAttack
{
    float speed = 1.5f;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public GameObject Earth_Teleport_inEffect;
    public GameObject Earth_Teleport_outEffect;

    public List<GameObject> trails;

    private bool collided;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private AudioSource audioSource;
    private Collider attackCollider;

    private Transform heroTransform;
    private Vector3 originalScale;
    private Rigidbody heroRigidbody;
    private Animator heroAnimator;

    private Hero_Script heroScript;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackCollider = GetComponent<Collider>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // Instantiate muzzle effect
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(muzzleVFX, ps.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        // Play attack sound
        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.EERTH_ATTACK, audioSource);
    }

    void FixedUpdate()
    {
        if (speed != 0 && rb != null)
        {
            rb.position += moveDirection * (speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collided)
        {
            collided = true;
            Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.HIT_BLAST, audioSource);

            if (trails.Count > 0)
            {
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].transform.parent = null;
                    var ps = trails[i].GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Stop();
                        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                }
            }

            speed = 0;
            rb.isKinematic = true;

            // Set the attack's collider to trigger mode
            if (attackCollider != null)
            {
                attackCollider.isTrigger = true;
            }

            // Align the x position of the attack object with the hit object
            Vector3 newPosition = transform.position;
            newPosition.x = collision.transform.position.x;
            transform.position = newPosition;

            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity) as GameObject;
                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
                else
                {
                    Destroy(hitVFX, ps.main.duration);
                }
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            Debug.Log("Collision Detected with " + collision.gameObject.name);

            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Mov_Trap"))
            {
                GameObject hero = GameObject.FindGameObjectWithTag("Hero");
                if (hero == null)
                {
                    Debug.LogError("Hero not found!");
                    Destroy(gameObject);  // Ensure the attack object is destroyed even if the hero is not found
                    return;
                }

                heroScript = hero.GetComponent<Hero_Script>();
                if (heroScript == null)
                {
                    Debug.LogError("Hero_Script not found on Hero!");
                    Destroy(gameObject);  // Ensure the attack object is destroyed if Hero_Script is not found
                    return;
                }

                heroTransform = hero.transform;
                heroRigidbody = hero.GetComponent<Rigidbody>();
                heroAnimator = hero.GetComponent<Animator>();

                if (heroTransform == null || heroRigidbody == null || heroAnimator == null)
                {
                    Debug.LogError("One of the Hero components is missing!");
                    Destroy(gameObject);  // Ensure the attack object is destroyed if any component is missing
                    return;
                }

                originalScale = heroTransform.localScale;

                Vector3 targetPosition = collision.contacts[0].point;
                if (collision.gameObject.CompareTag("Mov_Trap"))
                {
                    float offsetX = heroTransform.forward.x > 0 ? 1f : -1f;
                    targetPosition += new Vector3(offsetX, 0, 0);
                }

                StartCoroutine(ShrinkHeroAndTeleport(targetPosition));
            }
            else
            {
                StartCoroutine(DestroyParticle(0f));
            }
        }
    }

    private IEnumerator ShrinkHeroAndTeleport(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float shrinkDuration = 1f;
        Vector3 endScale = originalScale * 0.1f;

        heroScript.Movement_Stop();

        heroAnimator.SetBool("isRunning", false);
        heroAnimator.SetBool("isWalking", false);
        heroAnimator.SetBool("isFalling", true);

        if (Earth_Teleport_inEffect != null)
        {
            Instantiate(Earth_Teleport_inEffect, heroTransform.position, Quaternion.identity);
        }

        while (elapsedTime < shrinkDuration)
        {
            heroTransform.localScale = Vector3.Lerp(originalScale, endScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heroTransform.localScale = endScale;
        heroTransform.position = targetPosition;

        StartCoroutine(RegrowHero());
    }

    private IEnumerator RegrowHero()
    {
        float elapsedTime = 0f;
        float regrowDuration = 1f;
        Vector3 startScale = heroTransform.localScale;

        if (Earth_Teleport_outEffect != null)
        {
            Instantiate(Earth_Teleport_outEffect, heroTransform.position, Quaternion.identity);
        }

        while (elapsedTime < regrowDuration)
        {
            heroTransform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / regrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heroTransform.localScale = originalScale;

        heroScript.Movement_Resume();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(DestroyParticle(0f));  // Ensure the attack object is destroyed after the teleportation sequence
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
