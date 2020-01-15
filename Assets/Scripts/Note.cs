using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    public int beatOfThisNote;

    // Start is called before the first frame update
    void Start()
    {
        tc = FindObjectOfType<TrackCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        beatOfThisNote = (int)Mathf.Round(tc.trackPosInBeats);
    }
}
