using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Attack_UI_Script : MonoBehaviour
{
    public static Attack_UI_Script instance;
    public GameObject attackUI;
    public Button instantiateAttackButton; // Button to instantiate the chosen attack
    public Sprite defaultButtonImage;  // Default image for the buttons
    public Sprite selectedButtonImage; // Image to show when a button is selected
    private Hero_Attack_Script heroAttackScript;
    private Animator attackUIAnimator;

    private AttackType selectedAttackType;
    private Skill_Pick_Up_Script.ElementType currentElementType; // Track the current element type

    private Button lastSelectedButton; // Track the last selected button

    public enum AttackType
    {
        Fire,
        Water,
        Electric,
        Earth,
        Air,
        None
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        attackUIAnimator = attackUI.GetComponent<Animator>();
        attackUI.SetActive(false);
        instantiateAttackButton.gameObject.SetActive(false); // Initially, hide the instantiate button
    }

    public void SetHeroAttackScript(Hero_Attack_Script script)
    {
        heroAttackScript = script;
    }

    public void ShowAttackUI()
    {
        attackUI.SetActive(true);
    }

    public void HideAttackUI()
    {
        attackUI.SetActive(false);
    }

    // The methods for each button that will be assigned in the inspector
    public void Fire_Attack_Button(Button button) => SetSelectedAttackType(AttackType.Fire, button);
    public void Water_Attack_Button(Button button) => SetSelectedAttackType(AttackType.Water, button);
    public void Electric_Attack_Button(Button button) => SetSelectedAttackType(AttackType.Electric, button);
    public void Earth_Attack_Button(Button button) => SetSelectedAttackType(AttackType.Earth, button);
    public void Air_Attack_Button(Button button) => SetSelectedAttackType(AttackType.Air, button);

    private void SetSelectedAttackType(AttackType attackType, Button button)
    {
        // Revert the image of the last selected button to the default image
        if (lastSelectedButton != null)
        {
            lastSelectedButton.image.sprite = defaultButtonImage;
        }

        // Set the new selected button's image
        selectedAttackType = attackType;
        button.image.sprite = selectedButtonImage;

        // Show the instantiate button when an attack type is selected
        instantiateAttackButton.gameObject.SetActive(true);

        // Update the last selected button reference
        lastSelectedButton = button;
    }

    public void Instantiate_Attack_Button()
    {
        if (heroAttackScript != null && selectedAttackType != AttackType.None)
        {
            // Pass both the selected attack type and the current element type to determine which attack to instantiate
            heroAttackScript.InstantiateSelectedAttack(selectedAttackType, currentElementType);
            instantiateAttackButton.gameObject.SetActive(false); // Hide the button after an attack is instantiated
            PlayCloseAttackUIAnimation();
        }
        else
        {
            Debug.LogError("Hero_Attack_Script not found or no attack type selected!");
        }
    }

    public void SetCurrentElementType(Skill_Pick_Up_Script.ElementType elementType)
    {
        currentElementType = elementType;
    }

    private void PlayCloseAttackUIAnimation()
    {
        if (attackUIAnimator != null)
        {
            attackUIAnimator.Play("Close_Attack_UI"); // Play the close animation
            StartCoroutine(HideUIAfterDelay(0.5f)); // Hide the UI after 0.5 seconds
        }
        else
        {
            Debug.LogError("Animator for attackUI not found!");
        }
    }

    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideAttackUI();
    }
}
