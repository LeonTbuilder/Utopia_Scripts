using UnityEngine;

public class Poison_Particles_Script : MonoBehaviour
{
    public GameObject smoke_Gas_PF;

    public delegate void PoisonTransformHandler(GameObject poison);
    public event PoisonTransformHandler OnPoisonTransform;

    void Awake()
    {
        SetState();
    }

    public void SetState()
    {
        GetComponent<Rigidbody>().useGravity = true; // To simulate the lava density
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Reset the particle velocity
    }

    void Update()
    {
        MovementAnimation();
    }

    void MovementAnimation()
    {
        Vector3 movementScale = new Vector3(1.0f, 1.0f, 1.0f);
        movementScale.x += Mathf.Abs(GetComponent<Rigidbody>().linearVelocity.x) / 30.0f;
        movementScale.z += Mathf.Abs(GetComponent<Rigidbody>().linearVelocity.y) / 30.0f;
        movementScale.y = 1.0f;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Lava"))
        {
            Replace_With_SmokeGas();
            OnPoisonTransform?.Invoke(gameObject);

        }


        if (other.gameObject.CompareTag("Gem"))
        {
            Destroy(other.gameObject);
        }
    }

    public void Replace_With_SmokeGas()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parentTransform = transform.parent;

        GameObject smoke_Gas = Instantiate(smoke_Gas_PF, position, rotation);

        if (parentTransform != null)
        {
            smoke_Gas.transform.SetParent(parentTransform, true);
            Destroy(smoke_Gas);
        }

        Destroy(gameObject);
    }
}
