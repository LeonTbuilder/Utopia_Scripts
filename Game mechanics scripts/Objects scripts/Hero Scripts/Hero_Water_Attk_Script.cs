using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;
using static UnityEngine.ParticleSystem;

public class Hero_Water_Attk_Script : MonoBehaviour, I_HeroAttack
{
    float speed = 1.5f;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    public GameObject water_PF;
    public int water_Count;
    private GameObject[] Water_List;

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
        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.WATER_ATTACK, audioSource);
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

            // Stop the sound
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Mov_Trap"))
            {
                InstantiateWaterParticles();

                // Increment SkillsUsed PlayerPrefs
                int skillsUsed = PlayerPrefs.GetInt("SkillsUsed", 0);
                PlayerPrefs.SetInt("SkillsUsed", skillsUsed + 1);
            }

            StartCoroutine(DestroyParticle(0f));
        }
    }


    private void InstantiateWaterParticles()
    {
        Water_List = new GameObject[water_Count];

        for (int i = 0; i < Water_List.Length; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f),
                                      transform.position.y + Random.Range(-0.25f, 0.25f),
                                      transform.position.z);  // Set Z position to current position's Z
            Water_List[i] = Instantiate(water_PF, pos, Quaternion.identity);
            Water_List[i].layer = gameObject.layer;
            Water_List[i].transform.SetParent(transform.parent);
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
