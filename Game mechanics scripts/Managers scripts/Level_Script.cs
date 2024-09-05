using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Script : MonoBehaviour
{
    [HideInInspector]
    public Hero_Script _hero;

    public static Level_Script _instance;

    [SerializeField]
    private Transform upperBound;

    [SerializeField]
    private Transform lowerBound;

    bool isLoadedHero;

    public enum Type_ofLEVEL
    {
        GEMS,
        CHEST,
        ENEMY,
        BUDDY,
        BOSS
    };

    public Type_ofLEVEL levelType; // Add this line to specify the level type for each level

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        isLoadedHero = false;
    }

    private void Start()
    {
        NotifyLevelBounds();
    }

    private void NotifyLevelBounds()
    {
        if (upperBound != null && lowerBound != null)
        {
            Level_Scroller.Instance.SetBounds(upperBound, lowerBound);
        }
        else
        {
            Debug.LogError("UpperBound and/or LowerBound objects not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (!isLoadedHero && _hero == null)
        {
            GameObject heroObject = GameObject.FindGameObjectWithTag("Hero");
            if (heroObject != null)
            {
                _hero = heroObject.GetComponent<Hero_Script>();
                isLoadedHero = true;
            }
        }
    }
}
