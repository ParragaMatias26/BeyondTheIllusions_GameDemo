using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] Clips;
    Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();

    AudioSource Source;

    private void Awake()
    {
        Source = GetComponent<AudioSource>();

        foreach (AudioClip clip in Clips) 
        {
            if (!AudioClips.ContainsKey(clip.name)) AudioClips.Add(clip.name, clip);
        }
    }

    private void Start()
    {
        CanvasManager.Instance.OnVolumeChange += AdjustVolume;
    }

    public void PlaySound(string clipName)
    {
        Source.clip = AudioClips[clipName];
        Source.Play();
    }

    public void StopSound()
    {
        Source.Stop();
        Source.clip = null;
    }

    public void Loop(bool state) => Source.loop = state;

    void AdjustVolume(float newValue) => Source.volume = newValue;
}
