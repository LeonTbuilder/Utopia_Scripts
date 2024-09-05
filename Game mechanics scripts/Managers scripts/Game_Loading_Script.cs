using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Loading_Script : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        StartCoroutine(Load_Game());
    }

    IEnumerator Load_Game()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main_Menu");
    }
}
