using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> menuSources = new List<AudioSource>();
    public Gamemode gm;
    [Header("Electricity")]
    public AudioMixerGroup musicGrp;
    public AudioMixerGroup sfxGrp;
    public Sound[] sounds;


    public static AudioManager instance;






    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;

            // Assign where which audio mixer group this sound belongs to
            if (s.sfx)
            {
                s.source.outputAudioMixerGroup = sfxGrp;
            }
            else
            {
                s.source.outputAudioMixerGroup = musicGrp;
            }

            if (s.spatial)
            {
                s.source.rolloffMode = AudioRolloffMode.Linear;
                s.source.minDistance = 1;
                s.source.maxDistance = 70;
            }

            if (s.menuSound)
            {
                menuSources.Add(s.source);
            }
        }
    }

    public void PlaySound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void PauseSound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Pause();
    }

    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.UnPause();
    }

    public void StopSound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }
}
