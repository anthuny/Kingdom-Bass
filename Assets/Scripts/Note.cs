using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    private PathManager pm;
    private Gamemode gm;

    [HideInInspector]
    public bool isNote, isLaunch;

    [HideInInspector]
    public Vector3 startingPos;
    [HideInInspector]
    public float pathWidth;
    [HideInInspector]
    private Player player;
    [HideInInspector]
    public string arrowDir;
    //[HideInInspector]
    public int eighthWait;

    public bool doneOnce;
    public bool doneOnce2;
    public bool doneOnce3;
    public bool doneOnce4;

    public int laneNumber;
    [HideInInspector]
    public float percDistance;

    bool hitEnd;

    private float wallLengths;
    private Renderer rend;

    public Vector3 playerPos;

    private float startTime;
    private float curTime = 0;

    public bool canMove;

    public GameObject meshRendererRef;
    private Renderer noteRend;
    private float noteWidth;
    private bool hitEndLoop;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();
        noteRend = meshRendererRef.GetComponent<Renderer>();
        noteWidth = noteRend.bounds.size.z;

        //Determine the direction of the arrow on the note
        switch (arrowDir)
        {
            case "left":
                if (isNote)
                {
                    Sprite leftArrow = Resources.Load<Sprite>("Sprites/T_LeftArrow") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;
                   
                }
                else if (isLaunch)
                {
                    Sprite leftArrowLaunch = Resources.Load<Sprite>("Sprites/T_LeftArrowLaunch") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;
                }

                break;

            case "right":
                if (isNote)
                {
                    Sprite rightArrow = Resources.Load<Sprite>("Sprites/T_RightArrow") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;
                }
                else if (isLaunch)
                {
                    Sprite rightArrowLaunch = Resources.Load<Sprite>("Sprites/T_RightArrowLaunch") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;
                }


                break;

            case "up":
                Sprite upArrow = Resources.Load<Sprite>("Sprites/T_UpArrow") as Sprite;
                    
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = upArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.upArrowC;

                break;

            default:
                Debug.Log(this.gameObject.name + " does not have an arrow direction");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (canMove && !doneOnce)
        {
            doneOnce = true;
            startTime = tc.trackPos;
        }

        // Calculate the percantage of completion of the note on the lane
        percDistance = Mathf.Abs((transform.position.z - player.transform.position.z) / pm.pathLength) * 100;

        // Determine the speed the note needs to use to get to the player on the beat
        //gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);
        gm.noteSpeed = pm.pathLength / (tc.secPerBeat * tc.noteTimeTaken);

        if (curTime <= 1 && !doneOnce2)
        {
            curTime = Mathf.Clamp01((tc.trackPos - startTime) / (tc.noteTimeTaken * tc.secPerBeat));

            Vector3 pos;
            // Interpolate the note between the edge of the note, to the edge of the player (closest edges from eachother, based on curTime)

            pos = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, startingPos.z),
                            new Vector3(transform.position.x, transform.position.y, player.transform.position.z), curTime);
            transform.position = pos;
        }

        if (curTime >= 1)
        {
            doneOnce2 = true;
            transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;

        }


        if (doneOnce2 && !doneOnce3)
        {
            doneOnce3 = true;

            // Rounds the value to the nearest .25f
            tc.previousNoteBeatTime = ((Mathf.Round((tc.trackPosInBeatsGame - .25f) * 4)) / 4) + .25f;

            tc.previousNoteBeatTime3 = tc.previousNoteBeatTime;
            tc.nextNoteInBeats3 = tc.previousNoteBeatTime3 + (tc.noteEighthCount[tc.nextIndex3] / tc.maxNoteIntervalsEachBeat);
            //Debug.Log(tc.previousNoteBeatTime3);
            //Debug.Log(tc.nextNoteInBeats3);

            if (!tc.doneOnce)
            {
                tc.doneOnce = true;

                tc.nextIndex3++;

                tc.trackPosNumber = tc.trackPosIntervalsList3[tc.nextIndex3 - 1];
                tc.trackPosNumber2 = tc.trackPosIntervalsList3[tc.nextIndex3];

                tc.previousNoteBeatTime2 = tc.previousNoteBeatTime + (tc.noteEighthCount[tc.nextIndex3 - 1] / tc.maxNoteIntervalsEachBeat);

                tc.nextNoteInBeats = tc.previousNoteBeatTime2;
                tc.nextNoteInBeats2 = tc.nextNoteInBeats + (tc.noteEighthCount[tc.nextIndex3] / tc.maxNoteIntervalsEachBeat);

                float a = (tc.nextNoteInBeats + tc.previousNoteBeatTime) / 2;
                float b = (tc.previousNoteBeatTime2 + tc.nextNoteInBeats2) / 2;
                tc.curNoteDiff = a;
                tc.nextNoteDiff = b;

                //Debug.Log("nextIndex3 " + tc.nextIndex3);
                //Debug.Log("previousNotebeatTime " + tc.previousNoteBeatTime);
                //Debug.Log("previousNoteBeatTime2 " + tc.previousNoteBeatTime2);

                //Debug.Log("nextNoteInBeats " + tc.nextNoteInBeats);
                //Debug.Log("nextNoteInBeats2 " + tc.nextNoteInBeats2);

                //Debug.Log("currentNoteDiff " + tc.curNoteDiff);
                //Debug.Log("nextNoteDiff " + tc.nextNoteDiff);
                //Debug.Log("-----------------------------");
            }


            // Ensure that the player can gain score for the note, even if they inputed a movement just before passing a note
            // This is reliant on this note staying alive long enough for to switch canGetNote to true from false
            if (!tc.canGetNote && !doneOnce4)
            {
                //Debug.Log("allowing");
                doneOnce4 = true;
                tc.canGetNote = true;
            }

            //tc.trackPosIntervalsList2.RemoveAt(0);
            tc.trackPosIntervalsList2.RemoveAt(0);

            // If this note is the 2nd note of all notes ever.
            // Remove interval index 0 when it gets to the end of it's path
            if (tc.notes.transform.GetChild(1).gameObject == this.gameObject)
            {
                //Debug.Log("should removing thing");
                tc.deadNoteAssigned = true;
                tc.trackPosIntervalsList.RemoveAt(0);
                tc.canGetNote = true;
                hitEnd = true;
            }

            else if (tc.notes.transform.GetChild(0).gameObject == this.gameObject)
            {
                tc.trackPosIntervalsList.RemoveAt(0);
                tc.canGetNote = true;
                hitEnd = true;
            }

            else
            {
                Debug.Log("error");
            }
        }

        if (tc.deadNoteAssigned && doneOnce2 && !doneOnce3)
        {
            doneOnce3 = true;
            tc.trackPosIntervalsList.RemoveAt(0);
            tc.canGetNote = true;
            //Debug.Log("removed index 0 of interval list.");
        }
        if (hitEnd)
        {
            hitEnd = false;
            hitEndLoop = true;
            // The float (0.5f) may cause problems when spawning notes at different speeds,
            // or if the bpm of the song changes
            Invoke("DestroyNote", (tc.secPerBeat * tc.trackPosIntervalsList[0]) + .02f);
            //Debug.Log((tc.secPerBeat * tc.trackPosIntervalsList[0]));
            //Debug.Log("nextIndex 3 " + tc.trackPosIntervals);
        }

        
        if (hitEndLoop)
        {

            float halfWay = tc.pointToNextBeat - (tc.previousNoteBeatTime / 2);
            float a = tc.pointToNextBeat - halfWay;
            float halfWay2 = tc.pointToNextBeat2 - (tc.previousNoteBeatTime / 2);
            float b = tc.pointToNextBeat2 - halfWay2;

            //Debug.Log("halfWay1 " + halfWay);
            //Debug.Log(a);
            //Debug.Log("halfWay2 " + halfWay2);
            //Debug.Log(b);

            // future anthony - im up to here, trying to do what's on the white board. Need to figure out further ahead pointto and prevnote
            /*
            // Calculate the difference between the next beat and the last in timeinbeats
            float difference = tc.pointToNextBeat - tc.previousNoteBeatTime;
            Debug.Log("whole thing " + (tc.previousNoteBeatTime + (gm.goodMin * difference)));
            Debug.Log("previousNoteBeatTime " + tc.previousNoteBeatTime);
            Debug.Log("pointToNextBeat " + tc.pointToNextBeat);
            //Debug.Log("a " + (gm.goodMin * difference));
            // If player can get note, and
            // they are still in range for the worst score from the beat before in seconds
            if (tc.canGetNote && tc.trackPosInBeatsGame > tc.previousNoteBeatTime + (gm.goodMin * difference) + 0.1f)
            {
                Debug.Log("configuring miss");
                player.GetComponent<Player>().Missed();
                hitEndLoop = false;

                Debug.Log("trackPosInBeatsGame " + tc.trackPosInBeatsGame);
                Debug.Log("previousNoteBeatTime " + tc.previousNoteBeatTime);
                Debug.Log("difference " + difference);
                Debug.Log("`````````````````````````````");
            }
            */
        }
        
    }
    void DestroyNote()
    {
        Destroy(this.gameObject);
    }
}


