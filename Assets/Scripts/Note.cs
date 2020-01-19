using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    private PathManager pm;
    private Gamemode gm;
    public int beatOfThisNote;
    public bool online;
    public Vector3 startingPos;
    Path path;
    public float pathWidth;
    float t;
    float trackPosInBeats;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        path = pm.initialPath.GetComponent<Path>();
    }

    // Update is called once per frame
    void Update()
    {
        beatOfThisNote = (int)Mathf.Round(tc.trackPosInBeats);
        //Debug.Log("Beat of this note : " + beatOfThisNote);
        //Debug.Log("Track pos in Beats : " + tc.trackPosInBeats);
        //Debug.Log("beats shown in advance : " + tc.BeatsShownInAdvance);

        t = ((tc.BeatsShownInAdvance - (beatOfThisNote - tc.trackPosInBeats)) / tc.BeatsShownInAdvance);
        t = Mathf.Clamp01(t);
        transform.position = Vector3.Lerp(startingPos, new Vector3(pathWidth, 0.01f, 0), t);

        //Debug.Log(((tc.BeatsShownInAdvance - (beatOfThisNote - tc.trackPosInBeats)) / tc.BeatsShownInAdvance));

    }
   
}
