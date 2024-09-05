using UnityEngine;

public class Point_Changer : MonoBehaviour
{
    private Hero_Script heroScript;
    private Vector3 initialLocalPosition;

    void Start()
    {
        heroScript = GetComponentInParent<Hero_Script>();
        if (heroScript != null)
        {
            initialLocalPosition = transform.localPosition;
        }

    }

    void Update()
    {
        if (heroScript != null)
        {
            if (heroScript.facing_Right)
            {
                transform.localPosition = initialLocalPosition;
            }
            else
            {
                transform.localPosition = new Vector3(-initialLocalPosition.x, initialLocalPosition.y, initialLocalPosition.z);
            }
        }
    }
}
