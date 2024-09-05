using UnityEngine;

public class Always_Vertical_Script : MonoBehaviour
{
    void LateUpdate()
    {
        // Store the current position
        Vector3 currentPosition = transform.position;

        // Reset the rotation to keep the object vertical
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // Restore the position to avoid any changes due to rotation reset
        transform.position = currentPosition;
    }
}
