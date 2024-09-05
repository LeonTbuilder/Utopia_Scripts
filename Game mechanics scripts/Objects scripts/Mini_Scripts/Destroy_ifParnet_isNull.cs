using UnityEngine;

public class Destroy_ifParent_isNull : MonoBehaviour
{
    private Enemy_Script enemyScript;
    private Hero_Script heroScript;

    void Start()
    {
        enemyScript = GetComponentInParent<Enemy_Script>();

        heroScript = GetComponentInParent<Hero_Script>();
    }

    void Update()
    {
        if (enemyScript.isDead 
            || heroScript.isDead)
        {
            Destroy(gameObject);
        }
    }
}
