using UnityEngine;
using System.Collections;
using SmallHedge.SoundManager;

public class Lava_Spawner_Script : MonoBehaviour
{
    public GameObject lava_PF;
    public GameObject lavaRock_PF; // Reference to the rock prefab
    public int lave_Count;
    public float x_Box = 0.5f;
    public float y_Box = 0.5f;
    GameObject[] Lava_List;

    private AudioSource lavaAudioSource;
    private AudioSource boilingAudioSource;
    private AudioSource steamAudioSource;
    private AudioSource alchemyAudioSource; // Reference to the alchemy sound

    private int totalTransformedLava = 0; // Track the total number of transformed lava particles
    private bool isTransforming = false; // Flag to prevent multiple coroutines from running concurrently

    void Start()
    {
        Lava_List = new GameObject[lave_Count];
        lavaAudioSource = gameObject.AddComponent<AudioSource>();
        boilingAudioSource = gameObject.AddComponent<AudioSource>();
        steamAudioSource = gameObject.AddComponent<AudioSource>();
        alchemyAudioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < Lava_List.Length; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-x_Box, x_Box),
                                      transform.position.y + Random.Range(-y_Box, y_Box),
                                      9.0f);  // Set Z position to 9
            Lava_List[i] = Instantiate(lava_PF, pos, Quaternion.identity);
            Lava_List[i].layer = Lava_List[0].layer;
            Lava_List[i].transform.SetParent(transform.parent);
            Lava_List[i].GetComponent<Lava_Particles_Script>().OnLavaRockTransform += HandleLavaRockTransform;
            Lava_List[i].GetComponent<Lava_Particles_Script>().OnGasTransform += HandleGasTransform;
            Lava_List[i].GetComponent<Lava_Particles_Script>().OnWaterCollision += HandleWaterCollision;
            Lava_List[i].GetComponent<Lava_Particles_Script>().OnPoisonCollision += HandlePoisonCollision;
        }

        Play_Lava_Sound();
    }

    void HandleLavaRockTransform(GameObject lava)
    {
        StartCoroutine(Transform_Lava_toLavaRock());
    }

    void HandleGasTransform(GameObject lava)
    {
        StartCoroutine(Transform_Remaining_Lava_toGas());
    }

    void HandleWaterCollision(GameObject lava)
    {
        Stop_Lava_Sound();
        Play_Boiling_Sound();
        StartCoroutine(Play_Steam_Sound(1f));
    }

    void HandlePoisonCollision(GameObject lava)
    {
        Play_Alchemy_Sound();
        Play_Steam_Sound();
    }

    IEnumerator Transform_Lava_toLavaRock()
    {
        if (isTransforming) yield break; // Prevent multiple coroutines from running concurrently
        isTransforming = true; // Set flag to true to indicate coroutine is running

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject lava in Lava_List)
        {
            if (lava != null && lava.CompareTag("Lava"))
            {
                lava.GetComponent<Lava_Particles_Script>().Replace_To_LavaRock();
                totalTransformedLava++;

                // Check if two particles have been transformed
                if (totalTransformedLava % 2 == 0)
                {
                    Vector3 pos = lava.transform.position;
                    GameObject lavaRock = Instantiate(lavaRock_PF, pos, Quaternion.identity);
                    lavaRock.transform.SetParent(this.transform);
                }
            }
        }

        isTransforming = false; // Reset the flag after the coroutine completes
    }

    IEnumerator Transform_Remaining_Lava_toGas()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (GameObject lava in Lava_List)
        {
            if (lava != null && lava.CompareTag("Lava"))
            {
                lava.GetComponent<Lava_Particles_Script>().Replace_To_Gas();
            }
        }
    }

    void Play_Lava_Sound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Lava, lavaAudioSource);
    }

    void Stop_Lava_Sound()
    {
        lavaAudioSource.Stop();
    }

    void Play_Boiling_Sound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Lava_Boiling, boilingAudioSource);
    }

    void Play_Alchemy_Sound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Alcamy, alchemyAudioSource);
    }

    void Play_Steam_Sound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Steam, steamAudioSource);
    }

    IEnumerator Play_Steam_Sound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Play_Steam_Sound();
    }
}
