using UnityEngine;
using System.Collections;

public class Camera_ShakeMotion_Script : MonoBehaviour
{
    public static Camera_ShakeMotion_Script Instance { get; private set; }

    public float shakeDuration = 0.15f; // Duration of each shake
    public float shakeMagnitude = 0.1f; // Magnitude of the shake
    public int shakeRepetitions = 3; // Number of shake repetitions
    public float speedMultiplier = 2.0f; // Speed multiplier for the shake animation

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Start_Shake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake_Object());
        }
    }

    private IEnumerator Shake_Object()
    {
        isShaking = true;

        float adjustedDuration = shakeDuration / speedMultiplier;

        for (int i = 0; i < shakeRepetitions; i++)
        {
            Vector3 randomDirection = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            float elapsed = 0.0f;

            while (elapsed < adjustedDuration)
            {
                transform.localPosition = Vector3.Lerp(originalPosition, randomDirection, (elapsed / adjustedDuration));
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0.0f;
            while (elapsed < adjustedDuration)
            {
                transform.localPosition = Vector3.Lerp(randomDirection, originalPosition, (elapsed / adjustedDuration));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}
