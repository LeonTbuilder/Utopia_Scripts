using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Attack_Script : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject fire_Attack_PF;
    public GameObject water_Attack_PF;
    public GameObject electric_Attack_PF;
    public GameObject earth_Attack_PF;
    public GameObject air_Attack_PF;

    public Transform attack_Point;

    [Header("Defence Settings")]
    public GameObject air_Defence_PF;

    public Transform defence_Point;

    [Header("Aura Settings")]
    public GameObject fire_Aura_PF;
    public GameObject water_Aura_PF;
    public GameObject air_Aura_PF;
    public GameObject electric_Aura_PF;
    public GameObject earth_Aura_PF;

    public Transform aura_Point;

    private Hero_Script heroScript;
    private bool attackUsed = false;
    private Skill_Pick_Up_Script.ElementType currentElementType;

    private void Awake()
    {
        heroScript = GetComponentInParent<Hero_Script>();
    }

    private void Start()
    {
        Attack_UI_Script.instance.SetHeroAttackScript(this);
    }

    public void SetSkill(Skill_Pick_Up_Script.SkillType skillType, Skill_Pick_Up_Script.ElementType elementType, GameObject pickup)
    {
        if (skillType == Skill_Pick_Up_Script.SkillType.Attack)
        {
            heroScript.has_Attack_Skill = true;
            currentElementType = elementType;  // Store the current element type
            Attack_UI_Script.instance.instantiateAttackButton.gameObject.SetActive(true); // Enable the instantiate button

            // Update the current element type in the Attack UI
            Attack_UI_Script.instance.SetCurrentElementType(elementType);

            // Reset attackUsed when a new skill is picked up
            attackUsed = false;

            if (elementType == Skill_Pick_Up_Script.ElementType.All_Elements)
            {
                All_Element_Collected();
            }
        }
        else if (skillType == Skill_Pick_Up_Script.SkillType.Defence)
        {
            heroScript.has_Defence_Skill = true;
            InstantiateDefenceSkill();
        }
    }

    public void InstantiateSelectedAttack(Attack_UI_Script.AttackType selectedAttackType, Skill_Pick_Up_Script.ElementType elementType)
    {
        if (attackUsed) return;

        GameObject attackPrefab = null;

        if (elementType == Skill_Pick_Up_Script.ElementType.All_Elements)
        {
            // Instantiate based on the chosen attack type if "All Elements" is picked up
            switch (selectedAttackType)
            {
                case Attack_UI_Script.AttackType.Fire:
                    attackPrefab = fire_Attack_PF;
                    break;
                case Attack_UI_Script.AttackType.Water:
                    attackPrefab = water_Attack_PF;
                    break;
                case Attack_UI_Script.AttackType.Electric:
                    attackPrefab = electric_Attack_PF;
                    break;
                case Attack_UI_Script.AttackType.Earth:
                    attackPrefab = earth_Attack_PF;
                    break;
                case Attack_UI_Script.AttackType.Air:
                    attackPrefab = air_Attack_PF;
                    break;
            }
        }
        else
        {
            // Instantiate based on the element type of the skill picked up
            switch (currentElementType)
            {
                case Skill_Pick_Up_Script.ElementType.Fire:
                    attackPrefab = fire_Attack_PF;
                    break;
                case Skill_Pick_Up_Script.ElementType.Water:
                    attackPrefab = water_Attack_PF;
                    break;
                case Skill_Pick_Up_Script.ElementType.Electric:
                    attackPrefab = electric_Attack_PF;
                    break;
                case Skill_Pick_Up_Script.ElementType.Earth:
                    attackPrefab = earth_Attack_PF;
                    break;
                case Skill_Pick_Up_Script.ElementType.Air:
                    attackPrefab = air_Attack_PF;
                    break;
            }
        }

        if (attackPrefab != null)
        {
            Vector3 direction = heroScript.facing_Right ? Vector3.right : Vector3.left;
            heroScript.Trigger_ATK_Animation();
            Ready_Set_Attack(attackPrefab, direction);
            attackUsed = true; // Mark the attack as used

            // Reset has_Attack_Skill to false after the attack is instantiated
            heroScript.has_Attack_Skill = false;
        }
    }

    void Ready_Set_Attack(GameObject attackPrefab, Vector3 direction)
    {
        GameObject attackInstance = Instantiate(attackPrefab, attack_Point.position, attack_Point.rotation);
        attackInstance.transform.SetParent(transform.parent);
        attackInstance.GetComponent<I_HeroAttack>().Initialize(direction);
    }

    private void InstantiateDefenceSkill()
    {
        GameObject defencePrefab = null;

        // Logic for instantiating defence skills can go here

        if (defencePrefab != null)
        {
            GameObject defenceInstance = Instantiate(defencePrefab, defence_Point.position, defence_Point.rotation);
            defenceInstance.transform.SetParent(defence_Point);
        }
    }

    public void All_Element_Collected()
    {
        Attack_UI_Script.instance.ShowAttackUI();
    }
}

public interface I_HeroAttack
{
    void Initialize(Vector3 direction);
}