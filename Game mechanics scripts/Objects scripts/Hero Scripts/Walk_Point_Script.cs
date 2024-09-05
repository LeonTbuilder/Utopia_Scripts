using UnityEngine;
using UnityEngine.EventSystems;

public class Walk_Point_Script : MonoBehaviour
{
    [Header("Fairy Pool Settings")]
    public GameObject fairyPrefab;
    public int poolSize = 2;
    public LayerMask pinLayerMask;
    public LayerMask uiLayerMask;

    private GameObject[] fairyPool;
    private Camera raycastCamera;
    private int currentFairyIndex = 0;
    private GameObject activeFairy;

    [Header("Hero Settings")]
    public float detectionRange = 0.2f;
    private bool isDead;
    private bool _isWin;
    Transform hero_Pos;

    void Start()
    {
        raycastCamera = Camera.main;
        hero_Pos = GameObject.FindWithTag("Hero").transform;
        InitializeFairyPool();
    }

    void Update()
    {
        if (isDead || _isWin)
        {
            ReturnFairyToPool();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, pinLayerMask))
            {
                return;
            }

            Vector3 worldPosition = raycastCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, raycastCamera.nearClipPlane));

            // Set the Z position to 9.1
            worldPosition.z = 9.1f;

            InstantiateFairy(worldPosition);
        }

        CheckHeroProximity();
    }

    void InitializeFairyPool()
    {
        fairyPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            fairyPool[i] = Instantiate(fairyPrefab);
            fairyPool[i].SetActive(false);
            fairyPool[i].transform.SetParent(transform);
        }
    }

    void InstantiateFairy(Vector3 position)
    {
        if (isDead || _isWin)
        {
            return; // Prevent instantiating new fairies if the hero is dead or has won
        }

        if (activeFairy != null)
        {
            activeFairy.SetActive(false);
        }

        GameObject fairy = fairyPool[currentFairyIndex];

        position.z = 9.1f;
        fairy.transform.position = position;

        fairy.SetActive(true);

        activeFairy = fairy;

        currentFairyIndex = (currentFairyIndex + 1) % fairyPool.Length;
    }

    void CheckHeroProximity()
    {
        if (activeFairy == null || hero_Pos == null) return;

        float distanceToFairy = Mathf.Abs(hero_Pos.position.x - activeFairy.transform.position.x);

        if (distanceToFairy <= detectionRange)
        {
            ReturnFairyToPool();
        }
    }

    void ReturnFairyToPool()
    {
        if (activeFairy != null)
        {
            activeFairy.SetActive(false);
            activeFairy = null;
        }
    }

    // These methods can be called by the Hero script to update the hero's state
    public void SetHeroDead(bool isHeroDead)
    {
        isDead = isHeroDead;
    }

    public void SetHeroWin(bool isHeroWin)
    {
        _isWin = isHeroWin;
    }
}
