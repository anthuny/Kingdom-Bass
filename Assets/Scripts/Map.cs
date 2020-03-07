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
    public float noteTimeTaken;
    public float timeBeforeStart;

    [Header("Track")]
    public AudioClip track;
    public TextAsset mapXML;
}
