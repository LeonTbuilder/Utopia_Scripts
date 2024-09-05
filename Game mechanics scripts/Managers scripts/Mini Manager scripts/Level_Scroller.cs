using UnityEngine;

public class Level_Scroller : MonoBehaviour
{
    public static Level_Scroller Instance { get; private set; }

    [SerializeField]
    private float scrollSpeed = 5.0f;

    private Transform upperBound;
    private Transform lowerBound;

    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private bool isDragging;

    private float dragThreshold = 0.1f; // Minimum distance to consider it a drag
    private bool isDragMotion = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (upperBound == null || lowerBound == null)
        {
            return;
        }

        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        float scrollInput = Input.GetAxis("Vertical") + Input.GetAxis("Mouse ScrollWheel");
        HandleMouseDrag();
        HandleTouchDrag();

        if (isDragMotion && scrollInput != 0)
        {
            ScrollCamera(scrollInput * scrollSpeed * Time.deltaTime);
        }
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
            isDragMotion = false; // Reset drag motion
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isDragMotion = false; // Reset drag motion
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            if (delta.magnitude > dragThreshold)
            {
                isDragMotion = true; // Only consider as dragging if it exceeds the threshold
                ScrollCamera(-delta.y * scrollSpeed * Time.deltaTime / 10.0f);
            }
        }
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastMousePosition = touch.position;
                isDragMotion = false; // Reset drag motion
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                isDragMotion = false; // Reset drag motion
            }

            if (isDragging && touch.phase == TouchPhase.Moved)
            {
                Vector3 currentTouchPosition = touch.position;
                Vector3 delta = currentTouchPosition - lastMousePosition;
                lastMousePosition = currentTouchPosition;

                if (delta.magnitude > dragThreshold)
                {
                    isDragMotion = true; // Only consider as dragging if it exceeds the threshold
                    ScrollCamera(-delta.y * scrollSpeed * Time.deltaTime / 10.0f);
                }
            }
        }
    }

    private void ScrollCamera(float scrollAmount)
    {
        Vector3 newPosition = mainCamera.transform.position + Vector3.up * scrollAmount;
        newPosition.y = Mathf.Clamp(newPosition.y, lowerBound.position.y + 5f, upperBound.position.y - 5.0f);
        mainCamera.transform.position = newPosition;
    }

    public void SetBounds(Transform upper, Transform lower)
    {
        upperBound = upper;
        lowerBound = lower;
    }
}
