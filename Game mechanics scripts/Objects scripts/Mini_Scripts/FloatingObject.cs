using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.1f; // The height of the floating motion
    public float frequency = 1f; // The speed of the floating motion

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Update the object's position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
