using UnityEngine;

public class Destroy_Object_Script : MonoBehaviour
{

    public float destroy_Time = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroy_Time);
    }


}
