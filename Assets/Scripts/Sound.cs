using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0, 3)]
    public float pitch;
    //public bool isTrack;

    [HideInInspector]
    public AudioSource source;
}
