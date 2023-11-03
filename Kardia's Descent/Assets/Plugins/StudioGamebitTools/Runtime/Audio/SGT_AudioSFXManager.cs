using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace SGT_Tools.Audio
{

    public class SGT_AudioSFXManager : MonoBehaviour
    {

        [SerializeField] private int CreateAudioSource = 20;
        public bool PoolExpend = false;
        public List<AudioSource> AudioSource = new List<AudioSource>();
        public SGT_AudioManagerCenter AudioCenter;

        private void Awake()
        {
            
            CreateAudioList();
            AudioCenter.AudioManager = this;
        }



        /// <summary>
        /// Create Audio Sources for pooling
        /// </summary>
        public void CreateAudioList()
        {

            for (int i = 0; i < CreateAudioSource; i++)
            {
                GameObject Temp = new GameObject();
                Temp.gameObject.name = "PooledAudioSource" + i;
                Temp.transform.parent = gameObject.transform;
                Temp.AddComponent<SGT_AudioSourceControl>();
                Temp.GetComponent<SGT_AudioSourceControl>().SoundManager = this;
                AudioSource TempAudio = Temp.AddComponent<AudioSource>();
                AudioSource.Add(TempAudio);

                Temp.gameObject.SetActive(false);
            }
        }


        public void StopSource(AudioSource Source, float time)
        {
            //StartCoroutine(StopActiveAudioSource(Source, time));
            if (Source.gameObject.GetComponent<SGT_AudioSourceControl>()!=null)
            {
                Source.gameObject.GetComponent<SGT_AudioSourceControl>().DisableTimerStart(time);
            }

        }

        /// <summary>
        /// If Source destroy anyway, We will remove it and create new audio source for SFXManager
        /// </summary>
        /// <param name="Source"></param>
        public void RemoveSourceBeforeDestroyAndCreateNewOne(AudioSource Source)
        {
            AudioSource.Remove(Source);
            
            GameObject Temp = new GameObject();
            Temp.gameObject.name = "PooledAudioSource-AfterDestroyNewOne" ;
            Temp.transform.parent = gameObject.transform;
            Temp.AddComponent<SGT_AudioSourceControl>();
            Temp.GetComponent<SGT_AudioSourceControl>().SoundManager = this;
            AudioSource TempAudio = Temp.AddComponent<AudioSource>();
            AudioSource.Add(TempAudio);

            Temp.gameObject.SetActive(false);
        }



        public AudioSource IfAudioSourceIsNullThenCreateNewSource()
        {
            GameObject Temp = new GameObject();
            Temp.gameObject.name = "PooledAudioSource-ExpendNewSource";
            //Temp.transform.parent = gameObject.transform;/
            Temp.AddComponent<SGT_AudioSourceControl>();
            Temp.GetComponent<SGT_AudioSourceControl>().SoundManager = this;
            AudioSource TempAudio = Temp.AddComponent<AudioSource>();
            AudioSource.Add(TempAudio);

            Temp.gameObject.SetActive(false);
            return TempAudio;
        }



        public void GetBackSourceAsParent(GameObject Value,AudioSource Source)
        {
            if (Value!=null&&Source!=null)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(GetBackNextFrame(Value, Source));
                }
                
            }
            
        }

        IEnumerator GetBackNextFrame(GameObject Value, AudioSource Source)
        {
            yield return new WaitForEndOfFrame();
            
            if (Value!=null)
            {
                if (gameObject!=null)
                {
                    Value.transform.parent = gameObject.transform;
                }
                
            }
            else
            {
                RemoveSourceBeforeDestroyAndCreateNewOne(Source);
            }
            
        }



        [Button("Set Name Of Gameobject", ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(1)]
        public void SetNameOfGameobject()
        {
            gameObject.name = "--SGT_AudioSFXManager--";
        }

        /// <summary>
        /// If Source created with expend option, Then It will move to attach target transform.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Value"></param>
        public void ParentObjectEnd(GameObject Source, Transform Value)
        {
            StartCoroutine(ParentObjectEndOfFrame(Source,Value));
        }


        IEnumerator ParentObjectEndOfFrame(GameObject Source, Transform Value)
        {
            yield return new WaitForEndOfFrame();
            Source.transform.parent = Value;
        }

    }
}