using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
namespace SGT_Tools.Audio
{

public class SGT_GroundChecker : MonoBehaviour
{
        string layername;
        public LayerMask layers;
        public List<string> GroundName = new List<string>();
        public List<AudioClip> GroundAudioClip = new List<AudioClip>();

        public int _rayCastDistance;

        public AudioMixerGroup MixerGroup;
        public AudioSource _AudioSource;
        private float PitchInput;
        public bool DisableComponent = false;
        [DrawWithUnity]
        [FoldoutGroup("Events")]
        [PropertyOrder(1)]
        public UnityEvent Stepped;
    

    private void Start()
    {
        _AudioSource = GetComponent<AudioSource>();
        _AudioSource.outputAudioMixerGroup = MixerGroup;

    }



    //Bu fonklsyion walking input'undan çalışır value değiştikçe çalışır. Locomotion yürümede...
    public void CheckGroundSound(float ControllerInput)
    {
            if (!DisableComponent)
            {

            
        
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, _rayCastDistance, layers))
                {

                    if (ControllerInput > 0)
                    {
                        PitchInput = SGT_Math.Remap(ControllerInput, 0, 1, 1, 2.8f);
                    }
                    else if (ControllerInput < 0)
                    {
                        PitchInput = SGT_Math.Remap(ControllerInput, -1, 0, 2.8f, 1);
                    }

                    _AudioSource.pitch = PitchInput;


                    for (int i = 0; i < GroundName.Count; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(GroundName[i]))
                        {
                                if (_AudioSource.clip!=null)
                                {
                                    if (_AudioSource.clip.name == GroundAudioClip[i].name&&_AudioSource.isPlaying)
                                    {
                                        break;
                                    }

                                }
                                if (ControllerInput>0)
                                {
                                    _AudioSource.clip = GroundAudioClip[i];
                                    Stepped.Invoke();
                                    _AudioSource.Play();
                            
                                }
                        

                            break;
                            }
        
                    }

                    if (ControllerInput == 0)
                    {
                        _AudioSource.Stop();

                    }

                }
            }
        }



        public void DisableEnableAudio(bool Value)
        {

            DisableComponent = Value;

        }


}
}