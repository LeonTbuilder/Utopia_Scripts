using UnityEngine;

namespace SmallHedge.SoundManager
{
    public class Hero_Play_SoundExit : StateMachineBehaviour
    {
        [SerializeField] private Hero_SoundType sound;
        [SerializeField, Range(0, 1)] private float volume = 1;
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Sound_Manager_Script.Play_Hero_Sound(sound, null, volume);
        }
    }
}
