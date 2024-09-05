using UnityEngine;
using System.Collections;

public class Air_Defence_Sphere_Script : MonoBehaviour
{
    public float pushBackForce = 2f;
    private Hero_Script heroScript;

    private void Awake()
    {
        heroScript = GetComponentInParent<Hero_Script>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                Vector3 pushDirection = other.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection.Normalize();

                enemyRigidbody.AddForce(pushDirection * pushBackForce, ForceMode.Impulse);

                Enemy_Script enemyScript = other.GetComponent<Enemy_Script>();
                if (enemyScript != null)
                {
                    enemyScript.SetPushedBack(true);
                    StartCoroutine(ResetPushedBackAndResume(enemyScript, 1.0f));
                }

                heroScript?.Movement_Stop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        heroScript?.Movement_Resume();
    }

    private IEnumerator ResetPushedBackAndResume(Enemy_Script enemyScript, float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyScript.SetPushedBack(false);
        heroScript?.Movement_Resume();
    }
}
