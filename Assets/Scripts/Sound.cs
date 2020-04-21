using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool menuSound;
    public bool playOnAwake;
    public bool loop;
    public bool spatial;
    public bool sfx;

    [HideInInspector]
    public AudioSource source;
}
