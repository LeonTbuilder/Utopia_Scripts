using UnityEngine;

public class Burn_Obstacle : MonoBehaviour
{
    private Burn_Effect_Script[] burnEffects;
    private Collider objectCollider;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        burnEffects = GetComponentsInChildren<Burn_Effect_Script>();
        objectCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fire_ATK"))
        {
            objectCollider.enabled = false;

            foreach (var burnEffect in burnEffects)
            {
                burnEffect.StartBurning();
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Air_ATK"))
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }

    }

}
