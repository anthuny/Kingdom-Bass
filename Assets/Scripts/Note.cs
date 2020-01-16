using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    private PathManager pm;
    public int beatOfThisNote;
    public bool online;
    public Vector3 startingPos;
    Path path;
    public float pathWidth;
    private bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        path = pm.initialPath.GetComponent<Path>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    void Move()
    {
        if (canMove)
        {
            beatOfThisNote = (int)Mathf.Round(tc.trackPosInBeats);

            canMove = false;
            Debug.Log("moving");
            transform.position = Vector3.Lerp(startingPos, new Vector3(pathWidth, 0.01f, path.pathLength - path.pathLength), (tc.BeatsShownInAdvance - (beatOfThisNote - tc.trackPosInBeats)) / tc.BeatsShownInAdvance);
            Debug.Log((tc.BeatsShownInAdvance - (beatOfThisNote - tc.trackPosInBeats)) / tc.BeatsShownInAdvance);
            // Future Anthony: I dont think the 3rd paramater in the vector3.Lerp is correct. Does this whole thing need be in update? or just called once? It's not going from 0 - 1. It's always at 1.01 > 1.08? | Fix this
        }
    }
}
