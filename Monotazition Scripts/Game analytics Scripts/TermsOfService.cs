using UnityEngine;
using UnityEngine.UI;

public class TermsOfService : MonoBehaviour
{
    public GameObject termsPanel;
    public Button acceptButton;
    public Button backButton;
    public GameObject checkMark;

    void Start()
    {
        if (PlayerPrefs.GetInt("TermsAccepted", 0) == 1)
        {
            
            checkMark.SetActive(true);
            backButton.interactable = true;
            acceptButton.interactable = false;
            termsPanel.SetActive(false); 
        }
        else
        {
            
            checkMark.SetActive(false);
            backButton.interactable = false;
            termsPanel.SetActive(true); 
        }

        acceptButton.onClick.AddListener(AcceptTerms);
        backButton.onClick.AddListener(Back);
    }

    public void ShowTerms()
    {
        termsPanel.SetActive(true);
    }

    public void AcceptTerms()
    {

        PlayerPrefs.SetInt("TermsAccepted", 1);
        PlayerPrefs.Save(); 
        checkMark.SetActive(true); 
        backButton.interactable = true;
        acceptButton.interactable = false; 
    }

    public void Back()
    {

        if (PlayerPrefs.GetInt("TermsAccepted", 0) == 1)
        {
            termsPanel.SetActive(false);
            Main_Menu_Manager_Script._instance.BG_Mask.SetActive(false);
        }
    }
}
