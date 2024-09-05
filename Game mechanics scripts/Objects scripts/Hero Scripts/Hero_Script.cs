using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SmallHedge.SoundManager;

public class Hero_Script : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody hero_RB;
    private Animator hero_Animator;

    [Header("State Variables")]
    public bool isGrounded = true;
    public bool facing_Right = true;
    public bool isDead = false;
    private bool _isWin = false;
    private bool isJumping = false;

    [Header("Initial Settings")]
    private Quaternion initialRotation;

    [Header("Movement Values")]
    public float hero_Speed = 1f;
    public float running_Speed_Multiplier = 1.5f;
    private float move_Horizontal;

    [Header("Target Info")]
    private Transform _objective;
    private Transform _item;
    private Transform _enemy;
    private Transform _fairy;

    [Header("Detection States")]
    public bool is_Objective_Detected = false;
    public bool is_Item_Detected = false;
    public bool is_Enemy_Detected = false;
    public bool is_Bulder_Detected = false;
    public bool is_MovTrap_Detected = false;
    public bool is_Obstacle_Detected = false;
    public bool has_Attack_Skill = false;
    public bool has_Defence_Skill = false;
    public bool is_Target_inSight = false;

    [Header("Layer Masks")]
    public LayerMask objective_LayerMask;
    public LayerMask obstacle_LayerMask;
    public LayerMask enemy_LayerMask;
    public LayerMask item_LayerMask;
    public LayerMask movTrap_LayerMask;

    [Header("Raycast")]
    public Transform attackPoint;
    float raycastRange = 0.3f;

    Hero_Life_Manager_Script heroLifeManager;

    private void Awake()
    {
        heroLifeManager = FindFirstObjectByType<Hero_Life_Manager_Script>();

        hero_RB = GetComponent<Rigidbody>();
        hero_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        hero_RB.interpolation = RigidbodyInterpolation.Interpolate;
        hero_Animator.SetTrigger("Entrance");
        initialRotation = transform.rotation;
    }


    private void Update()
    {
        if (isDead || _isWin )
        {
            move_Horizontal = 0.0f;
            return;
        }

        Vector3 direction = new Vector3(move_Horizontal, 0, 0);

        Move_Priority();
        Handle_Rotation(direction);
        Keep_Initial_Rotation();
        Animation_Update_States();
        Detect_Enemies_On_Sides();
    }

    private void FixedUpdate()
    {
        if (isDead || _isWin) return;

        Follow_Fairy(); // Follow the fairy movement logic
    }

    private void Keep_Initial_Rotation()
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
    }

    private void Move_Priority()
    {
        bool stopMovement = false;

        if ((Is_Trap_inRange(raycastRange) || Is_Obstacle_inRange(0.1f)) && isGrounded)
        {
            if (_fairy != null)
            {
                float directionToFairy = _fairy.position.x - transform.position.x;
                bool fairyOnOppositeSide = (directionToFairy > 0 && !facing_Right) || (directionToFairy < 0 && facing_Right);

                if (fairyOnOppositeSide)
                {
                    Handle_Rotation(new Vector3(directionToFairy, 0, 0));
                    stopMovement = false;
                }
                else
                {
                    Movement_Stop();
                    stopMovement = true;
                }
            }
            else
            {
                Movement_Stop();
                stopMovement = true;
            }
        }

        if (!stopMovement)
        {
            Movement_Resume();
        }
    }

    private void Follow_Fairy()
    {
        _fairy = GameObject.FindWithTag("Hero_Walk_Point")?.transform;

        if (_fairy == null) return;

        float distanceToFairy = Mathf.Abs(transform.position.x - _fairy.position.x);

        if (distanceToFairy > 0.2f)
        {
            move_Horizontal = (_fairy.position.x < transform.position.x) ? -hero_Speed : hero_Speed;
            Vector3 targetPosition = new Vector3(_fairy.position.x, transform.position.y, transform.position.z);
            hero_RB.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, Mathf.Abs(move_Horizontal) * Time.deltaTime));
        }
        else
        {
            move_Horizontal = 0f;
        }
    }



    private void Move_Towards(Transform target, float walk_Speed = 1.0f)
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - transform.position).normalized;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, walk_Speed * Time.deltaTime);
        hero_RB.MovePosition(newPosition);

        Handle_Rotation(direction);
    }

    private bool Perform_Raycast(float range, LayerMask layerMask, out RaycastHit hit, params string[] tags)
    {
        Vector3 direction = facing_Right ? Vector3.right : Vector3.left;
        Vector3 origin = transform.position + new Vector3(0, 0.45f, 0);

        Debug.DrawRay(origin, direction * range, Color.blue);

        if (Physics.Raycast(origin, direction, out hit, range, layerMask))
        {
            foreach (string tag in tags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    return true;
                }
            }
        }

        return false;
    }


    private bool Is_Trap_inRange(float range)
    {
        RaycastHit hit;
        if (Perform_Raycast(range, movTrap_LayerMask, out hit, "Bulder", "Mov_Trap"))
        {
            is_Bulder_Detected = hit.collider.CompareTag("Bulder");
            is_MovTrap_Detected = hit.collider.CompareTag("Mov_Trap");
            return true;
        }
        is_Bulder_Detected = false;
        is_MovTrap_Detected = false;
        return false;
    }

    private bool Is_Obstacle_inRange(float range)
    {
        RaycastHit hit;
        if (Perform_Raycast(range, obstacle_LayerMask, out hit, "Obstacle", "Limit", "Mov_Trap"))
        {
            is_Obstacle_Detected = true;
            return true;
        }
        is_Obstacle_Detected = false;
        return false;
    }

    private bool Is_Target_inRange(float range)
    {
        RaycastHit hit;

        if (Perform_Raycast(range, objective_LayerMask, out hit, "Chest", "Buddy", "Gem"))
        {
            if (!Perform_Raycast(range, obstacle_LayerMask, out hit, "Obstacle", "Limit", "Mov_Trap"))
            {
                is_Target_inSight = true;
                return true;
            }
        }

        is_Target_inSight = false;
        return false;
    }

    private Transform[] Find_Targets_Layer(LayerMask layerMask, params string[] tags)
    {
        List<Transform> targets = new List<Transform>();
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                if (((1 << obj.layer) & layerMask) != 0)
                {
                    targets.Add(obj.transform);
                }
            }
        }
        return targets.ToArray();
    }

    private void Detect_Enemies_On_Sides()
    {
           
        if (_fairy != null) return;

        RaycastHit hitRight;
        RaycastHit hitLeft;

        bool enemyOnRight = Perform_Front_Raycast(Vector3.right, out hitRight);
        bool enemyOnLeft = Perform_Front_Raycast(Vector3.left, out hitLeft);

        if (enemyOnRight && hitRight.collider.CompareTag("Enemy"))
        {
            facing_Right = true;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (enemyOnLeft && hitLeft.collider.CompareTag("Enemy") )
        {
            facing_Right = false;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }


    private bool Perform_Front_Raycast(Vector3 direction, out RaycastHit hit)
    {
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        float range = 2f;

        Debug.DrawRay(origin, direction * range, Color.green);

        if (Physics.Raycast(origin, direction, out hit, range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }

            if (((1 << hit.collider.gameObject.layer) & objective_LayerMask) != 0)
            {
                return true;
            }
        }

        return false;
    }


    public void Movement_Stop()
    {
        move_Horizontal = 0.0f;
        hero_RB.isKinematic = true;
    }

    public void Movement_Resume()
    {
        hero_RB.isKinematic = false;
    }

    private void Handle_Rotation(Vector3 direction)
    {
        if (direction.x < 0 && facing_Right)
        {
            facing_Right = false;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (direction.x > 0 && !facing_Right)
        {
            facing_Right = true;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem") && Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.GEMS)
        {
            Handle_Win_State();
        }
        else if (other.CompareTag("Chest") && Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.CHEST)
        {
            Handle_Win_State();
        }
        else if (other.CompareTag("Jump_Point") && !isJumping)
        {
            Handle_Edge_Jump();
        }

        switch (other.gameObject.tag)
        {
            case "Lava":
                Handle_MagmaCollision();
                break;
            case "Poison":
                Handle_PoisonCollision();
                break;
            case "Gas":
                Handle_Gas_Collision();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jump_Point"))
        {
            isJumping = false;
        }
    }

    private void Handle_Edge_Jump()
    {
        isJumping = true;
        hero_Animator.SetBool("isJumping", true);

        Vector3 forwardDirection = facing_Right ? Vector3.right : Vector3.left;

        float jumpForce = 0.5f;
        hero_RB.linearVelocity = new Vector3(hero_RB.linearVelocity.x, jumpForce, hero_RB.linearVelocity.z);

        float forwardForce = 0.7f;
        hero_RB.linearVelocity += forwardDirection * forwardForce;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (Game_Manager_Script.instance.isGameOver || Game_Manager_Script.instance.isGameWin) return;

        switch (collision.gameObject.tag)
        {
            case "Chest":
                if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.CHEST)
                {
                    StartCoroutine(Delayed_WinState());
                }
                break;
            case "Gem_Stone":
                if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.GEMS)
                {
                    Handle_Win_State();
                }
                break;
            case "Buddy":
                if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.BUDDY)
                {
                    Handle_Win_State();
                }
                break;
            case "Enemy_ATK":
                if (!has_Defence_Skill)
                    heroLifeManager.LowerLife();
                break;
            case "Enemy":
                if (!has_Defence_Skill)
                    Handle_EnemyCollision(collision);
                break;
            case "Skill":
                Handle_Skill_Pickup(collision.gameObject.GetComponent<Skill_Pick_Up_Script>());
                Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.SKILL_PICKUP);
                break;
            case "Bulder":
                Handle_Bulder_Collision(collision);
                break;
            case "Ground":
            case "Obstacle":
            case "Pin":
            case "Lava_Rock":
            case "Water":
            case "Mov_Obstacle":
                isGrounded = true;
                break;
            default:
                break;
        }
    }

    private void Handle_Skill_Pickup(Skill_Pick_Up_Script skillPickup)
    {
        if (skillPickup == null) return;

        var heroAttackScript = GetComponentInChildren<Hero_Attack_Script>();
        if (heroAttackScript != null)
        {
            heroAttackScript.SetSkill(skillPickup.skillType, skillPickup.elementType, skillPickup.gameObject);

            if (has_Attack_Skill)
            {
                heroAttackScript.SetSkill(skillPickup.skillType, skillPickup.elementType, skillPickup.gameObject);
            }

            if (skillPickup.skillType == Skill_Pick_Up_Script.SkillType.Attack)
            {
                has_Attack_Skill = true;
            }
            else if (skillPickup.skillType == Skill_Pick_Up_Script.SkillType.Defence)
            {
                has_Defence_Skill = true;
            }
        }
    }

    private void Handle_Bulder_Collision(Collision collision)
    {
        Bulder_Script bulderScript = collision.gameObject.GetComponent<Bulder_Script>();
        if (bulderScript != null && bulderScript.IsMoving && !isDead)
        {
            Hero_isDead();
            heroLifeManager.ResetLife();

        }
    }

    private IEnumerator Rotate_Hero()
    {
        float targetAngle = facing_Right ? 90f : -90f;
        float currentAngle = transform.eulerAngles.y;
        float finalAngle = currentAngle + targetAngle;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, finalAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, finalAngle, 180f * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return null;
        }
        transform.eulerAngles = new Vector3(0, finalAngle, 0);
    }

    public void Handle_Win_State()
    {
        _isWin = true;
        Game_Manager_Script.instance.Game_is_Win();
        move_Horizontal = 0.0f;
        StartCoroutine(Rotate_Hero());
        hero_Animator.SetBool("isWinning", true);
        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.Win);
    }

    private IEnumerator Delayed_WinState()
    {
        yield return new WaitForSeconds(2f);
        Handle_Win_State();
    }

    private void Handle_EnemyCollision(Collision collision)
    {
        Hero_isDead();
        heroLifeManager.ResetLife();

        hero_Animator.SetTrigger("isHit");
    }

    private void Handle_MagmaCollision()
    {
        if (!isDead)
        {
            Hero_isDead();
            heroLifeManager.ResetLife();

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Magma_Layer"));
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Magma_Layer"));
        }
    }

    private void Handle_PoisonCollision()
    {
        if (!isDead)
        {
            Hero_isDead();
            heroLifeManager.ResetLife();

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Poison_Layer"));
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Poison_Layer"));
        }
    }

    private void Handle_Gas_Collision()
    {
        if (!isDead)
        {
            Hero_isDead();
            heroLifeManager.ResetLife();

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Gas_Layer"));
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Gas_Layer"));
        }
    }

    internal void Hero_isDead()
    {
        Play_Hero_Death_Animation();

        StartCoroutine(SetKinematicAfterDelay(0.5f));

        Energy_Manager_Script._instance.UpdateEnergy(Game_Manager_Script.instance.total_Energy);
        Game_Manager_Script.instance.Game_is_Over();
        Sound_Manager_Script.Play_Hero_Sound(Hero_SoundType.Lose);
    }

    IEnumerator SetKinematicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hero_RB.isKinematic = true;
        hero_RB.GetComponent<Collider>().isTrigger = true;

    }

    public void Add_Gems(int amount)
    {
        Game_Manager_Script.instance.gems_Current += amount;
        PlayerPrefs.SetInt("Gem", Game_Manager_Script.instance.gems_Current);
        UI_inGame_Manager_Script._instance.ShowGemsText(Game_Manager_Script.instance.gems_Current);
    }
    private enum Animation_States
    {
        Idle,
        Running,
        Walking,
        Falling,
        Jumping
    }

    private void Animation_Update_States()
    {
        isGrounded = Is_Grounded();

        Animation_States animationState = DetermineAnimationState();

        Update_Animator(animationState);
    }

    private Animation_States DetermineAnimationState()
    {
        if (isJumping)
        {
            return Animation_States.Jumping;
        }

        if (!isGrounded)
        {
            return Animation_States.Falling;
        }

        if (_fairy == null || Mathf.Abs(move_Horizontal) < 0.1f)
        {
            return Animation_States.Idle;
        }

        if (Is_Target_inRange(raycastRange))
        {
            return Animation_States.Running;
        }

        return Animation_States.Walking;
    }

    private void Update_Animator(Animation_States state)
    {
        hero_Animator.SetBool("isIdle", state == Animation_States.Idle);
        hero_Animator.SetBool("isWalking", state == Animation_States.Walking);
        hero_Animator.SetBool("isRunning", state == Animation_States.Running);
        hero_Animator.SetBool("isJumping", state == Animation_States.Jumping);
        hero_Animator.SetBool("isFalling", state == Animation_States.Falling);
    }

    public void Trigger_Hit_Animation()
    {
        hero_Animator.SetTrigger("IsHit");

    }
    public void Trigger_ATK_Animation()
    {
        hero_Animator.SetTrigger("isAttacking");
    }

    public void Play_Hero_Death_Animation()
    {
        isDead = true;
        hero_Animator.SetTrigger("isDead");
    }

    private bool Is_Grounded()
    {
        float offset = 0.25f;
        Vector3 originLeft = transform.position + Vector3.up * 0.1f + Vector3.left * offset;
        Vector3 originRight = transform.position + Vector3.up * 0.1f + Vector3.right * offset;
        Vector3 originCenter = transform.position + Vector3.up; 
        float rayLength = 0.3f;

        Debug.DrawRay(originLeft, Vector3.down * rayLength, Color.red);
        Debug.DrawRay(originRight, Vector3.down * rayLength, Color.red);
        Debug.DrawRay(originCenter, Vector3.down * rayLength, Color.red); 

        RaycastHit hitLeft, hitRight, hitCenter;

        bool hitLeftGround = Physics.Raycast(originLeft, Vector3.down, out hitLeft, rayLength);
        bool hitRightGround = Physics.Raycast(originRight, Vector3.down, out hitRight, rayLength);
        bool hitCenterGround = Physics.Raycast(originCenter, Vector3.down, out hitCenter, rayLength);

        if (hitLeftGround || hitRightGround || hitCenterGround)
        {
            string hitTagLeft = hitLeftGround ? hitLeft.collider.tag : string.Empty;
            string hitTagRight = hitRightGround ? hitRight.collider.tag : string.Empty;
            string hitTagCenter = hitCenterGround ? hitCenter.collider.tag : string.Empty; 

            if (IsGroundTag(hitTagLeft) || IsGroundTag(hitTagRight) || IsGroundTag(hitTagCenter))
            {
                isGrounded = true;
                isJumping = false;

                return true;
            }
        }

        isGrounded = false;
        return false;
    }


    private bool IsGroundTag(string tag)
    {
        switch (tag)
        {
            case "Ground":
            case "Obstacle":
            case "Pin":
            case "Enemy":
            case "Lava_Rock":
            case "Lava":
            case "Buddy":
            case "Poison":
            case "Water":
            case "Mov_Obstacle":
            case "Gem_Stone":
                return true;
            default:
                return false;
        }
    }


}