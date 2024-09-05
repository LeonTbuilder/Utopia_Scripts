using System.Collections;
using UnityEngine;
using SmallHedge.SoundManager;

public class Water_Spawner_Script : MonoBehaviour
{
    public GameObject water_PF;
    public int water_Count;
    public float x_Box = 0.5f;
    public float y_Box = 0.5f;
    GameObject[] Water_List;

    private AudioSource movingWaterAudioSource;
    private AudioSource poisonBoilingAudioSource;

    void Start()
    {
        Water_List = new GameObject[water_Count];
        movingWaterAudioSource = gameObject.AddComponent<AudioSource>();
        poisonBoilingAudioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < Water_List.Length; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-x_Box, x_Box),
                                      transform.position.y + Random.Range(-y_Box, y_Box),
                                      9.0f);  // Set Z position to 9
            Water_List[i] = Instantiate(water_PF, pos, Quaternion.identity);
            Water_List[i].layer = Water_List[0].layer;
            Water_List[i].transform.SetParent(transform.parent);

            Water_List[i].GetComponent<Water_Particles_Script>().OnPoisonCollision += Poison_Collision;
            Water_List[i].GetComponent<Water_Particles_Script>().OnMovement += Handle_Movement;
        }

        Play_Moving_WaterSound();
    }

    void Poison_Collision(GameObject water)
    {
        Play_PoisonBoilingSound();
    }

    void Handle_Movement(GameObject water)
    {
        Play_Moving_WaterSound();
    }

    void Play_Moving_WaterSound()
    {
        if (!movingWaterAudioSource.isPlaying)
        {
            Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Moving_Water, movingWaterAudioSource);
        }
    }

    void Play_PoisonBoilingSound()
    {
        Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Poison_Boiling, poisonBoilingAudioSource);
    }
}
