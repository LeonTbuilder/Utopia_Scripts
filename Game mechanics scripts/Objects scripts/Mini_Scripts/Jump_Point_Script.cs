using UnityEngine;

public class Jump_Point_Script : MonoBehaviour
{

    private void Start()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero")) 
        {
            gameObject.SetActive(false);

        }
    }
}
