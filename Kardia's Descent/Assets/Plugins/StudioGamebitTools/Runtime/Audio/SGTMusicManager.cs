using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Audio;


namespace SGT_Tools.Audio
{

    public class SGTMusicManager : MonoBehaviour
{

        [Button("Set Name Of Gameobject", ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(-1)]
        public void SetNameOfGameobject()
        {
            gameObject.name = "--SGT_MusicManager--";
        }

        [PropertyOrder(1)]
    public SGTMusicManagerGroup[] MusicGroup;

    //You can set this settings for globally
    [HideInInspector]
    public float Volume = 1f;
    [HideInInspector]
    public float FadeIn = 2f;
    [HideInInspector]
    public float FadeOut = 2f;
    [HideInInspector]
    public float DelayTimeFadeOut = 0f;
    [HideInInspector]
    public bool StartBeggining = true;

    private void Awake()
    {
        for (int i = 0; i < MusicGroup.Length; i++)
        {
            GameObject Temp = new GameObject();
            Temp.gameObject.name = MusicGroup[i].Clip.name;
            Temp.transform.parent = gameObject.transform;
            AudioSource TempAudio = Temp.AddComponent<AudioSource>();
            TempAudio.clip = MusicGroup[i].Clip;
            TempAudio.volume = 0;
            TempAudio.pitch = MusicGroup[i].Pitch;
            TempAudio.priority = 0; //This is for If there is a lot of audio source, Unity not play someof audiosource in the scene, This is high priority source and It will not disable source.
            MusicGroup[i].Source = TempAudio;
            MusicGroup[i].Source.loop = MusicGroup[i].Loop;
            TempAudio.outputAudioMixerGroup = MusicGroup[i].Output;
            TempAudio.playOnAwake = false;


        }

    }


    public void PlayMusic(int index)
    {
        if (MusicGroup[index].StartBeggining)
        {
            MusicGroup[index].Source.Play();
        }
        
        DOTween.To(() => MusicGroup[index].Source.volume, x => MusicGroup[index].Source.volume = x, MusicGroup[index].Volume, MusicGroup[index].FadeIn);
        MusicGroup[index].PlayMusicImmeditlyEvent.Invoke();
        StartCoroutine(DelayPlay(index));
    }


    IEnumerator DelayPlay(int index)
    {
        yield return new WaitForSeconds(MusicGroup[index].DelayTimeFadeOut);
        
        MusicGroup[index].PlayMusicDelayEvent.Invoke();
    }


        public void StopMusicWithPitch(int index)
        {
            DOTween.To(() => MusicGroup[index].Source.pitch, x => MusicGroup[index].Source.pitch = x, 0, MusicGroup[index].FadeOut);
            DOTween.To(() => MusicGroup[index].Source.volume, x => MusicGroup[index].Source.volume = x, 0, MusicGroup[index].FadeOut);
        }

    

    public void StopMusic(int index)
    {
        MusicGroup[index].StopMusicEvent.Invoke();
        StartCoroutine(DelayStopStart(index));
        

    }

    IEnumerator DelayStopStart(int index)
    {
        yield return new WaitForSeconds(MusicGroup[index].DelayTimeFadeOut);
        DOTween.To(() => MusicGroup[index].Source.volume, x => MusicGroup[index].Source.volume = x, 0, MusicGroup[index].FadeOut);
        MusicGroup[index].StopDelayStartMusicEvent.Invoke();
    }


    public void StartMusic(int index)
    {
        MusicGroup[index].Source.Play();
    }

    public void StopOnlyMusic(int index)
    {
        MusicGroup[index].Source.Stop();
    }


    public void PauseOnlyMusic(int index)
    {
        MusicGroup[index].Source.Pause();
    }


    public void PauseAllMusic()
    {
        for (int i = 0; i < MusicGroup.Length; i++)
        {
            MusicGroup[i].Source.Pause();
        }
        
    }

    public void PlayAllMusic()
    {
        for (int i = 0; i < MusicGroup.Length; i++)
        {
            MusicGroup[i].Source.Play();
        }

    }

    public void StopAllMusic()
    {
        for (int i = 0; i < MusicGroup.Length; i++)
        {
            MusicGroup[i].Source.Stop();
        }

    }



    public void MuteAllMusic()
    {
        for (int i = 0; i < MusicGroup.Length; i++)
        {
            MusicGroup[i].Source.volume = 0;
        }

    }


    public void SetFadeIn(int Value)
    {

        FadeIn = Value;
    }

    public void SetFadeOut(int Value)
    {

        FadeOut = Value;
    }

    public void SetVolume(int Value)
    {

        FadeOut = Value;
    }

    public void SetDelayTimeFadeOut(int Value)
    {

        DelayTimeFadeOut = Value;
    }

    public void SetStartBeggining(int Value)
    {

        DelayTimeFadeOut = Value;
    }

    //Assign Current to target
    public void AssignFadeIn(int Value)
    {

        MusicGroup[Value].FadeIn=FadeIn;
    }

    public void AssignFadeOut(int Value)
    {

        MusicGroup[Value].FadeOut = FadeOut;
    }

    public void AssignVolume(int Value)
    {

        MusicGroup[Value].Volume = Volume;
    }

    public void AssignDelayTimeFadeOut(int Value)
    {

        MusicGroup[Value].DelayTimeFadeOut = DelayTimeFadeOut;
    }

    public void AssignStartBeggining(int Value)
    {

        MusicGroup[Value].StartBeggining = StartBeggining;
    }


#if UNITY_EDITOR
    [Button("Easy OutputMixer Assign", ButtonSizes.Medium), GUIColor(0, 1, 0)]
    [PropertyOrder(0)]
    public void AssignAudioMixer()
    {

        for (int i = 0; i < MusicGroup.Length; i++)
        {
            AudioMixerGroup MixerGroup = (AudioMixerGroup)AssetDatabase.LoadAssetAtPath("Assets/Plugins/StudioGamebitTools/StudioGamebitTools/Runtime/Audio/GlobalMixer/GlobalAudioManager.mixer", typeof(AudioMixerGroup));
            MusicGroup[i].Output = MixerGroup.audioMixer.FindMatchingGroups("Music")[0];
            MusicGroup[i].indexReadOnly = i;
        }

    }

#endif

}

[System.Serializable]
public class SGTMusicManagerGroup
{
    [ReadOnly]
    public int indexReadOnly;
    public AudioClip Clip;
    public AudioSource Source;
    public AudioMixerGroup Output;

    public float Volume = 1f;
    public float Pitch = 1f;
    public float FadeIn = 2f;
    public float FadeOut = 2f;
    public float DelayTimeFadeOut = 0f;
    public bool StartBeggining = true;
    public bool Loop = true;

    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent PlayMusicImmeditlyEvent = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent PlayMusicDelayEvent = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StopMusicEvent = new UnityEvent();
    [DrawWithUnity]
    [FoldoutGroup("Events")]
    [PropertyOrder(1)]
    public UnityEvent StopDelayStartMusicEvent = new UnityEvent();




   
}
}