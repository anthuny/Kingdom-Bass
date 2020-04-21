using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Map")]
public class Map : ScriptableObject
{
    [Header("General Info")]
    public string title;
    public int bpm;
    public string length;
    public string difficulty;
    public float starCount;
    public string subGenre;
    public float noteTimeTaken;
    public float timeBeforeStart;
    public int sliderFastSpeed;
    [Tooltip("1 is normal, higher = more health loss upon bad performance, lower = less health loss upon bad performance")]
    public float healthDrain;
    public int jetAimRange;

    [Header("Track")]
    public AudioClip track;
    public AudioClip trackLoop;
    public TextAsset mapXML;
}
