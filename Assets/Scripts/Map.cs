using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Map")]
public class Map : ScriptableObject
{
    [Header("General Info")]
    public string title;
    public int bpm;
    public float length;
    public float difficulty;
    public string subGenre;
    public float noteTimeTaken;
    public float timeBeforeStart;
    public int averageBeatsBtwNotes;

    [Header("Track")]
    public AudioClip track;
    public AudioClip trackLoop;
    public TextAsset mapXML;
}
