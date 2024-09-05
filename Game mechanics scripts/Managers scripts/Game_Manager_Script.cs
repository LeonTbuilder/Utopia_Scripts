using System;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;

public class Game_Manager_Script : MonoBehaviour
{
    public static Game_Manager_Script instance;

    [Header("Bools")]
    public bool isGameOver, isGameWin = false;

    [Header("Objectives Counter")]
    public int gems_Current, gems_Bonus, total_Energy, total_Enemies, total_Enemys_Killed;

    [Header("Enemy Tracking")]
    public List<GameObject> enemiesList = new List<GameObject>();

    public static event Action OnGameWin;
    public static event Action OnGameOver;

    private Stars_Leaderboard_Script starsLeaderboard;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializeGame();

        Application.targetFrameRate = 60;

        starsLeaderboard = FindFirstObjectByType<Stars_Leaderboard_Script>();
    }

    private void InitializeGame()
    {
        isGameOver = false;
        isGameWin = false;
        gems_Bonus = 0;
        gems_Current = PlayerPrefs.GetInt("Gem");
        total_Energy = PlayerPrefs.GetInt("Energy");

        Reset_EnemyCollision();
    }

    public void Game_is_Over()
    {
        if (isGameOver) return;

        isGameOver = true;

        UI_inGame_Manager_Script._instance?.Show_Game_Over();

        StopBackgroundMusic();
        OnGameOver?.Invoke();
    }

    public void Game_is_Win()
    {
        if (isGameWin) return;

        isGameWin = true;
        StopBackgroundMusic();

        UI_inGame_Manager_Script._instance?.Show_Game_Clear();

        OnGameWin?.Invoke();

        AwardStar();

        PlayerPrefs.SetInt("LevelsPassed", PlayerPrefs.GetInt("LevelsPassed", 0) + 1);
    }

    private void AwardStar()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        string starKey = $"Level{currentLevel}_LeftStar";
        if (PlayerPrefs.GetInt(starKey) == 0)
        {
            PlayerPrefs.SetInt(starKey, 1);
            int totalStars = PlayerPrefs.GetInt("Stars") + 1;
            PlayerPrefs.SetInt("Stars", totalStars);

            starsLeaderboard?.UpdateStarsText(totalStars);
        }
    }

    public void Game_is_Reset()
    {
        InitializeGame();
    }

    public void Reset_EnemyCollision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Enemy_Layer"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Magma_Layer"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Poison_Layer"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hero_Layer"), LayerMask.NameToLayer("Gas_Layer"), false);
    }

    public void Add_Gems(int add)
    {
        gems_Current += add;
        UI_inGame_Manager_Script._instance?.ShowGemsText(gems_Current);
    }

    public void Enemy_Killed_inLevelType(GameObject enemy)
    {
        total_Enemys_Killed++;
        enemiesList.Remove(enemy);
        Check_Enemy_Win_Condition();
        Reset_EnemyCollision();
    }

    private void Check_Enemy_Win_Condition()
    {
        if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.ENEMY && enemiesList.Count == 0)
        {
            Game_is_Win();
            FindFirstObjectByType<Hero_Script>()?.Handle_Win_State();
        }
    }

    public void InitializeEnemiesList()
    {
        enemiesList.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        total_Enemies = enemies.Length;
        enemiesList.AddRange(enemies);
    }

    public void StartBackgroundMusic()
    {
        Sound_Manager_Script.Play_Game_SoundTrack(MainGame_SoundType.Soundtrack, null, 1);
    }

    public void StopBackgroundMusic()
    {
        Sound_Manager_Script.StopAllSounds();
    }
}
