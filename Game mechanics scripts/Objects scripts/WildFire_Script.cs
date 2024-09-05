using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallHedge.SoundManager;

public class WildFire_Script : MonoBehaviour
{
    public int waterCollisionCount = 0;
    private AudioSource steamAudioSource;

    void Start()
    {
        steamAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            waterCollisionCount++;

            if (waterCollisionCount >= 4)
            {
                
                Destroy(gameObject, 0.5f);
                Destroy(other);

                Sound_Manager_Script.Play_Object_Sound(Objects_SoundType.O_Steam, steamAudioSource);


            }
        }
    }
}
