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
    //[HideInInspector]
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

    private bool hasBeenMissed;

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
    public bool assignedElectricity;
    public LineRenderer lr;
    public GameObject lrObj;
    public bool electricityHasTurnedOff;

    [Header("Bomb")]
    public GameObject bombObj;
    public bool bombHitPlayer;
    public List<int> nextBombLane = new List<int>();
    public bool clone;

    [Header("Slider")]
    public Note nextNoteScript;
    public string prevNoteNoteDir;
    public string prevNoteNoteType;
    public int indexInNotes;
    public bool usedAsSlider;
    private float sliderInterval;
    public List<Vector3> sliderintervals = new List<Vector3>();
    private int indexOfSliderNoteSet;
    public LineRenderer sliderLr;
    public Slider sliderScript;

    private bool doneOnce6;
    public bool isEndOfSlider;
    public bool isStartOfSlider;
    public GameObject sliderEdge;
    public GameObject sliderPredict;
    SliderInterval2 sliderPredictScript;

    public GameObject notePredict;
    SliderInterval2 notePredictScript;

    [Header("Tutorial")]
    public int tutStage = -1;
    public bool tutResetNote;
    public bool finalNoteInStage;
    public float beatWaitCurFN;
    public bool firstNoteInStage;
    public float beatWaitCurLN;
    public float beatWaitNewSet;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();
        noteRend = meshRendererRef.GetComponent<Renderer>();
        noteWidth = noteRend.bounds.size.z;


        if (!clone && noteType != "slider")
        {
            if (noteType != "bomb")
            {
                tc.notesSpawned++;
            }
        }

        if (noteType != "bomb")
        {
            gm.totalNotes++;
            noteNumber = gm.totalNotes;
        }

        gm.totalAllNotes++;

        if (tc.notesSpawned == 1)
        {
            tc.nextNoteInBeats = tc.selectedMap.noteTimeTaken + beatWait;
        }

        if (nextBombLane.Count > 0)
        {
            // Spawn a bomb note if this note shares a beatWait with another
            for (int i = 0; i < nextBombLane.Count; i++)
            {
                GameObject go = Instantiate(tc.noteVisual, tc.notesObj.transform.position, Quaternion.identity);
                Note noteScript = go.GetComponent<Note>();
                noteScript.clone = true;
                noteScript.beatWait = beatWait;
                noteScript.beatWaitCur = beatWaitCur;
                noteScript.laneNumber = nextBombLane[i];
                noteScript.noteType = noteType;
                noteScript.noteDir = noteDir;
                noteScript.noteNumber = noteNumber;
                noteScript.gameObject.name = noteScript.laneNumber + " Clone";

                // move the note to the correct lane
                go.transform.position = new Vector3(tc.path.pathWidth * (nextBombLane[i] - 1),
                    0.02f, pm.initialPath.GetComponent<Path>().pathLength);

                noteScript.startingPos.z = go.transform.position.z;

                noteScript.pathWidth = tc.path.pathWidth * (nextBombLane[i] - 1);

                // Allow the note to move when ready
                noteScript.canMove = true;

                // Get the index count of this note for reference
                int indexCount = this.gameObject.transform.GetSiblingIndex();

                // Set the bomb to the correct order in the children count
                go.transform.SetParent(tc.notesObj.transform);
                go.transform.SetSiblingIndex(indexCount + 1);
            }
        }

        // Find the index of this note in the notes list
        indexInNotes = tc.notes.IndexOf(gameObject);
                                                                                                                              
        if (tc.notes.Count >= 3)
        {                                               
            nextNoteScript = tc.notes[indexInNotes + 1].GetComponent<Note>();
            nextNoteScript.prevNoteNoteDir = noteDir;
            nextNoteScript.prevNoteNoteType = noteType;
        }

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
                if (noteType == "slider")
                {
                    // Set the note visual to invisible
                    noteObject.SetActive(false);

                    // Disable the spotlight
                    spotLight.SetActive(false);

                    // Disable the hit marker
                    hitMarkerCanvas.SetActive(false);                

                    // If the note before this note is not a slider OR the end of a slider, continue
                    if (prevNoteNoteType != "slider" || prevNoteNoteDir == "down")
                    {
                        Debug.Log("spawning slider");
                        isStartOfSlider = true; 

                        sliderLr = Instantiate(gm.sliderRef, transform.position, Quaternion.identity);
                        gm.sliders.Add(sliderLr.gameObject.transform);

                        sliderScript = sliderLr.gameObject.GetComponent<Slider>();
                        sliderScript.gm = gm;
                        sliderScript.tc = tc;
                        sliderLr.SetPosition(0, new Vector3(0, 0, 70));
                        sliderLr.SetPosition(1, new Vector3(0, 0, 70));

                        sliderLr.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);

                        sliderScript.indexOfSliderNoteSet++;
                        indexOfSliderNoteSet = (int)sliderScript.indexOfSliderNoteSet;
                        CreateSliderIntervals();
                    }
                    else
                    {
                        sliderLr = tc.notes[indexInNotes - 1].gameObject.GetComponent<Note>().sliderLr;
                        sliderScript = tc.notes[indexInNotes - 1].gameObject.GetComponent<Note>().sliderScript;
                        sliderScript.indexOfSliderNoteSet++;
                        indexOfSliderNoteSet = (int)sliderScript.indexOfSliderNoteSet;
                        CreateSliderIntervals();
                    }

                    // Insantiate predict transform
                    GameObject go = Instantiate(sliderPredict, transform.position, Quaternion.identity);
                    sliderPredictScript = go.GetComponent<SliderInterval2>();

                    sliderPredictScript.player = player;
                    sliderPredictScript.gm = gm;
                    sliderPredictScript.slider = sliderScript;
                    sliderPredictScript.parent = transform;
                    sliderPredictScript.noteScript = this;

                    go.transform.SetParent(gameObject.transform);
                    go.transform.localPosition = new Vector3(0, 0, -gm.sliderOffset);

                    if (indexInNotes + 1 < tc.notes.Count)
                    {
                        if (tc.notes[indexInNotes + 1].gameObject.GetComponent<Note>().noteType != "slider")
                        {
                            break;
                        }

                        if (tc.notes[indexInNotes + 1].gameObject.GetComponent<Note>().noteType == "slider")
                        {
                            if (!sliderScript.setOfSliderNotes.Contains(gameObject.transform))
                            {
                                sliderScript.setOfSliderNotes.Add(gameObject.transform);
                            }
                            if (!sliderScript.setOfSliderNotes.Contains((tc.notes[indexInNotes + 1].gameObject.transform)))
                            {
                                sliderScript.setOfSliderNotes.Add(tc.notes[indexInNotes + 1].gameObject.transform);
                            }
                        }
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

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

                    // Disable the hit marker
                    hitMarkerCanvas.SetActive(false);

                    // Insantiate predict transform
                    GameObject go = Instantiate(sliderPredict, transform.position, Quaternion.identity);
                    sliderPredictScript = go.GetComponent<SliderInterval2>();

                    sliderPredictScript.fromBomb = true;
                    sliderPredictScript.note = gameObject;
                    sliderPredictScript.player = player;
                    sliderPredictScript.gm = gm;
                    sliderPredictScript.slider = sliderScript;
                    sliderPredictScript.parent = transform;

                    go.transform.SetParent(gameObject.transform);
                    go.transform.localPosition = new Vector3(0, 0, -gm.sliderOffset);
                    
                }
                break;

            case "down":
                {
                    // Set the note visual to invisible
                    noteObject.SetActive(false);

                    // Disable the spotlight
                    spotLight.SetActive(false);

                    // Disable the hit marker
                    hitMarkerCanvas.SetActive(false);

                    sliderLr = tc.notes[indexInNotes - 1].gameObject.GetComponent<Note>().sliderLr;
                    sliderScript = tc.notes[indexInNotes - 1].gameObject.GetComponent<Note>().sliderScript;

                    sliderScript.indexOfSliderNoteSet++;
                    indexOfSliderNoteSet = (int)sliderScript.indexOfSliderNoteSet;
                    CreateSliderIntervals();

                    isEndOfSlider = true;
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
            //hitMarkerCanvas.GetComponent<LookAtCam>().enabled = true;

            // Disable the spotlight
            spotLight.SetActive(false);

            // Move electricity end to the ground of this note
            Vector3 pos = Vector3.zero;
            pos.y = 0;
            ElectrictyEnd.localPosition = pos;
        }

        // Enable slider edge if this note is a slider...
        // Change size depending if this is the start/end of a slider
        if (noteType == "slider")
        {
            sliderEdge.SetActive(true);
            if (isStartOfSlider || isEndOfSlider)
            {
                sliderEdge.transform.localScale = new Vector3(2.4f, .035f, 2.4f);
            }
            else
            {
                sliderEdge.transform.localScale = new Vector3(1.7f, .035f, 1.7f);
            }
        }
    }

    // Not currently being used
    // Purpose - Was used to check if the player was close enough to notes specifically for sliders
    void CheckIfMissedSlider()
    {
        if (player.nearestLaneNumber != laneNumber && !player.nearestSliderStartEnd.gameObject.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().missed)
        {
            player.Missed(false, this, gameObject.name);
            player.nearestSliderStartEnd.gameObject.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().missedOn = true;
            player.nearestSliderStartEnd.gameObject.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().Missed();
        }
    }

    void CheckToPlayHitSound()
    {
        if (noteType != "bomb" && noteType != "blast" && !missed)
        {
            if (noteType == "slider" && !player.nearestSliderStartEnd.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().missed)
            {
                gm.am.PlaySound("NoteHit");
                return;
            }
            else if (noteType != "slider")
            {
                gm.am.PlaySound("NoteHit");
                return;
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove || gm.tutPaused)
        {
            return;
        }

        UpdateSliderLocation();

        if (noteDir == "up" && noteType != "bomb")
        {
            player.DoNoteEffectUp(this);
        }

        // Happens once when canMove is triggered true
        if (canMove && !doneOnce)
        {
            if (noteType != "bomb")
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
                //player.activeNotes.Add(this.gameObject.transform);
                player.activeNotes.Add(this.gameObject.transform);
            }
            else
            {
                doneOnce = true;
                startTime = tc.trackPos;
            }
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

            //tc.notes.Remove(this.gameObject);

            if (player.closestNoteInFront == this.gameObject)
            {
                player.closestNoteInFront = null;
            }

            if (player.closestNoteInFrontScript == this)
            {
                player.closestNoteInFrontScript = null;
            }

        }

        ReachedPlayer();
    }
    
    void ReachedPlayer()
    {
        if (doneOnce2 && !doneOnce3)
        {
            doneOnce3 = true;
            gm.notesPassedPlayer++;

            CheckToPlayHitSound();

            //Debug.Log(177 - gm.notesLeftInfront);

            if (noteType != "bomb")
            {
                gm.notesLeftInfront--;

                tc.previousNoteBeatTime3 = tc.nextNoteInBeats;
            }

            if (noteDir != "up" && noteDir != "down")
            {
                gm.lrs.Remove(lr);
                Destroy(lrObj);
                player.electricNotes.RemoveAt(0);
            }

            if (!clone)
            {
                tc.nextIndex3++;
            }

            QueueEndOfTrack();

            // If this note is behind the player, turn behindPlayer to true
            if (player.transform.position.z > transform.position.z)
            {
                behindPlayer = true;
            }
        }
    }

    private void Update()
    {
        if (missed && noteType != "blast" && noteType != "slider" && !hasBeenMissed)
        {
            hasBeenMissed = true;
            transform.GetChild(1).GetComponentInChildren<Image>().color = gm.missedNoteC;
            spotLight.gameObject.SetActive(false);
        }

        if (missed && noteType == "blast" && !hasBeenMissed)
        {
            hasBeenMissed = true;
            hitMarker.GetComponent<Image>().color = gm.missedNoteC;
        }
    }

    void UpdateSliderLocation()
    {
        if (sliderLr)
        {
            if (sliderScript.setOfSliderNotes.Count >= 1)
            {
                List<Vector3> temp = new List<Vector3>();

                foreach (Transform note in sliderScript.setOfSliderNotes)
                {
                    if (note)
                    {
                        temp.Add(note.transform.position);
                    }
                }

                Vector3[] posOfPoints;
                posOfPoints = temp.ToArray();

                sliderLr.positionCount = posOfPoints.Length;
                sliderLr.SetPositions(posOfPoints);
            }
        }
    }

    void CreateSliderIntervals()
    {
        if (sliderLr.positionCount >= 2)
        {
            for (int i = 0; i < gm.sliderIntervalCount; i++)
            {
                sliderInterval += (1f / (gm.sliderIntervalCount + 1));

                //Debug.Log(i);
                //Debug.Log(sliderScript.indexOfSliderNoteSet + " indexofslidernoteset");
                //Debug.Log(sliderLr.positionCount + " positionCount");

                if (sliderScript.indexOfSliderNoteSet > sliderLr.positionCount + 1)
                {
                    break;
                }
                else
                {
                    // Create the interval GO
                    gm.sliderIntervalCountGo++;

                    GameObject go = Instantiate(gm.sliderIntervalRef, transform.position, Quaternion.identity);
                    go.transform.SetParent(gm.sliderIntervalPar.transform);
                    go.transform.localPosition = new Vector3(0, 0, -gm.sliderOffset);

                    SliderInterval goScript = go.GetComponent<SliderInterval>();

                    go.name = "SliderInterval " + sliderInterval.ToString("F2") + " " + gm.sliderIntervalCountGo;
                    goScript.gm = gm;
                    goScript.note = this;
                    goScript.player = player;
                    goScript.sliderInterval = sliderInterval;
                    goScript.slider = sliderScript;

                    sliderScript.allSliderIntervals.Add(go.transform);
                    sliderScript.sliderIntervalsInFront.Add(go.transform);

                    // Set the start location for the spawned transform
                    goScript.sliderStartCount = sliderScript.indexOfSliderNoteSet - 2;
                    // Set the end location for the spawned transform
                    goScript.sliderEndCount = sliderScript.indexOfSliderNoteSet - 1;

                    if (sliderInterval > 1 - (1 / (gm.sliderIntervalCount + 1)))
                    {
                        sliderInterval = 0;
                        break;
                    }
                }

            }
        }
    }

    void QueueEndOfTrack()
    {
        if (clone)
        {
            StartCoroutine("DestroyNote");
            return;
        }

        tc.trackPosIntervalsList2.RemoveAt(0);

        // If this is the last note of the track, queue the post statistics screen
        if (gm.notesLeftInfront <= 0)
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
            
        if (!clone)
        {
            // remove this note from the 'furthestbehindnote' variable
            gm.playerScript.closestBehindNote = null;
        }

        // If this note is apart of a slider note
        if (noteType == "slider" && sliderLr)
        {
            int index = sliderScript.setOfSliderNotes.IndexOf(gameObject.transform);

            // Spawn an EGO and set it's position to the same as this note when it reaches the player
            GameObject go = new GameObject();
            go.transform.SetParent(gm.sliderTransformPar.transform);
            go.transform.position = gameObject.transform.position;

            //Instantiate a new slider edge for the fake note so that it can change color when the player fails the slider
            GameObject seGO = Instantiate(sliderEdge, transform.position, Quaternion.identity);
            seGO.transform.SetParent(go.transform);

            if (sliderScript.setOfSliderNotes.Contains(gameObject.transform))
            {
                // Replace this note with the EGO to replace to transform...
                // This is so the slider stays at a point a note cannot be in 
                sliderScript.setOfSliderNotes.Insert(index, go.transform);

                // Remove the note from the list
                sliderScript.setOfSliderNotes.Remove(gameObject.transform);
            }
        }

        if (noteType != "bomb")
        {
            // remove this note to the 'activeNotes' list
            player.activeNotes.Remove(this.gameObject.transform);
        }


        // remove this note from the 'noteBehind' list
        player.notesInfront.Remove(this.gameObject.transform);
        tc.notes.Remove(this.gameObject);

        Destroy(this.gameObject);
    }
}

    
