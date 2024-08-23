using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {

    public SoundType name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public float SpatialBlend;
    public int NumberOfSource = 1;

    [HideInInspector]
    public List<AudioSource> source;
}
