using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TutorialStep
{
    public GameObject targetObject;
    public string text;
    public bool showHand;
}

public class Tutorial_Level__Script : MonoBehaviour
{
    public static Tutorial_Level__Script Instance { get; private set; }

    [SerializeField]
    public List<TutorialStep> steps = new List<TutorialStep>();

    public delegate void TutorialStepsLoaded(List<TutorialStep> tutorialSteps);
    public event TutorialStepsLoaded OnTutorialStepsLoaded;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (OnTutorialStepsLoaded != null)
        {
            OnTutorialStepsLoaded.Invoke(steps);
        }
    }
}
