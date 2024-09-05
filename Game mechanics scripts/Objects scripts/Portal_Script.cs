using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;

public class Portal_Script : MonoBehaviour
{
    [Header("Portal Settings")]
    
    public Transform centerOfPortal;
    public float moveSpeed = 5f;
    float minScale = 0.4f;

    Transform exit_portal; 

    private GameObject Hero_Object;
    private Transform heroTransform;
    private Rigidbody heroRigidbody;
    private bool isMovingToCenter = false;
    private Vector3 originalScale;

    private void Awake()
    {
        Initialize_Hero();
        FindExitPortal();
    }

    private void Update()
    {
        if (isMovingToCenter && heroTransform != null)
        {
            Pull_Hero_To_Center();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Hero") || collision.CompareTag("Buddy"))
        {
            Move_To_Center();
        }
        else if (collision.CompareTag("Skill"))
        {
            StartCoroutine(Teleport_And_Regrow_Skill(collision.transform));
        }
    }

    private void Initialize_Hero()
    {
        Hero_Object = GameObject.FindGameObjectWithTag("Hero");
        if (Hero_Object == null)
        {
            Hero_Object = GameObject.FindGameObjectWithTag("Buddy");
        }

        if (Hero_Object != null)
        {
            heroTransform = Hero_Object.transform;
            heroRigidbody = Hero_Object.GetComponent<Rigidbody>();
            originalScale = heroTransform.localScale;
        }
        else
        {
            Debug.LogWarning("Hero or Buddy object not found!");
        }
    }

    private void FindExitPortal()
    {
        GameObject portalOutObject = GameObject.FindGameObjectWithTag("Portal_Out");
        if (portalOutObject != null)
        {
            exit_portal = portalOutObject.transform;
        }
        else
        {
            Debug.LogWarning("Exit portal with tag 'Portal_Out' not found!");
        }
    }

    private void Move_To_Center()
    {
        if (!isMovingToCenter && heroRigidbody != null)
        {
            StartCoroutine(Delayed_Teleport());
        }
    }

    private IEnumerator Delayed_Teleport()
    {
        yield return new WaitForSeconds(0.5f);

        isMovingToCenter = true;
        heroRigidbody.isKinematic = true;
        heroRigidbody.useGravity = false;
    }

    private void Pull_Hero_To_Center()
    {
        Vector3 targetPosition = centerOfPortal.position;
        float distance = Vector3.Distance(heroTransform.position, targetPosition);
        float totalDistance = Vector3.Distance(exit_portal.position, targetPosition);

        if (distance > 0.05f)
        {
            heroTransform.position = Vector3.MoveTowards(heroTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            Scale_Hero_Size(distance, totalDistance);
        }
        else
        {
            Finish_Pulling_To_Center();
        }
    }

    private void Scale_Hero_Size(float distance, float totalDistance)
    {
        float scaleFactor = Mathf.Lerp(minScale, 1.0f, distance / totalDistance);
        heroTransform.localScale = originalScale * scaleFactor;
    }

    private void Finish_Pulling_To_Center()
    {
        heroTransform.localScale = originalScale * minScale;
        isMovingToCenter = false;
        StartCoroutine(Teleport_Hero());
    }

    private IEnumerator Teleport_Hero()
    {
        yield return new WaitForSeconds(0.1f);

        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_WARP_IN);

        if (exit_portal != null)
        {
            if (heroRigidbody != null)
            {
                heroRigidbody.linearVelocity = Vector3.zero;
                heroRigidbody.angularVelocity = Vector3.zero;
            }

            heroTransform.position = exit_portal.position;

            StartCoroutine(Regrow_Hero_Size());
        }
    }

    private IEnumerator Regrow_Hero_Size()
    {
        float elapsedTime = 0f;
        float duration = 1f; // Time to regrow to original size
        Vector3 startScale = heroTransform.localScale;
        Vector3 endScale = originalScale;

        while (elapsedTime < duration)
        {
            heroTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_WARP_OUT);
        heroTransform.localScale = originalScale;

        if (heroRigidbody != null)
        {
            heroRigidbody.isKinematic = false;
            heroRigidbody.useGravity = true;
        }
    }

    private IEnumerator Teleport_And_Regrow_Skill(Transform skillTransform)
    {
        Rigidbody skillRigidbody = skillTransform.GetComponent<Rigidbody>();
        Vector3 skillOriginalScale = skillTransform.localScale;

        yield return new WaitForSeconds(0.05f); // Faster delay before teleportation for skills

        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_WARP_IN);

        if (exit_portal != null)
        {
            if (skillRigidbody != null)
            {
                skillRigidbody.linearVelocity = Vector3.zero;
                skillRigidbody.angularVelocity = Vector3.zero;
                skillRigidbody.isKinematic = true;
                skillRigidbody.useGravity = false;
            }

            skillTransform.position = exit_portal.position;
            yield return Regrow_Object_Size(skillTransform, skillOriginalScale, 0.5f); // Faster regrow duration for skills
        }

        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_WARP_OUT);

        if (skillRigidbody != null)
        {
            skillRigidbody.isKinematic = false;
            skillRigidbody.useGravity = true;
        }
    }

    private IEnumerator Regrow_Object_Size(Transform objTransform, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startScale = objTransform.localScale;

        while (elapsedTime < duration)
        {
            objTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objTransform.localScale = targetScale;
    }
}
