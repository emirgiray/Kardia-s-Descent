using System;
using System.Collections;
using System.Collections.Generic;
using SGT_Tools.Bridge;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioQueueManager : MonoBehaviour
{
    [ProgressBar("Min","Max")]
    [PropertyOrder(0)]
    [SerializeField] private float clipProgress = 0;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float fadeTime = 1f;
    private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    private float Min = 0;
    private float Max = 1;
    public void AddClipToQueue(AudioClip clip)
    {
        audioQueue.Enqueue(clip);
        if (!audioSource.isPlaying)
        {
            PlayNextClip();
        }
    }

    private void Start()
    {
        AddClipsToQueue(clips);
    }

    private void AddClipsToQueue(AudioClip[] clips)
    {
        foreach (var clip in clips)
        {
            audioQueue.Enqueue(clip);
        }
       
    }

    public void PlayNextClip()
    {
        if (audioQueue.Count > 0)
        {
            audioSource.clip = audioQueue.Dequeue();
            StartCoroutine(FadeInAndPlay());
        }
    }

    private IEnumerator FadeInAndPlay()
    {
        audioSource.volume = 0;
        audioSource.Play();
        clipProgress = 0;
        Max = audioSource.clip.length;
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        StartCoroutine(FadeOutAndNext());
        StopCoroutine(AssignProgress());
        StartCoroutine(AssignProgress());
    }

    private IEnumerator FadeOutAndNext()
    {
        
        yield return new WaitForSeconds(audioSource.clip.length - fadeTime);

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        PlayNextClip();
    }

    private IEnumerator AssignProgress()
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            clipProgress = audioSource.time;
            clipProgress = SGT_Math.Remap(clipProgress, 0, audioSource.clip.length, Min, audioSource.clip.length);
        }
        
    }
}
