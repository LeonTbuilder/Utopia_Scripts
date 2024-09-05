using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    [CreateAssetMenu(menuName = "Sound Manager SO/Sounds SO", fileName = "Sounds SO")]
    public class SoundsSO : ScriptableObject
    {
        public SoundList[] sounds_Hero;

        public SoundList[] sounds_Objects;

        public SoundList[] sounds_Win_Lose;

        public SoundList[] sounds_Menu;

        public SoundList[] sounds_Game_Music;

        public SoundList[] Ui_Sound;
    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public bool loop;
        public AudioClip[] sounds;
    }
}
