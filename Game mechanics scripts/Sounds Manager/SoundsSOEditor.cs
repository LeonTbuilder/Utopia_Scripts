#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    [CustomEditor(typeof(SoundsSO))]
    public class SoundsSOEditor : Editor
    {
        private void OnEnable()
        {
            ref SoundList[] sound_Hero_List = ref ((SoundsSO)target).sounds_Hero;
            ref SoundList[] sound_Objects_List = ref ((SoundsSO)target).sounds_Objects;
            ref SoundList[] sound_Win_Lose_List = ref ((SoundsSO)target).sounds_Win_Lose;
            ref SoundList[] menu_SoundList = ref ((SoundsSO)target).sounds_Menu;
            ref SoundList[] game_Music_List = ref ((SoundsSO)target).sounds_Game_Music;
            ref SoundList[] sound_UI_List = ref ((SoundsSO)target).Ui_Sound;


            InitializeSoundList(ref sound_Hero_List, typeof(Hero_SoundType));
            InitializeSoundList(ref sound_Objects_List, typeof(Objects_SoundType));
            InitializeSoundList(ref sound_Win_Lose_List, typeof(Win_LoseSoundType));
            InitializeSoundList(ref menu_SoundList, typeof(Menu_SoundType));
            InitializeSoundList(ref game_Music_List, typeof(MainGame_SoundType));
            InitializeSoundList(ref sound_UI_List, typeof(UI_SoundType));


        }

        private void InitializeSoundList(ref SoundList[] soundList, Type enumType)
        {
            if (soundList == null)
                soundList = new SoundList[0];

            string[] names = Enum.GetNames(enumType);
            bool differentSize = names.Length != soundList.Length;

            Dictionary<string, SoundList> sounds = new Dictionary<string, SoundList>();

            if (differentSize)
            {
                for (int i = 0; i < soundList.Length; ++i)
                {
                    if (!sounds.ContainsKey(soundList[i].name))
                    {
                        sounds.Add(soundList[i].name, soundList[i]);
                    }
                }
            }

            Array.Resize(ref soundList, names.Length);
            for (int i = 0; i < soundList.Length; i++)
            {
                string currentName = names[i];
                soundList[i].name = currentName;
                if (soundList[i].volume == 0) soundList[i].volume = 1;

                if (differentSize)
                {
                    if (sounds.ContainsKey(currentName))
                    {
                        SoundList current = sounds[currentName];
                        UpdateElement(ref soundList[i], current.volume, current.sounds, current.mixer, current.loop);
                    }
                    else
                    {
                        UpdateElement(ref soundList[i], 1, new AudioClip[0], null, false);
                    }
                }
            }
        }

        private static void UpdateElement(ref SoundList element, float volume, AudioClip[] sounds, AudioMixerGroup mixer, bool loop)
        {
            element.volume = volume;
            element.sounds = sounds;
            element.mixer = mixer;
            element.loop = loop;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
