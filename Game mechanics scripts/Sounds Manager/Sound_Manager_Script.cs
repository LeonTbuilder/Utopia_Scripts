using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound_Manager_Script : MonoBehaviour
    {
        [SerializeField] private SoundsSO SO;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup musicMixerGroup; // Separate mixer group for music
        [SerializeField] private AudioMixerGroup sfxMixerGroup;   // Separate mixer group for SFX

        private static Sound_Manager_Script instance = null;
        private AudioSource musicAudioSource;
        private AudioSource sfxAudioSource;

        private float musicVolume = 1f;
        private float sfxVolume = 1f;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                AudioSource[] audioSources = GetComponents<AudioSource>();
                if (audioSources.Length < 2)
                {
                    musicAudioSource = gameObject.AddComponent<AudioSource>();
                    sfxAudioSource = gameObject.AddComponent<AudioSource>();
                }
                else
                {
                    musicAudioSource = audioSources[0];
                    sfxAudioSource = audioSources[1];
                }
                DontDestroyOnLoad(gameObject);

                // Assign the correct mixer groups
                musicAudioSource.outputAudioMixerGroup = musicMixerGroup;
                sfxAudioSource.outputAudioMixerGroup = sfxMixerGroup;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            Game_Manager_Script.OnGameWin += PlayWinSound;
            Game_Manager_Script.OnGameOver += PlayLoseSound;
        }

        private void OnDisable()
        {
            Game_Manager_Script.OnGameWin -= PlayWinSound;
            Game_Manager_Script.OnGameOver -= PlayLoseSound;
        }

        public static AudioSource Play_Hero_Sound(Hero_SoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.sounds_Hero[(int)sound], source, volume * instance.sfxVolume, instance.sfxMixerGroup);
        }

        public static AudioSource Play_Object_Sound(Objects_SoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.sounds_Objects[(int)sound], source, volume * instance.sfxVolume, instance.sfxMixerGroup);
        }

        public static AudioSource Play_Win_Lose_Sound(Win_LoseSoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.sounds_Win_Lose[(int)sound], instance.musicAudioSource, volume * instance.musicVolume, instance.musicMixerGroup);
        }

        public static AudioSource Play_Menu_Sound(Menu_SoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.sounds_Menu[(int)sound], source, volume * instance.musicVolume, instance.musicMixerGroup);
        }

        public static AudioSource Play_Game_SoundTrack(MainGame_SoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.sounds_Game_Music[(int)sound], instance.musicAudioSource, volume * instance.musicVolume, instance.musicMixerGroup);
        }

        public static AudioSource Play_UI_Sound(UI_SoundType sound, AudioSource source = null, float volume = 1)
        {
            return PlaySound(instance.SO.Ui_Sound[(int)sound], source, volume * instance.sfxVolume, instance.sfxMixerGroup);
        }

        private static AudioSource PlaySound(SoundList soundList, AudioSource source, float volume, AudioMixerGroup mixerGroup)
        {
            if (soundList.sounds == null || soundList.sounds.Length == 0)
            {
                Debug.LogWarning("No sounds found in the sound list.");
                return null;
            }

            AudioClip[] clips = soundList.sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

            if (source)
            {
                source.outputAudioMixerGroup = mixerGroup; // Assign the correct mixer group
                source.clip = randomClip;
                source.volume = volume * soundList.volume;
                source.loop = soundList.loop;
                source.Play();
            }
            else
            {
                instance.sfxAudioSource.outputAudioMixerGroup = mixerGroup;
                instance.sfxAudioSource.loop = soundList.loop;
                if (soundList.loop)
                {
                    instance.sfxAudioSource.clip = randomClip;
                    instance.sfxAudioSource.volume = volume * soundList.volume;
                    instance.sfxAudioSource.Play();
                }
                else
                {
                    instance.sfxAudioSource.PlayOneShot(randomClip, volume * soundList.volume);
                }
                source = instance.sfxAudioSource;
            }

            return source;
        }

        public static void StopSound(AudioSource source)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }

        private void PlayWinSound()
        {
            musicAudioSource.Stop();
            Play_Win_Lose_Sound(Win_LoseSoundType.Win_Music);
        }

        private void PlayLoseSound()
        {
            musicAudioSource.Stop();
            Play_Win_Lose_Sound(Win_LoseSoundType.Lose_Music);
        }

        public static void StopAllSounds()
        {
            instance.musicAudioSource.Stop();
            instance.sfxAudioSource.Stop();
        }

        public static void SetMusicVolume(float volume)
        {
            instance.musicVolume = volume;
            instance.audioMixer.SetFloat("MyExposedParam", Mathf.Log10(volume) * 20);
        }

        public static void SetSfxVolume(float volume)
        {
            instance.sfxVolume = volume;
            instance.audioMixer.SetFloat("MyExposedParam 1", Mathf.Log10(volume) * 20);
        }
    }
}
