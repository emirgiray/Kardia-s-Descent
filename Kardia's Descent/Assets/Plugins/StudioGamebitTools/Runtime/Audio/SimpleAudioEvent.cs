using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using UnityEditor;
using DG.Tweening;

namespace SGT_Tools.Audio
{


[CreateAssetMenu(menuName="AudioEvents")]
public class SimpleAudioEvent : SGT_AudioEvent
{
    public SGT_AudioManagerCenter AudioManagerCenter;
    public AudioMixerGroup MixerGroup;
	public AudioClip[] clips;
    [MinMaxSlider(0, 1)]
    public Vector2 volume = new Vector2(0.9f,1);

    [MinMaxSlider(0, 2)]
    public Vector2 pitch= new Vector2(0.9f, 1);
    public bool _3DAudios;
    public AudioRolloffMode RolloffMode;
    public float _MinDistance = 1;
    public float _MaxDistance = 50;
    [Tooltip("Eğer obje arka arkaya çalması gerekiyorsa true yap.")]
    public bool Replay = false;
    [Tooltip("Eğer objeden çıkan ses 3 boyutluysa true olacak değilse false olacak")]
    public bool Spatialize = true;
    public bool AttachObject = true;

        public bool FadeIn = false;
        [ShowIf("FadeIn")]
        public float FadeTime = 1f;
        AudioSource source;


    public override void Play(Transform Position)
	{


      
            source = SelectAudioSource(AudioManagerCenter);

            if (source==null)
            {
                if (AudioManagerCenter.AudioManager.PoolExpend)
                {
                    source = AudioManagerCenter.AudioManager.IfAudioSourceIsNullThenCreateNewSource();
                    source.gameObject.SetActive(true);
                    if (AttachObject)
                    {
                        AudioManagerCenter.AudioManager.ParentObjectEnd(source.gameObject, Position);
                    }
                    


                }
                else
                {
                    return;
                }

            }
        
        if (Position != null)
        {
            if (AttachObject)
            {
                source.gameObject.transform.parent = Position;
            }
            source.gameObject.transform.position = Position.position;
        }

            source.outputAudioMixerGroup = MixerGroup;
            source.clip = clips[Random.Range(0, clips.Length)];
            float NewVolume = Random.Range(volume.x, volume.y);
            source.volume = NewVolume;  
            if (_3DAudios)
            {
            source.spatialBlend = 1;
            }
            else
            {
                source.spatialBlend = 0;
            }

            source.rolloffMode = RolloffMode;
            source.minDistance = _MinDistance;
            source.maxDistance = _MaxDistance;

            source.pitch = Random.Range(pitch.x, pitch.y);
            
            if (Spatialize)
            {
                source.spatialize = true;
            }
            else
            {
                source.spatialize = false;
            }
            if (FadeIn)
            {
                float NewV = 0;
                DOTween.To(() => NewV, x => source.volume = x, NewVolume, FadeTime);
               
            }
            if (source!=null)
            {
                if (source.gameObject.activeInHierarchy)
                {
                    source.Play();
                }
                
            }
            
            
                AudioManagerCenter.AudioManager.StopSource(source, source.clip.length);
            
        



    }



    [Button("Stop Audio", ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(1)]
    public void StopAudioSource()
    {
        if (source!=null)
        {
            source.Stop();
        }
        
    }

    

    AudioSource s;
    public AudioSource SelectAudioSource(SGT_AudioManagerCenter AudioManager)
    {
        for (int i = 0; i < AudioManager.AudioManager.AudioSource.Count; i++)
        {
                if (AudioManager.AudioManager!=null)
                {
                    if (AudioManager.AudioManager.AudioSource[i]!=null)
                    {
                        if (!AudioManager.AudioManager.AudioSource[i].gameObject.activeInHierarchy && !AudioManager.AudioManager.AudioSource[i].isPlaying)
                        {

                            s = AudioManager.AudioManager.AudioSource[i];
                            AudioManager.AudioManager.AudioSource[i].gameObject.SetActive(true);

                            return s;
                            
                        }
                        else
                        {
                            s = null;
                        }
                    }
                
                   
                }
            }
        return s;

    }

#if UNITY_EDITOR

    [Button("Easy AssignManager", ButtonSizes.Medium), GUIColor(0, 1, 0)]
    [PropertyOrder(1)]
    public void AssignManager()
    {

        AudioManagerCenter = (SGT_AudioManagerCenter)AssetDatabase.LoadAssetAtPath("Assets/Plugins/StudioGamebitTools/StudioGamebitTools/Runtime/Audio/SGT_Audio Manager Center.asset", typeof(SGT_AudioManagerCenter));
        MixerGroup= (AudioMixerGroup)AssetDatabase.LoadAssetAtPath("Assets/Plugins/StudioGamebitTools/StudioGamebitTools/Runtime/Audio/GlobalMixer/GlobalAudioManager.mixer", typeof(AudioMixerGroup));
        MixerGroup=MixerGroup.audioMixer.FindMatchingGroups("SFX")[0];
    }




    [Button("TestSource", ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void PreviewPlay()
    {

        PlayClip(clips[Random.Range(0, clips.Length)]);
    }

    [Button("StopSource", ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
    [PropertyOrder(0)]
    public void PreviewStop()
    {

        StopAllClips();
    }



    //You can play audio clip in editor.
    public static void PlayClip(AudioClip clip)
    {
        
        System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
            null,
            new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );
        method.Invoke(
            null,
            new object[] { clip, 0, false }
        );
    }




    public static void StopAllClips()
    {
        System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass =
              unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
            null,
            new System.Type[] { },
            null
        );
        method.Invoke(
            null,
            new object[] { }
        );
    }



#endif

}
}