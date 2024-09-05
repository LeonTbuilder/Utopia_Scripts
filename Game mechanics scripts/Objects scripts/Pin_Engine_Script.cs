using SmallHedge.SoundManager;
using UnityEngine;

public class Pin_Engine_Script : MonoBehaviour
{
    [Header("Change Var")]
    public float speed = 10f;

    [Header("Bools")]
    private bool isColliding = false;
    private bool isHintActive = false;

    [Header("WayPoint List")]
    public Vector3[] waypoints;

    [Header("Drag Objects")]
    public Transform _to, _from, _edge;

    [Header("Debug")]
    public bool isEdgeColliding = false;

    const float Max_Swipe_Time = 0.5f;
    const float Min_Swipe_Dis = 0.17f;

    private Vector3 dragDirection;
    private bool shouldMove;

    private Vector2 start_Pos;
    private float startTime;

    private RaycastHit hit;
    private Camera raycastCamera;

    private Vector3 initialPosition;
    private float movedDistance = 0f;

    private Collider edgeCollider;
    private Collider[] colliders_List;
    private Animator animator;

    private Vector3 moveDirection;

    Hero_Life_Manager_Script heroLifeManager;

    private void Start()
    {
        edgeCollider = _edge.GetComponent<Collider>();

        colliders_List = GetComponentsInChildren<Collider>();

        heroLifeManager = FindFirstObjectByType<Hero_Life_Manager_Script>();

        WayPoint_Calculate();

        raycastCamera = Camera.main;

        initialPosition = transform.position;
        animator = GetComponent<Animator>();

        Calculate_Move_Direction();
    }

    void WayPoint_Calculate()
    {
        waypoints = new Vector3[2];
        waypoints[0] = transform.position;

        Vector2 vector_Dir_Normalize = (_to.position - _from.position).normalized;
        float angle = Mathf.Atan(vector_Dir_Normalize.y / vector_Dir_Normalize.x);

        waypoints[1] = new Vector3(
            transform.position.x + Mathf.Sign(vector_Dir_Normalize.x) * Mathf.Cos(Mathf.Abs(angle)) * 10.0f,
            transform.position.y + Mathf.Sign(vector_Dir_Normalize.y) * Mathf.Sin(Mathf.Abs(angle)) * 10.0f,
            transform.position.z
        );
    }

    void Calculate_Move_Direction()
    {
        moveDirection = (_to.position - _from.position).normalized;
    }

    void Update()
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin)
            return;

        Drag_Handle();
        if (shouldMove && !isColliding)
        {
            Move_Object();
        }

        Destroy_Pin();
    }

    void Drag_Handle()
    {
        if (Input.GetMouseButtonDown(0) && !isColliding)
        {
            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Drag_Layer")))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    start_Pos = Input.mousePosition;
                    startTime = Time.time;

                    foreach (Transform child in transform)
                    {
                        if (child.CompareTag("Hand"))
                        {
                            child.gameObject.SetActive(false);
                            isHintActive = false;
                        }
                    }
                }
            }
        }
        

        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - startTime <= Max_Swipe_Time)
            {
                Vector2 endPos = Input.mousePosition;
                Vector2 swipe = endPos - start_Pos;

                if (swipe.magnitude >= Min_Swipe_Dis)
                {
                    if (Mathf.Abs(swipe.y) > Mathf.Abs(swipe.x))
                    {
                        dragDirection = Vector3.up;
                        shouldMove = true;
                    }
                    else
                    {
                        dragDirection = Vector3.left;
                        shouldMove = true;
                    }

                    if (shouldMove && edgeCollider != null)
                    {
                        edgeCollider.enabled = false;
                    }

                    Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Pin_Swoosh);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && isEdgeColliding)
        {
            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 2f);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Drag_Layer")))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    animator.Play("Pin_Shake_Anim");
                    heroLifeManager.LowerLife();
                    Camera_ShakeMotion_Script.Instance.Start_Shake();
                }
            }
        }
    }

    void Move_Object()
    {
        Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime;
        movedDistance += Vector3.Distance(transform.position, newPosition);


        if (Vector3.Distance(newPosition, waypoints[1]) <= 0.1f)
        {
            foreach (Collider collider in colliders_List)
            {
                collider.isTrigger = true;
            }

            shouldMove = false;

            transform.position = waypoints[1];
        }
        else
        {
            transform.position = newPosition;
        }
    }


    public void Set_Collision_State(bool state)
    {
        isColliding = state;
        if (isColliding)
        {
            shouldMove = false;
        }
    }

    public void OnEdge_Collision_Enter(Collider other)
    {
        isEdgeColliding = true;
        Set_Collision_State(true);
    }

    public void OnEdge_Collision_Exit(Collider other)
    {
        isEdgeColliding = false;
        Set_Collision_State(false);
    }

    public void ActivateHint(GameObject hintHand)
    {
        isHintActive = true;
        hintHand.SetActive(true);
    }

    void Destroy_Pin()
    {
        if (movedDistance > 5f && !isHintActive)
        {
            int pinsRemoved = PlayerPrefs.GetInt("PinsRemoved", 0);
            PlayerPrefs.SetInt("PinsRemoved", pinsRemoved + 1);

            Destroy(gameObject);
        }
    }

}
