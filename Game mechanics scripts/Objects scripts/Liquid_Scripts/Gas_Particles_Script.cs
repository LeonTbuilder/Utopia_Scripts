using UnityEngine;

public class Gas_Particles_Script : MonoBehaviour
{
    private const float GAS_FLOATABILITY = 2.0f;
    private Rigidbody rb;

    private float lastYPosition;
    private float timeStationary;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetState();
    }

    private void Start()
    {
        Destroy(gameObject, 3.5f); // Destroy after 3.5 seconds as a fallback
        lastYPosition = transform.position.y;
        timeStationary = 0f;
    }

    public void SetState()
    {
        rb.useGravity = false; // To simulate Gas density
        rb.linearVelocity = Vector3.zero;   // Reset the particle velocity
        gameObject.layer = LayerMask.NameToLayer("Gas_Layer");
    }

    void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        if (velocity.y < 50)
        {
            rb.AddForce(Vector3.up * GAS_FLOATABILITY, ForceMode.Acceleration); // Gas always goes upwards
        }

        CheckForStationary();
    }

    private void CheckForStationary()
    {
        float currentYPosition = transform.position.y;
        if (Mathf.Approximately(currentYPosition, lastYPosition))
        {
            timeStationary += Time.deltaTime;
            if (timeStationary >= 1f) // Destroy after 1 second of stationary
            {
                Destroy(gameObject);
            }
        }
        else
        {
            timeStationary = 0f; // Reset the timer if the position changes
        }

        lastYPosition = currentYPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle trigger events if needed
    }
}
