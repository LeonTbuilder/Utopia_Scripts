using SmallHedge.SoundManager;
using UnityEngine;

public class Bulder_Script : MonoBehaviour
{
    private Rigidbody bulder_RB;
    private AudioSource audioSource;
    public LayerMask obstacleLayerMask;
    float detectionDistance = 0.3f;
    float movementThreshold = 0.1f;

    public bool IsMoving { get; private set; } = false;

    void Start()
    {
        bulder_RB = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        Keep_Z_Position();

        DetectObstacles();
        HandleMovementSound();
    }

    private void Keep_Z_Position()
    {
        Vector3 position = transform.position;
        if (position.z != 9f)
        {
            position.z = 9f;
            transform.position = position;
        }
    }

    private void DetectObstacles()
    {
        Vector3 detectionOriginRight = transform.position + new Vector3(detectionDistance, 0, 0); // Offset right
        Vector3 detectionOriginLeft = transform.position + new Vector3(-detectionDistance, 0, 0); // Offset left

        Vector3 rightDetectionPoint = detectionOriginRight + Vector3.right * detectionDistance;
        Vector3 leftDetectionPoint = detectionOriginLeft + Vector3.left * detectionDistance;

        Debug.DrawLine(detectionOriginRight, rightDetectionPoint, Color.red);
        Debug.DrawLine(detectionOriginLeft, leftDetectionPoint, Color.red);

        if (Physics.Linecast(detectionOriginRight, rightDetectionPoint, out RaycastHit hitRight, obstacleLayerMask) ||
            Physics.Linecast(detectionOriginLeft, leftDetectionPoint, out RaycastHit hitLeft, obstacleLayerMask))
        {
            StopMovementAndRotation();
        }
        else
        {
            bulder_RB.constraints = RigidbodyConstraints.None;
        }
    }

    private void StopMovementAndRotation()
    {
        bulder_RB.constraints = RigidbodyConstraints.FreezeRotation; // Adjust this based on your needs.
    }

    private void HandleMovementSound()
    {
        if (bulder_RB.linearVelocity.magnitude > movementThreshold) // Using linearVelocity as required
        {
            if (!audioSource.isPlaying)
            {
                Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Rock_Moving, audioSource);
                IsMoving = true;
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                IsMoving = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsMoving && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hero")))
        {
            bulder_RB.isKinematic = true; 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            bulder_RB.isKinematic = false; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 detectionOriginRight = transform.position + new Vector3(detectionDistance, 0, 0); // Offset right
        Vector3 detectionOriginLeft = transform.position + new Vector3(-detectionDistance, 0, 0); // Offset left

        Vector3 rightDetectionPoint = detectionOriginRight + Vector3.right * detectionDistance;
        Vector3 leftDetectionPoint = detectionOriginLeft + Vector3.left * detectionDistance;

        Gizmos.DrawLine(detectionOriginRight, rightDetectionPoint);
        Gizmos.DrawLine(detectionOriginLeft, leftDetectionPoint);
    }
}
