using UnityEngine;
using System.Collections;

public class Root_ATK_Enemy_Script : MonoBehaviour
{
    public GameObject Root_Prefab;
    public float detect_Range = 2.4f;
    public float attack_Delay = 0.5f;
    public float attack_Duration = 2.2f;
    public float reload_Time = 5f;

    private GameObject _targetHero;
    private bool isAttacking = false;
    private Animator enemy_Animator;
    private LayerMask obstacleLayer;
    private LayerMask heroLayer;
    public LayerMask ignoredLayer;

    void Start()
    {

        enemy_Animator = GetComponent<Animator>();

        var enemyScript = GetComponent<Enemy_Script>();
        if (enemyScript != null)
        {

            obstacleLayer = enemyScript.obstacle_Layer;
            heroLayer = enemyScript.heroLayerMask;
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            Detect_toAttack_Hero();
        }
    }

    private void Detect_toAttack_Hero()
    {
        Vector3 direction = transform.forward;
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0); // Raise the origin by 0.5 units on the Y-axis
        RaycastHit hit;

        Debug.DrawLine(origin, origin + direction * detect_Range, Color.magenta);

        if (Physics.Raycast(origin, direction, out hit, detect_Range, heroLayer))
        {
            if (hit.collider.CompareTag("Hero") && !Is_Obstacle_InWay(hit))
            {
                _targetHero = hit.collider.gameObject;
                StartCoroutine(Attack_Delay());
            }
        }
    }

    private bool Is_Obstacle_InWay(RaycastHit hit)
    {
        Vector3 directionToHero = (hit.point - transform.position).normalized;
        float distanceToHero = Vector3.Distance(transform.position, hit.point);

        RaycastHit obstacleHit;

        // Perform a raycast but ignore layers specified in the ignoredLayer mask
        if (Physics.Raycast(transform.position, directionToHero, out obstacleHit, distanceToHero, obstacleLayer))
        {
            // Check if the obstacle is not in the ignored layers
            if (((1 << obstacleHit.collider.gameObject.layer) & ignoredLayer) != 0)
            {
                return false; // Ignore this obstacle
            }
            return true; // Otherwise, there is an obstacle in the way
        }
        return false;
    }

    private IEnumerator Attack_Delay()
    {
        isAttacking = true;

        if (enemy_Animator != null)

            enemy_Animator.SetBool("isAttacking", true);

        else
        {
            enemy_Animator.SetBool("isAttacking", false);
            Debug.LogError("Enemy Animator is not assigned.");

        }

        yield return new WaitForSeconds(attack_Delay);
        if (_targetHero != null && Root_Prefab != null)
        {
            GameObject attackInstance = Instantiate(Root_Prefab, _targetHero.transform.position, Quaternion.identity);

            Vector3 attackPosition = attackInstance.transform.position;
            attackPosition.z = 9.6f;
            attackInstance.transform.position = attackPosition;

            attackInstance.transform.SetParent(transform.parent, worldPositionStays: true);

            yield return new WaitForSeconds(attack_Duration);
        }


        yield return new WaitForSeconds(reload_Time);
        isAttacking = false;
    }
}
