using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy_Script : MonoBehaviour
{
    Rigidbody buddy_RB;
    Animator buddy_Animator;

    public float buddy_Speed;

    public bool _isWin = false, isDead = false;
    private Transform _target;

    [Header("Detection States")]
    public bool isTargetDetected = false;
    public LayerMask heroLayerMask;
    public LayerMask enemyLayerMask;

    void Start()
    {
        buddy_RB = GetComponent<Rigidbody>();
        buddy_Animator = GetComponent<Animator>();
        StartCoroutine(Play_Animations());
    }

    void FixedUpdate()
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin || _isWin || isDead)
            return;

        Detect_Targets();
        Look_At_Target();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin)
            return;

        switch (collision.gameObject.tag)
        {
            case "Lava":
                Handle_LavaCollision();
                break;
            case "Hero":
                Handle_HeroCollision();
                break;
            case "Enemy":
                Handle_EnemyCollision(collision);
                break;
            case "Poison":
                Handle_PoisonCollision();
                break;
            case "Gas":
                Handle_PoisonCollision();
                break;
            case "Bulder":
                Handle_Bulder_Collision(collision);
                break;
            default:
                break;
        }
    }

    private void Handle_LavaCollision()
    {
        if (!isDead)
        {
            isDead = true;
            buddy_Animator.SetTrigger("Death");
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Buddy_Layer"), LayerMask.NameToLayer("Magma_Layer"));
            Invoke(nameof(Execute_Game_Over), 1.5f);
        }
    }

    private void Handle_PoisonCollision()
    {
        if (!isDead)
        {
            isDead = true;
            buddy_Animator.SetTrigger("Death");
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Buddy_Layer"), LayerMask.NameToLayer("MetaBalls"));
            Invoke(nameof(Execute_Game_Over), 1.5f);
        }
    }

    private void Handle_HeroCollision()
    {
        _isWin = true;
    }

    private void Handle_EnemyCollision(Collision collision)
    {
        isDead = true;
        buddy_Animator.SetTrigger("Death");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Buddy_Layer"), LayerMask.NameToLayer("Enemy_Layer"));
        Invoke(nameof(Execute_Game_Over), 1.5f);
    }

    private void Handle_Bulder_Collision(Collision collision)
    {
        if (!isDead)
        {
            buddy_Animator.SetTrigger("Hit");
            StartCoroutine(Delayed_Buddy_Death());
        }
    }

    private IEnumerator Delayed_Buddy_Death()
    {
        yield return new WaitForSeconds(1f);
        isDead = true;
        buddy_Animator.SetTrigger("Death");
        Execute_Game_Over();
    }

    private void Execute_Game_Over()
    {
        if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.BUDDY)
        {
            Hero_Script hero = FindFirstObjectByType<Hero_Script>();
            if (hero != null)
            {
                hero.Play_Hero_Death_Animation();
            }
        }

        Destroy(gameObject);
        Game_Manager_Script.instance.Game_is_Over();
    }

    private void Detect_Targets()
    {
        RaycastHit hit;
        Vector3 direction = Vector3.right;
        Vector3 origin = transform.position + new Vector3(0, 0.8f, 0);

        // Detect hero
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, heroLayerMask))
        {
            if (hit.collider.CompareTag("Hero"))
            {
                isTargetDetected = true;
                _target = hit.transform;
                return;
            }
        }

        // Detect enemy
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, enemyLayerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                isTargetDetected = true;
                _target = hit.transform;
                return;
            }
        }

        isTargetDetected = false;
        _target = null;
    }

    private void Look_At_Target()
    {
        if (_target == null)
            return;

        Vector3 directionToTarget = _target.position - transform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    IEnumerator Play_Animations()
    {
        while (!isDead && !_isWin)
        {
            int randomAnimation = Random.Range(0, 3);
            switch (randomAnimation)
            {
                case 0:
                    buddy_Animator.Play("Wait");
                    break;
                case 1:
                    buddy_Animator.Play("Idle");
                    break;
                case 2:
                    buddy_Animator.Play("Idle_Action");
                    break;
            }
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }
}
