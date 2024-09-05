using SmallHedge.SoundManager;
using UnityEngine;

public class Water_Particles_Script : MonoBehaviour
{
    public GameObject smoke_PF;
    public GameObject poison_PF;
    public GameObject smoke_Gas_PF;

    public delegate void WaterEventHandler(GameObject water);
    public event WaterEventHandler OnPoisonCollision;
    public event WaterEventHandler OnMovement;
    private AudioSource steamAudioSource;

    private Rigidbody rb;
    private Poison_Spawner_Script poisonSpawner; // Add this line

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetState();
        poisonSpawner = FindFirstObjectByType<Poison_Spawner_Script>(); // Initialize the reference

        // Initialize or safely check for the AudioSource component
        if (!TryGetComponent(out steamAudioSource))
        {
            steamAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void SetState()
    {
        rb.useGravity = true; // To simulate Water density
        rb.linearVelocity = Vector3.zero;   // Reset the particle velocity
    }

    void Update()
    {
        Movement_Animation();
        Check_For_Movement();
    }

    void Movement_Animation()
    {
        Vector3 movementScale = new Vector3(1.0f, 1.0f, 1.0f);
        movementScale.x += Mathf.Abs(rb.linearVelocity.x) / 15.0f;
        movementScale.z += Mathf.Abs(rb.linearVelocity.y) / 15.0f;
        movementScale.y = 1.0f;
    }

    void Check_For_Movement()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            OnMovement?.Invoke(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Poison"))
        {
            OnPoisonCollision?.Invoke(gameObject);
            Replace_With_Poison();
            GameObject smokePF = Instantiate(smoke_Gas_PF, transform.position, transform.rotation);
            Destroy(smokePF, 1f);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Lava") || other.gameObject.CompareTag("Lava_Rock") || other.gameObject.CompareTag("WildFire"))
        {
            GameObject smokePF = Instantiate(smoke_PF, transform.position, transform.rotation);
            Destroy(smokePF, 1f);
            Destroy(gameObject);
        }
    }

    void Replace_With_Poison()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parentTransform = transform.parent;

        GameObject poison = Instantiate(poison_PF, position, rotation);

        if (parentTransform != null)
        {
            poison.transform.SetParent(parentTransform, true);
        }

        poisonSpawner?.AddPoisonParticle(poison); // Ensure poisonSpawner is not null
        Destroy(gameObject);
    }

    // Additional function in Water_Spawner_Script to prevent the exception
    public void Play_Moving_WaterSound()
    {
        if (steamAudioSource != null && !steamAudioSource.isPlaying)
        {
            Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Steam, steamAudioSource);
        }
    }
}
