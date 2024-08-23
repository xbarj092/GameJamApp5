using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    public Sound[] sounds;
    [SerializeField] private bool SpacialBlend;
    [SerializeField] private AudioMixerGroup mixer;

    public bool Muted = false;

    void Awake()
    {
        foreach (Sound s in sounds) {
            for(int i = 0; i < s.NumberOfSource; i++) { 
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = s.name == SoundType.Ambience || s.name == SoundType.Menu;
                source.clip = s.clip;
                source.volume = s.volume;
                source.pitch = s.pitch;
                source.spatialBlend = s.SpatialBlend;
                source.outputAudioMixerGroup = mixer;
                s.source.Add(source);
            }
        }
    }

    public void Play(SoundType name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        bool find = false;

        AudioSource foundSource = s.source.Find((source) => {
            find = !source.isPlaying;
            return find;
        });

        if (foundSource != null)
        {
            foundSource.Play();
        }
        else
        {
            s.source[0].Play();
        }
    }

    public void Stop(SoundType name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.ForEach((source) => source.Stop());
    }

    public bool IsPlaying(SoundType name) { 
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.Exists((source) => source.isPlaying);
    }

    public void SetVolume(bool mute)
    {
        Muted = mute;
        AudioListener.volume = mute ? 0 : 1;
    }
}
