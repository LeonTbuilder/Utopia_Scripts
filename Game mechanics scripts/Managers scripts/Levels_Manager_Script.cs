using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SmallHedge.SoundManager;

public class Levels_Manager_Script : MonoBehaviour
{
    [Header("Level Info")]
    GameObject lvl_Current_Obj;
    public int lvl_Current;

    [Header("Backgrounds")]
    public GameObject bg_Forest, bg_Water, bg_Volcano, bg_Fantasy;

    public static Levels_Manager_Script _instance;

    public static event Action OnLevelLoaded; // Event to signal when a level is loaded
    public static event Action OnLevelExited; // Event to signal when a level is exited

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        LoadLevel();
    }

    public void LoadLevel()
    {
        Sound_Manager_Script.StopAllSounds();

        lvl_Current = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (lvl_Current == 0)
        {
            lvl_Current = 1;
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }

        string levelPath = $"Levels/Level {lvl_Current}";

        GameObject levelPrefab = Resources.Load<GameObject>(levelPath);

        if (levelPrefab == null) return;

        lvl_Current_Obj = Instantiate(levelPrefab);

        Game_Manager_Script.instance.InitializeEnemiesList();

        OnLevelLoaded?.Invoke();

        Sound_Manager_Script.Play_Game_SoundTrack(MainGame_SoundType.Soundtrack);
    }

    public Level_Script.Type_ofLEVEL GetCurrentLevelType()
    {
        if (lvl_Current_Obj != null)
        {
            var levelScript = lvl_Current_Obj.GetComponent<Level_Script>();
            if (levelScript != null)
            {
                return levelScript.levelType;
            }
        }
        return Level_Script.Type_ofLEVEL.GEMS;
    }

    public void CleanLevel()
    {
        if (lvl_Current_Obj != null)
        {
            Destroy(lvl_Current_Obj);
            lvl_Current_Obj = null;
        }
        Level_Pool_Manager._instance?.Clean_All_Objects();

        OnLevelExited?.Invoke();

        Sound_Manager_Script.StopAllSounds();
    }
}
