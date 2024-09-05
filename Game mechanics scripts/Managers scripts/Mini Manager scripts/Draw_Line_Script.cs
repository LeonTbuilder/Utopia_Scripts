using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLineScript : MonoBehaviour
{
    /*public GameObject fairy_Prefab;
    int pool_Size = 2;

    private Queue<GameObject> fairy_Pool;
    private int current_Waypoint = 0;
    private GameObject current_Fairy;

    private Hero_Script hero_Script;
    private Coroutine fairy_Coroutine;
    private bool game_Won = false;
    private bool game_Lost = false;

    float speed = 0.7f;
    float waypoint_Threshold = 0.1f;

    void Start()
    {
        hero_Script = GetComponent<Hero_Script>();

        // Initialize fairy pool
        fairy_Pool = new Queue<GameObject>();
        for (int i = 0; i < pool_Size; i++)
        {
            GameObject fairy = Instantiate(fairy_Prefab);
            fairy.transform.SetParent(transform); // Set the parent immediately after instantiation
            fairy.SetActive(false);
            fairy_Pool.Enqueue(fairy);
        }

        StartCoroutine(Initialize_Fairy_Delay(2.0f));
        Game_Manager_Script.OnGameWin += OnGame_Win;
        Game_Manager_Script.OnGameOver += OnGame_Over;
    }

    void Update()
    {
        // If the game is won or lost, stop further processing
        if (game_Won || game_Lost || hero_Script == null || hero_Script.path == null || current_Fairy == null)
        {
            return;
        }

        if (current_Waypoint >= hero_Script.path.vectorPath.Count)
        {
            StartCoroutine(Respawn_Fairy_Delay(2.0f));
            current_Waypoint = 0;
            return;
        }

        Vector3 direction = ((Vector3)hero_Script.path.vectorPath[current_Waypoint] - current_Fairy.transform.position).normalized;
        current_Fairy.transform.position += direction * Time.deltaTime * speed;

        if (Vector3.Distance(current_Fairy.transform.position, hero_Script.path.vectorPath[current_Waypoint]) < waypoint_Threshold)
        {
            current_Waypoint++;
        }
    }

    IEnumerator Initialize_Fairy_Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!game_Won && !game_Lost)
        {
            Vector3 startPosition = transform.position + new Vector3(0, 1, 0);
            current_Fairy = Get_Fairy_From_Pool(startPosition);
            if (current_Fairy != null)
            {
                fairy_Coroutine = StartCoroutine(Fairy_Movement());
            }
        }
    }

    IEnumerator Respawn_Fairy_Delay(float delay)
    {
        if (current_Fairy != null)
        {
            Return_Fairy_toPool(current_Fairy);
        }
        yield return new WaitForSeconds(delay);
        if (!game_Won && !game_Lost)
        {
            current_Fairy = Get_Fairy_From_Pool(transform.position);
        }
    }

    IEnumerator Fairy_Movement()
    {
        while (!game_Won && !game_Lost)
        {
            if (current_Fairy != null && hero_Script != null && hero_Script.path != null)
            {
                if (current_Waypoint >= hero_Script.path.vectorPath.Count)
                {
                    Return_Fairy_toPool(current_Fairy);
                    current_Waypoint = 0;
                    yield break;
                }

                Vector3 direction = ((Vector3)hero_Script.path.vectorPath[current_Waypoint] - current_Fairy.transform.position).normalized;
                current_Fairy.transform.position += direction * Time.deltaTime * speed;

                if (Vector3.Distance(current_Fairy.transform.position, hero_Script.path.vectorPath[current_Waypoint]) < waypoint_Threshold)
                {
                    current_Waypoint++;
                }
            }
            else
            {
                if (current_Fairy != null)
                {
                    Return_Fairy_toPool(current_Fairy);
                }
                yield break;
            }
            yield return null;
        }
    }

    GameObject Get_Fairy_From_Pool(Vector3 position)
    {
        GameObject fairy;
        if (fairy_Pool.Count > 0)
        {
            fairy = fairy_Pool.Dequeue();
        }
        else
        {
            fairy = Instantiate(fairy_Prefab);
            fairy.transform.SetParent(transform); // Set the parent immediately after instantiation if a new fairy is created
        }

        fairy.transform.position = position;
        fairy.SetActive(true);
        return fairy;
    }

    void Return_Fairy_toPool(GameObject fairy)
    {
        if (fairy != null)
        {
            fairy.SetActive(false);
            fairy.transform.SetParent(transform); // Ensure the fairy stays in the same hierarchy when returning to the pool
            fairy_Pool.Enqueue(fairy);
        }
    }

    void OnDestroy()
    {
        CleanUp_Fairies();
    }

    void OnGame_Win()
    {
        game_Won = true;
        CleanUp_Fairies();
    }

    void OnGame_Over()
    {
        game_Lost = true;
        CleanUp_Fairies();
    }

    void CleanUp_Fairies()
    {
        if (this == null) return;

        if (fairy_Coroutine != null)
        {
            StopCoroutine(fairy_Coroutine);
            fairy_Coroutine = null;
        }

        while (fairy_Pool.Count > 0)
        {
            GameObject fairy = fairy_Pool.Dequeue();
            Destroy(fairy);
        }

        if (current_Fairy != null)
        {
            Destroy(current_Fairy);
            current_Fairy = null;
        }
    }*/
}
