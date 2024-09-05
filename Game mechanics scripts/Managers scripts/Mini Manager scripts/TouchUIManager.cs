using UnityEngine;

public class TouchUIManager : MonoBehaviour
{
    public GameObject uiElement; // Assign your UI element in the Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false); // Ensure the UI element is initially inactive
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    // If the raycast hits an object, activate the UI
                    uiElement.SetActive(true);
                }
            }
            else
            {
                // If the raycast doesn't hit an object, deactivate the UI
                uiElement.SetActive(false);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // Deactivate the UI if the touch ends or is canceled
                uiElement.SetActive(false);
            }
        }
        else
        {
            // Deactivate the UI if there are no touches
            uiElement.SetActive(false);
        }
    }
}
