using System.Collections;
using UnityEngine;

public class Skill_Pick_Up_Script : MonoBehaviour
{
    public enum SkillType
    {
        Attack,
        Defence
    }

    public enum ElementType
    {
        Fire,
        Water,
        Air,
        Electric,
        Earth,
        All_Elements
    }

    public SkillType skillType;
    public ElementType elementType;

    public float skill_Move_Speed = 5f;

    private bool is_Moving_toHero = false;
    private Transform hero_Transform;
    private Collider skillCollider;

    private void Start()
    {
        skillCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (is_Moving_toHero && hero_Transform != null)
        {
            MoveTowardsHero();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            Hero_Script heroScript = collision.gameObject.GetComponent<Hero_Script>();

            if (heroScript != null)
            {
                // Check if the hero already has the same type of skill
                if ((skillType == SkillType.Attack && heroScript.has_Attack_Skill) ||
                    (skillType == SkillType.Defence && heroScript.has_Defence_Skill))
                {
                    Destroy(gameObject);
                    return;
                }

                hero_Transform = collision.transform;
                is_Moving_toHero = true;
                skillCollider.isTrigger = true;

                NotifyHeroScript();
                StartCoroutine(DestroySkillAfterDelay(0.25f));
            }
        }
    }

    private void MoveTowardsHero()
    {
        Vector3 targetPosition = hero_Transform.position + new Vector3(0, 0.5f, 0);
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, skill_Move_Speed * Time.deltaTime);
        }
    }

    private IEnumerator DestroySkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void NotifyHeroScript()
    {
        Hero_Script heroScript = hero_Transform.GetComponent<Hero_Script>();
        if (heroScript != null)
        {
            heroScript.has_Attack_Skill = skillType == SkillType.Attack;
            heroScript.has_Defence_Skill = skillType == SkillType.Defence;

            var heroAttackScript = heroScript.GetComponentInChildren<Hero_Attack_Script>();
            if (heroAttackScript != null)
            {
                heroAttackScript.SetSkill(skillType, elementType, gameObject);
            }
        }
    }
}