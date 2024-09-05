using System.Collections;
using UnityEngine;
using SmallHedge.SoundManager;
using System;

public class Poison_Spawner_Script : MonoBehaviour
{
    public GameObject Poison_PF;
    public int Poison_Count;
    public float x_Box = 0.5f;
    public float y_Box = 0.5f;

    GameObject[] Poison_List;

    private AudioSource poisonAudioSource;

    void Start()
    {
        Poison_List = new GameObject[Poison_Count];
        poisonAudioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < Poison_List.Length; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + UnityEngine.Random.Range(-x_Box, x_Box),
                                      transform.position.y + UnityEngine.Random.Range(-y_Box, y_Box),
                                      9.0f);  // Set Z position to 9
            Poison_List[i] = Instantiate(Poison_PF, pos, Quaternion.identity);
            Poison_List[i].layer = Poison_List[0].layer;
            Poison_List[i].transform.SetParent(transform.parent);
            Poison_List[i].GetComponent<Poison_Particles_Script>().OnPoisonTransform += HandlePoisonTransform;
        }

        Play_Poison_Sound();
    }

    void HandlePoisonTransform(GameObject destroyedPoison)
    {
        StartCoroutine(TransformRemainingPoisonToGas());
    }

    IEnumerator TransformRemainingPoisonToGas()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (GameObject poison in Poison_List)
        {
            if (poison != null && poison.CompareTag("Poison"))
            {
                poison.GetComponent<Poison_Particles_Script>().Replace_With_SmokeGas();
            }
        }
        CheckForRemainingPoison();
    }

    void Play_Poison_Sound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Poison, poisonAudioSource);
    }

    void Stop_Poison_Sound()
    {
        poisonAudioSource.Stop();
    }

    void CheckForRemainingPoison()
    {
        bool anyPoisonLeft = false;
        foreach (GameObject poison in Poison_List)
        {
            if (poison != null && poison.CompareTag("Poison"))
            {
                anyPoisonLeft = true;
                break;
            }
        }

        if (!anyPoisonLeft)
        {
            Stop_Poison_Sound();
        }
    }

    // Method to handle collision with lava
    public void HandleLavaCollision(GameObject lava)
    {
        StartCoroutine(DestroyRemainingPoison());
    }

    IEnumerator DestroyRemainingPoison()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (GameObject poison in Poison_List)
        {
            if (poison != null && poison.CompareTag("Poison"))
            {
                Destroy(poison);
            }
        }
        CheckForRemainingPoison();
    }

    // Method to add a new poison particle to the list
    public void AddPoisonParticle(GameObject poison)
    {
        // Resize the array to accommodate the new particle
        Array.Resize(ref Poison_List, Poison_List.Length + 1);
        Poison_List[Poison_List.Length - 1] = poison;
    }
}
