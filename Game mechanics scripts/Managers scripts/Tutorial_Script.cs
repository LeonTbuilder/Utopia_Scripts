using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class Tutorial_Script : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public GameObject targetObject;
        public string text;
        public bool showHand;
    }

    public List<TutorialStep> steps = new List<TutorialStep>();
    public GameObject circleMask;
    public TMP_Text tutorialText;
    public GameObject handUIPrefab;

    private int currentStepIndex = 0;
    private GameObject currentHandUI;

    void Start()
    {
        ShowStep(0);
    }

    void Update()
    {
        if (steps.Count == 0) return;

        if (steps[currentStepIndex].targetObject == null)
        {
            NextStep();
        }
    }

    void ShowStep(int index)
    {
        if (index >= steps.Count)
        {
            EndTutorial();
            return;
        }

        TutorialStep step = steps[index];
        if (step.targetObject != null)
        {
            circleMask.transform.position = step.targetObject.transform.position;
            RectTransform targetRect = step.targetObject.GetComponent<RectTransform>();
            circleMask.GetComponent<RectTransform>().sizeDelta = new Vector2(targetRect.rect.width, targetRect.rect.height);
            tutorialText.text = step.text;

            if (step.showHand)
            {
                if (currentHandUI == null)
                {
                    currentHandUI = Instantiate(handUIPrefab, circleMask.transform);
                }
                currentHandUI.transform.position = step.targetObject.transform.position;
            }
            else if (currentHandUI != null)
            {
                Destroy(currentHandUI);
            }
        }
    }

    public void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex < steps.Count)
        {
            ShowStep(currentStepIndex);
        }
        else
        {
            EndTutorial();
        }
    }

    void EndTutorial()
    {
        circleMask.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        if (currentHandUI != null)
        {
            Destroy(currentHandUI);
        }
    }
}