using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero_Menu_Position_Script : MonoBehaviour
{
    public static Hero_Menu_Position_Script _instance;

    public GameObject heroPrefab;
    private GameObject instantiatedHero;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main_Menu")
        {
            Instantiated_Hero();
        }
    }

    public void Instantiated_Hero()
    {
        if (instantiatedHero == null && heroPrefab != null)
        {
            instantiatedHero = Instantiate(heroPrefab, transform);
            instantiatedHero.transform.localPosition = Vector3.zero;
        }
    }

    public void DestroyHero()
    {
        if (instantiatedHero != null)
        {
            Destroy(instantiatedHero);
            instantiatedHero = null;
        }
    }
}
