using UnityEngine;
using UnityEngine.UI;

public class Hero_Life_Manager_Script : MonoBehaviour
{
    [Header("UI Components")]
    public Image[] hearts;

    [Header("Hero Reference")]
    Hero_Script heroScript;

    [Header("Puff Prefab")]
    public GameObject puffPrefab;

    private int currentLife;
    private int maxLife;

    private void OnEnable()
    {
        Levels_Manager_Script.OnLevelLoaded += ResetLife;
        Levels_Manager_Script.OnLevelExited += ResetLife;
    }

    private void OnDisable()
    {
        Levels_Manager_Script.OnLevelLoaded -= ResetLife;
        Levels_Manager_Script.OnLevelExited -= ResetLife;
    }

    private void Start()
    {
        heroScript = FindFirstObjectByType<Hero_Script>();
        maxLife = hearts.Length;
        currentLife = maxLife;
        UpdateHeartUI();
    }

    public void LowerLife()
    {

        heroScript.Trigger_Hit_Animation();

        if (currentLife > 0)
        {
            currentLife--;


            Vector3 puffPosition = hearts[currentLife].transform.position;
            Transform parentTransform = hearts[currentLife].transform.parent;

            GameObject puff = Instantiate(puffPrefab, puffPosition, Quaternion.identity, parentTransform);

            puff.transform.localPosition = hearts[currentLife].transform.localPosition;

            UpdateHeartUI();

            if (currentLife <= 0)
            {
                heroScript.Hero_isDead();
            }
        }
    }

    public void LowerAllHeartsWithoutDeath()
    {
        while (currentLife > 0)
        {
            currentLife--;
            Vector3 puffPosition = hearts[currentLife].transform.position;
            Transform parentTransform = hearts[currentLife].transform.parent;

            GameObject puff = Instantiate(puffPrefab, puffPosition, Quaternion.identity, parentTransform);

            puff.transform.localPosition = hearts[currentLife].transform.localPosition;

            hearts[currentLife].gameObject.SetActive(false);
        }
    }

    public void ResetLife()
    {
        currentLife = maxLife;
        UpdateHeartUI();
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < currentLife);
        }
    }
}
