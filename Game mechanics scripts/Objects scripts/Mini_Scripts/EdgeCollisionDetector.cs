using UnityEngine;

public class EdgeCollisionDetector : MonoBehaviour
{
    private Pin_Engine_Script pinEngineScript;

    private void Start()
    {
        
        pinEngineScript = GetComponentInParent<Pin_Engine_Script>();
    }

    // Notify the parent script about the collision
    private void OnTriggerEnter(Collider other)
    {

        pinEngineScript.Set_Collision_State(true);
        pinEngineScript.OnEdge_Collision_Enter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        
        pinEngineScript.Set_Collision_State(false);
        pinEngineScript.OnEdge_Collision_Exit(other);
    }
}
