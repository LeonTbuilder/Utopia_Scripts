using UnityEngine;

public class Gem_Script : MonoBehaviour
{
    public GameObject lava_explosion_Effect;
    public GameObject poison_explosion_Effect;
    [HideInInspector]
    public Gems_Spawner_Script spawnerScript;
    [HideInInspector]
    public Vector3 spawnerPosition; // Reference to the spawner position

    private void InstantiateExplosionEffect(GameObject explosionPrefab)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            InstantiateExplosionEffect(lava_explosion_Effect);
        }

        if (collision.gameObject.CompareTag("Poison"))
        {
            InstantiateExplosionEffect(poison_explosion_Effect);
        }

        if (collision.gameObject.CompareTag("Hero"))
        {
            if (spawnerScript != null)
                spawnerScript.MoveGemsTowardsHero(collision.transform);

        }
    }

    private void FixedUpdate()
    {
        // Apply a small force to move towards the spawner
        Vector3 directionToSpawner = (spawnerPosition - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(directionToSpawner * 4f); // Adjust the force as needed
    }
}
