using UnityEngine;

namespace SmallHedge.SoundManager
{
    public class SoundValues : MonoBehaviour
    {
        public AudioClip[] mainSoundTracks;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

    }

    public enum Hero_SoundType
    {
        WALK,

        RUNNING,

        JUMP,

        LAND,

        SKILL_PICKUP,

        FIRE_ATTACK,

        WATER_ATTACK,

        AIR_ATTACK,

        EERTH_ATTACK,

        ELECTRIC_ATTACK,

        HIT,

        Win,

        Lose,

        HIT_BLAST,

    }

    public enum Objects_SoundType
    {
        // Portal
        O_WARP_IN,

        O_WARP_OUT,

        //Lava
        O_Lava,

        O_Steam,

        O_Lava_Boiling,

        O_Alcamy,

        //Poison
        O_Poison,

        //Water
        O_Moving_Water,

        O_Poison_Boiling,

        // Pin
        O_Pin_Swoosh,

        // Burning
        O_Fire_Burning,

        //Boulder
        O_Rock_Moving

    }

    public enum Win_LoseSoundType
    {
        Win_Music,

        Lose_Music
    }

    public enum Menu_SoundType
    {
        Soundtrack,

    }

    public enum MainGame_SoundType
    {
        Soundtrack,

    }

    public enum UI_SoundType
    {
        Open_Panel_Click,
        Close_Panel_Click,
        Level_Click,
        Collect_Rewars_Click,

    }
}
