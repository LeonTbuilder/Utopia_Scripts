using UnityEngine;

namespace SmallHedge.SoundManager
{
    public class Hero_Play_SoundEnter : StateMachineBehaviour
    {
        [SerializeField] private Hero_SoundType sound;
        [SerializeField, Range(0, 1)] private float volume = 1;
        private AudioSource audioSource;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            audioSource = Sound_Manager_Script.Play_Hero_Sound(sound, null, volume);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Sound_Manager_Script.StopSound(audioSource);
        }
    }
}
