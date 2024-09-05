using UnityEngine;

public class Lava_Particles_Script : MonoBehaviour
{
    [Header("Drag Prefab")]
    public GameObject lava_RockPF;
    public GameObject Gas_PF;
    public int Spawner_ID;  
    private Poison_Spawner_Script poison_Spawner;

    public delegate void LavaTransformHandler(GameObject lava);
    public event LavaTransformHandler OnLavaRockTransform;
    public event LavaTransformHandler OnGasTransform;
    public event LavaTransformHandler OnWaterCollision;
    public event LavaTransformHandler OnPoisonCollision;

    void Awake()
    {
        SetState();
    }

    public void SetState()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; 
            rb.linearVelocity = Vector3.zero; 
        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing!");
        }
    }

    void Update()
    {
        MovementAnimation();
    }

    void MovementAnimation()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 movementScale = new Vector3(1.0f, 1.0f, 1.0f);
            movementScale.x += Mathf.Abs(rb.linearVelocity.x) / 30.0f;
            movementScale.z += Mathf.Abs(rb.linearVelocity.z) / 30.0f;
            movementScale.y = 1.0f;

        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing during movement animation.");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Poison"))
        {
            OnPoisonCollision?.Invoke(gameObject);
            Replace_To_Gas();

            if (poison_Spawner != null)
            {
                poison_Spawner.HandleLavaCollision(gameObject);
            }
            else
            {
                Debug.LogWarning("poison_Spawner is not assigned.");
            }
        }

        if (other.gameObject.CompareTag("Water"))
        {
            OnWaterCollision?.Invoke(gameObject);
            Replace_To_LavaRock();
        }

        if (other.gameObject.CompareTag("Lava_Rock"))
        {
            Lava_Particles_Script otherLavaRockScript = other.gameObject.GetComponent<Lava_Particles_Script>();

            if (otherLavaRockScript != null && otherLavaRockScript.Spawner_ID != Spawner_ID)
            {
                return;
            }

            Replace_To_LavaRock();
        }

        if (other.gameObject.CompareTag("Gem"))
        {
            Destroy(other.gameObject);
        }
    }

    public void Replace_To_LavaRock()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parentTransform = transform.parent;

        GameObject lavaRock = Instantiate(lava_RockPF, position, rotation);


        if (parentTransform != null)
        {
            lavaRock.transform.SetParent(parentTransform, true);
        }

        OnLavaRockTransform?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public void Replace_To_Gas()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parentTransform = transform.parent;

        GameObject gas = Instantiate(Gas_PF, position, rotation);

        if (parentTransform != null)
        {
            gas.transform.SetParent(parentTransform, true);
        }

        OnGasTransform?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
