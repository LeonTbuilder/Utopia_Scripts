using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;

public class Hero_Electric_Attk_Script : MonoBehaviour, I_HeroAttack
{
    float speed = 1.5f;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public GameObject electricEffectPrefab; // Add this line
    public List<GameObject> trails;
    public float chainRange = 3f; // Range to search for the next enemy

    private Rigidbody rb;
    private Vector3 moveDirection;
    private AudioSource audioSource;
    private List<GameObject> attackedEnemies = new List<GameObject>();

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.ELECTRIC_ATTACK, audioSource);
    }

    void FixedUpdate()
    {
        if (speed != 0 && rb != null)
        {
            rb.position += moveDirection * (speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Mov_Trap")) && !attackedEnemies.Contains(other.gameObject))
        {
            attackedEnemies.Add(other.gameObject);
            Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.HIT_BLAST, audioSource);

            // Handle trails
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

            // Instantiate hit effect
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

            // Instantiate electric effect on the collided object
            if (electricEffectPrefab != null)
            {
                var electricEffect = Instantiate(electricEffectPrefab, other.transform.position, Quaternion.identity);
                electricEffect.transform.parent = other.transform; // Attach the effect to the collided object
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Increment SkillsUsed PlayerPrefs
            int skillsUsed = PlayerPrefs.GetInt("SkillsUsed", 0);
            PlayerPrefs.SetInt("SkillsUsed", skillsUsed + 1);

            GameObject nextEnemy = FindNextEnemy(other.gameObject.transform.position);

            if (nextEnemy != null)
            {
                Vector3 targetPosition = nextEnemy.transform.position + new Vector3(0, 0.5f, 0);
                moveDirection = (targetPosition - transform.position).normalized;
                rb.isKinematic = false;
                speed = 2f;
            }
            else
            {
                StartCoroutine(DestroyParticle(0f));
            }
        }
    }


    private GameObject FindNextEnemy(Vector3 currentEnemyPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(currentEnemyPosition, chainRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy") && !attackedEnemies.Contains(collider.gameObject))
            {
                return collider.gameObject;
            }
        }
        return null;
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
