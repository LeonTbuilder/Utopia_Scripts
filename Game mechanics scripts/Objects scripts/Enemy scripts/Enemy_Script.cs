using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Script : MonoBehaviour
{

    public bool is_Projectile_attack = true;
    private Rigidbody enemy_RB;
    private GameObject _target;
    private Coroutine randomDirectionCoroutine;
    protected internal Animator Enemy_Animator;
    private bool isChangingDirection = false;
    private bool isPushedBack = false;
    private bool isGrounded = false;

    [Header("Movement Settings")]
    public float enemy_Speed = 6.4f;
    public float mov_Horizontal = 1f;
    public float mov_Vertical = 0.0f;
    private float current_Direction = 0.5f;

    [Header("Layer Masks")]
    public LayerMask obstacle_Layer;
    public LayerMask heroLayerMask;
    public LayerMask buddyLayerMask;
    public LayerMask jumpPointLayerMask;

    [Header("Detection States")]
    public bool isTargetDetected = false;
    public bool isObstacleDetected = false;

    [Header("State Settings")]
    public bool _isWin = false;
    public bool isDead = false;

    /*protected virtual*/ void Start()
    {
        enemy_RB = GetComponent<Rigidbody>();
        Enemy_Animator = GetComponent<Animator>();
        Enemy_Animator.SetTrigger("Entrance");

        randomDirectionCoroutine = StartCoroutine(Change_Random_Direction());
    }

    /*protected virtual*/ void FixedUpdate()
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin || _isWin || isDead || isPushedBack)
            return;

        if (!Is_Grounded())
        {
            Handle_Fall();
        }
        else
        {
            Detect_JumpPoint_And_HandleMovement();
        }

        Update_Animation_States();
    }

    private void Handle_Fall()
    {
        // Maintain x position, only allow vertical falling
        Vector3 currentVelocity = enemy_RB.linearVelocity;
        enemy_RB.linearVelocity = new Vector3(0, currentVelocity.y, currentVelocity.z);
    }

    private void Detect_JumpPoint_And_HandleMovement()
    {
        if (Detect_JumpPoint())
        {
            StartCoroutine(HandleJumpPointDetection());
        }
        else
        {
            Detect_And_Move();
        }
    }

    private bool Detect_JumpPoint()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = mov_Horizontal > 0 ? Vector3.right : Vector3.left;

        Debug.DrawRay(origin, direction * 0.45f, Color.green);

        if (Physics.Raycast(origin, direction, out hit, 0.45f, jumpPointLayerMask))
        {
            if (hit.collider.CompareTag("Jump_Point"))
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator HandleJumpPointDetection()
    {
        enemy_RB.isKinematic = true;
        mov_Horizontal = 0;

        current_Direction *= -1;
        mov_Horizontal = current_Direction;

        yield return null;

        enemy_RB.isKinematic = false;

        Detect_And_Move();
    }

    private void Update_Animation_States()
    {
        Enemy_Animator.SetBool("isWalking", mov_Horizontal != 0 && isGrounded);
        Enemy_Animator.SetBool("isIdle", mov_Horizontal == 0 && isGrounded);
        Enemy_Animator.SetBool("isFalling", !isGrounded);
    }

    private bool Is_Grounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        float rayLength = 0.4f;

        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                case "Obstacle":
                case "Pin":
                case "Lava_Rock":
                case "Lava":
                case "Buddy":
                case "Poison":
                case "Water":
                case "Mov_Obstacle":
                case "Gem_Stone":
                    isGrounded = true;
                    return true;
            }
        }

        isGrounded = false;
        return false;
    }

    private void Detect_And_Move()
    {
        isTargetDetected = Detect_Target(heroLayerMask) || Detect_Target(buddyLayerMask);
        isObstacleDetected = Is_Obstacle_inWay();

        if (isTargetDetected && !isObstacleDetected)
        {
            Stop_Random_Movement();
            Move_toward_Target();
            Rotate_Towards_Target();
        }
        else
        {
            Start_Random_Movement();
        }

        if (!Enemy_Animator.GetBool("isAttacking"))
        {
            Vector3 movement = new Vector3(mov_Horizontal, 0.0f, mov_Vertical);
            Handle_Rotation();
            enemy_RB.AddForce(movement * enemy_Speed);
        }

        if (!is_Projectile_attack) Enemy_Attack_ifClose();
    }

    private bool Detect_Target(LayerMask layerMask)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + new Vector3(0, 0.6f, 0);
        Vector3 direction = mov_Horizontal > 0 ? Vector3.right : Vector3.left;

        Debug.DrawRay(origin, direction * 10f, Color.red);

        if (Physics.Raycast(origin, direction, out hit, 10f, layerMask))
        {
            if (hit.collider.CompareTag("Hero") || hit.collider.CompareTag("Buddy"))
            {
                _target = hit.collider.gameObject;
                return true;
            }
        }

        _target = null;
        return false;
    }

    private bool Is_Obstacle_inWay()
    {
        if (_target == null) return false;

        Vector3 direction = (_target.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _target.transform.position);
        Vector3 origin = transform.position + new Vector3(0, 0.4f, 0);

        return Physics.Raycast(origin, direction, distance, obstacle_Layer);
    }

    private void Move_toward_Target()
    {
        if (_target != null)
        {
            float direction = _target.transform.position.x - transform.position.x;
            mov_Horizontal = Mathf.Sign(direction);
        }
    }

    private void Rotate_Towards_Target()
    {
        if (_target != null)
        {
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Enemy_Attack_ifClose()
    {
        if (_target != null && Vector3.Distance(transform.position, _target.transform.position) < 1.3f)
        {
            
            Enemy_Animator.SetBool("isAttacking", true);
            
            mov_Horizontal = 0;
            enemy_RB.linearVelocity = Vector3.zero;
            enemy_RB.angularVelocity = Vector3.zero;
        }
        else
        {
            Enemy_Animator.SetBool("isAttacking", false);
        }
    }

    void Handle_Rotation()
    {
        if (mov_Horizontal < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (mov_Horizontal > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void Start_Random_Movement()
    {
        if (randomDirectionCoroutine == null)
        {
            randomDirectionCoroutine = StartCoroutine(Change_Random_Direction());
        }
    }

    private void Stop_Random_Movement()
    {
        if (randomDirectionCoroutine != null)
        {
            StopCoroutine(randomDirectionCoroutine);
            randomDirectionCoroutine = null;
        }
    }

    private IEnumerator Change_Random_Direction()
    {
        while (true)
        {
            if (isDead) yield break;

            float randomTime = Random.Range(2f, 2.5f);

            if (!isChangingDirection)
            {
                current_Direction = current_Direction == 1.0f ? -1.0f : 1.0f;
                mov_Horizontal = current_Direction;
            }

            yield return new WaitForSeconds(randomTime);
        }
    }

    private IEnumerator Direction_Change_Cooldown()
    {
        isChangingDirection = true;
        yield return new WaitForSeconds(2f);
        isChangingDirection = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin)
            return;

        switch (collision.gameObject.tag)
        {
            case "HeroDEF":
                Handle_Damage_Collider(collision);
                break;
            case "Electric_ATK":
                Handle_Electric_ATK(collision);
                break;
            case "Lava":
                Handle_Liquid_Collision(collision);
                break;
            case "Gas":
                Handle_Gas_Collision(collision);
                break;
            case "Poison":
                Handle_Liquid_Collision(collision);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin)
            return;

        switch (collision.gameObject.tag)
        {
            case "Buddy":
                Handle_Buddy_Collision();
                break;
            case "Fire_ATK":
                Handle_Damage_Collision(collision);
                break;
            case "Water_ATK":
                Handle_Damage_Collision(collision);
                break;
            case "Earth_ATK":
                Handle_Damage_Collision(collision);
                break;
            case "Bulder":
                Handle_Bulder_Collision(collision);
                break;
            case "EnemyWalkPoint":
                StartCoroutine(HandleJumpPointDetection());
                break;
        }
    }

    private void Handle_Buddy_Collision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Buddy_Layer"), LayerMask.NameToLayer("Enemy_Layer"));
        mov_Horizontal = 0.0f;
        StartCoroutine(End_The_Level());
    }

    private void Handle_Bulder_Collision(Collision collision)
    {
        Bulder_Script bulderScript = collision.gameObject.GetComponent<Bulder_Script>();
        if (bulderScript.IsMoving && !isDead)
        {
            Handle_Damage_Collision(collision);
        }
    }

    private void Handle_Damage_Collider(Collider collider)
    {
        if (!isDead && transform.position.y < collider.transform.position.y)
        {
            Vector3 forceDirection = collider.gameObject.CompareTag("HeroDEF") ? new Vector3(1f, 0.0f, 0.0f) : new Vector3(0.0f, 1.0f, 0.0f);
            Receive_Damage(forceDirection);
        }

        Game_Manager_Script.instance.Enemy_Killed_inLevelType(gameObject);
        IncrementEnemiesKilled();
    }

    private void Handle_Electric_ATK(Collider collider)
    {
        isDead = true;
        mov_Horizontal = 0.0f;
        Enemy_Animator.SetTrigger("Hit");
        Enemy_Animator.SetTrigger("Die");
        Destroy(gameObject, 1.2f);
        Game_Manager_Script.instance.Enemy_Killed_inLevelType(gameObject);
        IncrementEnemiesKilled();
    }

    private void Handle_Liquid_Collision(Collider collider)
    {
        if (!isDead)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy_Layer"), LayerMask.NameToLayer("Magma_Layer"));
            Handle_Damage_Collider(collider);
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy_Layer"), LayerMask.NameToLayer("Magma_Layer"));
        }
    }

    private void Handle_Gas_Collision(Collider collider)
    {
        if (!isDead)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy_Layer"), LayerMask.NameToLayer("Gas_Layer"));
            Handle_Damage_Collider(collider);
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy_Layer"), LayerMask.NameToLayer("Gas_Layer"));
        }
    }

    private List<string> attack_Damage_Tags = new List<string> { "HeroATK", "Fire_ATK", "Water_ATK", "Electric_ATK", "Earth_ATK" };
    private void Handle_Damage_Collision(Collision collision)
    {
        if (!isDead && transform.position.y < collision.transform.position.y)
        {
            Enemy_Animator.SetTrigger("Hit");

            enemy_RB.isKinematic = true;
            enemy_RB.GetComponent<Collider>().isTrigger = true;
            Vector3 forceDirection = attack_Damage_Tags.Contains(collision.gameObject.tag)
                ? new Vector3(10.0f, 0.0f, 0.0f)
                : new Vector3(0.0f, 10.0f, 0.0f);
            Receive_Damage(forceDirection);
        }
    }

    public void Receive_Damage(Vector3 forceDirection)
    {
        enemy_RB.AddForce(forceDirection);
        Enemy_Animator.SetTrigger("Hit");
        StartCoroutine(Get_Damage_IE(forceDirection));
    }

    IEnumerator Get_Damage_IE(Vector3 forceDirection)
    {
        yield return new WaitForSeconds(0.15f);
        enemy_RB.AddForce(forceDirection * 2);
        Enemy_Animator.SetTrigger("Die");
        Destroy(gameObject, 1.2f);

        yield return new WaitForSeconds(0.7f);

        isDead = true;

        Game_Manager_Script.instance.Enemy_Killed_inLevelType(gameObject);
        IncrementEnemiesKilled();
    }

    IEnumerator End_The_Level()
    {
        yield return new WaitForSeconds(1.0f);
        Game_Manager_Script.instance.Game_is_Over();
    }

    public void SetPushedBack(bool value)
    {
        isPushedBack = value;
    }

    private void IncrementEnemiesKilled()
    {
        int enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled + 1);
    }
}
