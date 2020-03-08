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
    public Vector3 startingPos;
    [HideInInspector]
    public float pathWidth;
    [HideInInspector]
    private Player player;
    //[HideInInspector]
    public float beatWait;
    public float beatWaitCur;
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

    public string noteType;
    public string noteDir;

    public bool doneUpArrow;

    public Renderer spotLightRef;
    public bool behindPlayer = false;

    private float H;
    private float S;
    private float V;

    [Tooltip("The player is only able to obtain 1 amount of score per note." +
    " If this variable is true, the player is able to still obtain score for this note.")]
    public bool canGetNote = true;
    public int hitAmount = 0;
    public bool noteCalculatedAcc;

    [Tooltip("If this is true, this note has been missed")]
    //[HideInInspector]
    public bool missed;

    private bool offSetCompleted;
    private float timer;

    public GameObject noteObject;
    public GameObject hitMarker;
    public GameObject hitMarkerCanvas;
    public GameObject spotLight;

    Vector3 hitMarkerRot;

    bool doneOnce5;
    public int noteNumber;

    [Header("Electricity")]
    public Transform ElectrictyEnd;
    public int doneElecrictyEffect;

    [Header("Bomb")]
    public GameObject bombObj;
    private bool bombHitPlayer;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();
        noteRend = meshRendererRef.GetComponent<Renderer>();
        noteWidth = noteRend.bounds.size.z;

        gm.totalNotes++;
        noteNumber = gm.totalNotes;

        tc.notesSpawned++;

        //Determine the direction of the arrow on the note
        switch (noteDir)
        {
            case "left":
                if (noteType == "note")
                {
                    Sprite leftArrow = gm.leftArrow;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;

                    //Set the colour of the spotlight
                    Color.RGBToHSV(gm.horizontalNoteArrowC, out H, out S, out V);
                    V += gm.noteSpotLightDiff;
                    spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

                    // Increase the strength of the spotlight because it is a note note.
                    spotLightRef.material.SetFloat("Vector1_114CB03C", gm.noteSpotLightIntensity);
                }
                else if (noteType == "launch")
                {
                    Sprite leftArrowLaunch = gm.leftLaunchArrow;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;

                    //Set the colour of the spotlight
                    Color.RGBToHSV(gm.horizontalLaunchArrowC, out H, out S, out V);
                    V += gm.noteSpotLightDiff;
                    spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

                    //Increase the scale of the spotlight because it is a launch note.
                    Vector3 scale = spotLightRef.gameObject.transform.localScale;
                    spotLightRef.gameObject.transform.localScale = new Vector3(scale.x = 1 + gm.launchSpotLightInc, scale.y + gm.launchSpotLightInc / 2, scale.z = 1 + gm.launchSpotLightInc);

                    // Increase the strength of the spotlight because it is a launch note.
                    spotLightRef.material.SetFloat("Vector1_114CB03C", gm.launchSpotLightIntensity);
                }

                break;

            case "right":
                if (noteType == "note")
                {
                    Sprite rightArrow = gm.rightArrow;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;

                    //Set the colour of the spotlight
                    Color.RGBToHSV(gm.horizontalNoteArrowC, out H, out S, out V);
                    V += gm.noteSpotLightDiff;
                    spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

                    // Increase the strength of the spotlight because it is a note note.
                    spotLightRef.material.SetFloat("Vector1_114CB03C", gm.noteSpotLightIntensity);
                }
                else if (noteType == "launch")
                {
                    Sprite rightArrowLaunch = gm.rightArrowLaunch;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;

                    //Set the colour of the spotlight
                    Color.RGBToHSV(gm.horizontalLaunchArrowC, out H, out S, out V);
                    V += gm.noteSpotLightDiff;
                    spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

                    //Increase the scale of the spotlight because it is a launch note.
                    Vector3 scale = spotLightRef.gameObject.transform.localScale;
                    spotLightRef.gameObject.transform.localScale = new Vector3(scale.x = 1 + gm.launchSpotLightInc, scale.y + gm.launchSpotLightInc / 2, scale.z = 1 + gm.launchSpotLightInc);

                    // Increase the strength of the spotlight because it is a launch note.
                    spotLightRef.material.SetFloat("Vector1_114CB03C", gm.launchSpotLightIntensity);
                }

                break;

            case "up":
                Sprite upArrow = gm.upArrow;
                    
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = upArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.upArrowC;

                //Set the colour of the spotlight
                Color.RGBToHSV(gm.upArrowC, out H, out S, out V);
                V += gm.noteSpotLightDiff;
                spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

                // Increase the strength of the spotlight because it is a note note.
                spotLightRef.material.SetFloat("Vector1_114CB03C", gm.noteSpotLightIntensity);

                if (noteType == "bomb")
                {
                    // Make the bomb visible
                    bombObj.SetActive(true);

                    // Set the note visual to invisible
                    noteObject.SetActive(false);

                    // Disable the spotlight
                    spotLight.SetActive(false);
                }
                break;        

            default:
                Debug.Log(this.gameObject.name + " does not have proper arrow direction");
                break;
        }

        // Make the note look like a blast if it is a blast
        if (noteType == "blast")
        {                     
            // Set the note sprite to be the blast sprite
            Sprite blastNote = gm.blast;

            // Set the aim sprite to a different sprite
            hitMarker.GetComponent<Image>().sprite = gm.blastAim;

            transform.GetChild(1).GetComponentInChildren<Image>().sprite = blastNote;
            transform.GetChild(1).GetComponentInChildren<Image>().color = gm.blastNoteC;

            //Set the colour of the spotlight
            Color.RGBToHSV(gm.blastNoteC, out H, out S, out V);
            V += gm.noteSpotLightDiff;
            spotLightRef.material.SetColor("Color_9834739F", Color.HSVToRGB(H, S, V));

            // Increase the strength of the spotlight because it is a blast note.
            spotLightRef.material.SetFloat("Vector1_114CB03C", gm.noteSpotLightIntensity);

            // Increase the visual scale of the note to span across all lanes
            noteObject.GetComponent<RectTransform>().sizeDelta = new Vector3(500, 100, 0);

            // Set the note visual to invisible
            noteObject.SetActive(false);

            // Increase the visual scale of the aimsprite to span across all lanes
            hitMarker.GetComponent<RectTransform>().sizeDelta = new Vector3(7.5f, 1.25f, 0);

            // Change the color of the hitMarker
            hitMarker.GetComponent<Image>().color = gm.blastNoteC;

            // Turn the 'LookAtCam' script on so that the 'hitMarketCanvas' looks at the player
            hitMarkerCanvas.GetComponent<LookAtCam>().enabled = true;

            // Disable the spotlight
            spotLight.SetActive(false);
        }
    }

    void UpdateZRotation()
    {
        hitMarkerRot.z = 0;
        hitMarker.GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(hitMarkerRot);
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        CheckBombHitPlayer();

        if (noteType == "blast")
        {
            UpdateZRotation();
        }

        if (this.noteDir == "up" && this.noteType != "blast")
        {
            player.DoNoteEffectUp();
        }

        // Happens once when canMove is triggered true
        if (canMove && !doneOnce)
        {
            doneOnce = true;
            startTime = tc.trackPos;
            //Debug.Log("-----------------------------");
            //Debug.Log("startTime in seconds " + startTime);
            startTime /= tc.secPerBeat;
            //Debug.Log("startTime in beats " + startTime);
            startTime = Mathf.Round(startTime * 4) / 4;
            //Debug.Log("startTime in beats rounded " + startTime);
            startTime *= tc.secPerBeat;
            //Debug.Log("startTime in seconds rounded " + startTime);
            //Debug.Break();
            // Add this note to the active notes array
            player.activeNotes.Add(this.gameObject.transform);
        }

        // Calculate the percantage of completion of the note on the lane - not currently used
        percDistance = Mathf.Abs((transform.position.z - player.transform.position.z) / pm.pathLength) * 100;

        // Determine the speed the note needs to use to get to the player on the beat
        //gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);
        gm.noteSpeed = pm.pathLength / (tc.secPerBeat * tc.selectedMap.noteTimeTaken);

        if (curTime <= 1 && !doneOnce2)
        {
            //Debug.Log("startTime now " + startTime);
            curTime = Mathf.Clamp01((tc.trackPos - startTime) / (tc.selectedMap.noteTimeTaken * tc.secPerBeat));
            //Debug.Log(this.gameObject.name + " trackPosInBeatsGame " + tc.trackPosInBeatsGame);
            Vector3 pos;
            // Interpolate the note between the edge of the note, to the edge of the player (closest edges from eachother, based on curTime)

            pos = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, startingPos.z),
                            new Vector3(transform.position.x, transform.position.y, player.transform.position.z), curTime);
            transform.position = pos;
        }

        if (curTime > 0 && !doneOnce5)
        {
            doneOnce5 = true;
            //Debug.Log("startTime Final " + startTime);
            //Debug.Break();
        }

        if (curTime >= 1)
        {
            doneOnce2 = true;
            if (tc.curNoteCount < tc.noteLanes.Count)
            {
                transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
            }
            else
            {
                if (tc.trackInProgress)
                {
                    transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
                }
                else
                {
                    return;
                }
            }

            // Remove this note many lists when it passes the player
            player.notesInfront.Remove(this.gameObject.transform);

            tc.notes.Remove(this.gameObject);
            
            if (player.closestNoteInFront == this.gameObject)
            {
                player.closestNoteInFront = null;
            }

            if (player.closestNoteInFrontScript == this)
            {
                player.closestNoteInFrontScript = null;
            }
            
        }

        if (doneOnce2 && !doneOnce3)
        {
            doneOnce3 = true;
            // Rounds the value to the nearest .25f
            // This is done because the previousNoteBeatTime will most likely always be slightly off when it should be.
            // This rounds it to when the beat should happen when the note hit the player
            //Debug.Log("trackPosInBeatsGame " + tc.trackPosInBeatsGame);

            if (noteType != "bomb")
            {
                Debug.Log("before tc.trackPosInBeatsGame" + tc.trackPosInBeatsGame);
                Debug.Break();
                tc.previousNoteBeatTime = Mathf.Round(tc.trackPosInBeatsGame * 1) / 1;

                tc.previousNoteBeatTime3 = tc.previousNoteBeatTime;
                Debug.Log("after tc.trackPosInBeatsGame" + tc.trackPosInBeatsGame);
                Debug.Break();

                player.CalculateMissPointFrom();

                tc.searchingNotes = false;


                for (int i = Mathf.RoundToInt(tc.newStartingInt); i < tc.allNotes.Count; i++)
                {
                    //Debug.Log("i = " + i);
                    if (tc.allNotes[i].GetComponent<Note>().noteType != "bomb" && i != tc.newStartingInt)
                    {
                        //Debug.Log(i);
                        tc.oldNewStartingNoteAccum = tc.newStartingNoteAccum;
                        tc.newStartingNoteAccum = tc.allNotes[i].GetComponent<Note>().beatWaitCur;
                        //Debug.Log("tc.newStartingNoteAccum" + tc.newStartingNoteAccum);
                        tc.newStartingInt = i;

                        break;
                    }
                }
                //tc.beatWaitNextNote = 0;
                //tc.beatWaitAccum = 0;
             
                player.CalculateMissPointFrom2();
            }
            //Debug.Break();
            tc.nextIndex3++;
            //tc.beatWaitNextNote += tc.beatWaitCount[tc.nextIndex3];

            QueueEndOfTrack();

            // If this note is behind the player, turn behindPlayer to true
            if (player.transform.position.z > transform.position.z)
            {
                behindPlayer = true;
            }

            // Currently trying to make it so the player's accuracy is correct, even if there are bombs in the way
        }
    }

    void CheckBombHitPlayer()
    {
        if (noteType == "bomb" && !bombHitPlayer)
        {
            float dist = Mathf.Abs(this.gameObject.transform.position.z - player.transform.position.z);
            if (dist <= gm.bombHitRange && laneNumber == player.nearestLaneNumber)
            {
                bombHitPlayer = true;
                player.Missed(true);
                Debug.Break();
            }
        }

    }

    void QueueEndOfTrack()
    {
        tc.trackPosIntervalsList2.RemoveAt(0);

        // If this is the last note of the track, end differently
        if (noteNumber == tc.noteLanes.Count)
        {
            gm.EndTrackNote();
        }
            
        // If this is NOT the last note of the track, end usually
        else
        {
            //player.DestroyFurthestNote();
            StartCoroutine("DestroyNote");
        }

    }

    public IEnumerator DestroyNote()
    {
        yield return new WaitForSeconds(0.8f);
        // remove this note to the 'activeNotes' list
        player.activeNotes.Remove(this.gameObject.transform);
        // remove this note from the 'noteBehind' list
        player.notesInfront.Remove(this.gameObject.transform);
        // remove this note from the 'furthestbehindnote' variable
        player.furthestBehindNote = null;

        tc.notes.Remove(this.gameObject);

        //player.DestroyFurthestNoteNote();

        Destroy(this.gameObject);

    }
}

    
