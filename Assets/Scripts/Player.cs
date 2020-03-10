using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Gamemode gm;
    private PathManager pm;
    private TrackCreator tc;
    private CameraBehaviour cm;

    private float pathWidth;

    public bool movingRight;
    public bool movingLeft;
    private bool holdingMovingRightInp;
    private bool holdingMovingLeftInp;
    public bool blastInput;
    public bool aboutToBlast;
    //[HideInInspector]
    public int nearestLaneNumber;
    //[HideInInspector]
    public int oldNearestLaneNumber;

    bool hitLLaunch;
    bool hitRLaunch;

    private Renderer rend;
    public float playerWidth;

    public bool hitNote;
    bool doneOnce;
    bool doneOnce3;
    public bool doneOnce2;


    //[HideInInspector]
    public float missPointFrom;
    public float pointTo;
    //[HideInInspector]
    public float pointFrom;

    public bool isShielding;

    private float currentPointInBeats;
    private float missCurrentPointInBeats;

    public List<Transform> activeNotes = new List<Transform>();
    public List<Transform> notesInfront = new List<Transform>();
    public GameObject OldClosestNoteInFront;
    public GameObject closestNoteInFront;
    public Note closestNoteInFrontScript;

    // the nearest note of any type
    public Transform nearestAnyNote;
    // the nearest NON bomb note
    public Transform nearestNote;
    public GameObject furthestBehindNote;

    public Material shieldMat;
    private Color shieldColor;
    public GameObject shield;

    public float newPerfect;
    public float newGreat;
    public float newGood;
    public float newGoodMiss;
    private float oldNewGoodMiss;

    public Note nearestAnyNoteScript;
    public Note nearestNoteScript;

    public Text accuracyUI;

    public bool validMovement;

    [HideInInspector]
    public Vector3 playerPos;
    [HideInInspector]
    public Vector3 oldPlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        cm = Camera.main.GetComponent<CameraBehaviour>();

        rend = GetComponentInChildren<Renderer>();
        playerWidth = rend.bounds.size.z;

        pathWidth = pm.initialPath.GetComponent<Path>().pathWidth;

        ShieldReset();
    }

    public void RepositionPlayer()
    {
        playerPos.y = .8f;
        playerPos.x = 3;
        transform.position = playerPos;

        pm.FindNearestPath(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.tutPaused)
        {
            return;
        }
        Inputs();
        StartCoroutine("Movement");
        FindNearestNote();
        UpdateShield();
        AssignMisses();
        //SetNearestLaneNumber();
        UpdateNotesInfFront();
        IncMaxAcc();

        missCurrentPointInBeats = tc.trackPosInBeatsGame;

        if (notesInfront.Count > 0)
        {
            closestNoteInFront = notesInfront[0].gameObject;
        }
    }

    void AssignMisses()
    {
        // If there is a note alive 
        if (nearestAnyNote)
        {
            //nearestNoteScript = nearestAnyNote.gameObject.GetComponent<Note>();
            // Update this just in case? probably do not need this here
            nearestAnyNoteScript = nearestAnyNote.gameObject.GetComponent<Note>();
            //nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();

            // Calculate whether the player is far enough from the nearest note to determine that it's too late to get 'good' or higher score from it
            CalculateMissPointFrom();

            // If there is a note behind the player and it has not been hit yet AND
            // the note hasn't been missed yet AND
            // the player is unable to hit the note anymore give a miss to the player

            //Debug.Log("---------------------------------------------------------");
            //Debug.Log("nearestNote.transform.position.z " + nearestNote.transform.position.z);
            //Debug.Log("transform.position.z " + transform.position.z);
            //Debug.Log("nearestNoteScript.canGetNote " + nearestNoteScript.canGetNote);
            //Debug.Log("nearestNoteScript.noteType " + nearestNoteScript.noteType);
            //Debug.Log("missPointFrom " + missPointFrom);
            //Debug.Log("newGood " + newGood + 0.05);
            if (missPointFrom > newGoodMiss && nearestNoteScript.canGetNote)
            {
                if (nearestNote.transform.position.z < transform.position.z)
                {
                    Missed(false);
                    //Debug.Break();
                }
            }
        }
    }

    void IncMaxAcc()
    {
        if (!nearestNote)
        {
            return;
        }

        newGoodMiss = gm.goodMin / (nearestNoteScript.beatWait / gm.accuracy);

        if (1 - missPointFrom < newGoodMiss + 0.05f && nearestNote.transform.position.z > transform.position.z && !nearestNoteScript.noteCalculatedAcc && nearestNoteScript.noteType != "bomb")
        {
            nearestNoteScript.noteCalculatedAcc = true;

            // For every note, increase the max accuaracy by 3. (3 is the value perfect gives)
            gm.totalAccuracyMax += 3;
        }
    }

    public void CalculateMissPointFrom()
    {
        if (!tc.trackInProgress)
        {
            return;
        }

        if (tc.nextIndex3 <= tc.noteLanes.Count)
        {
            // i think this is for the last note of the map
            if (nearestNoteScript.noteNumber == tc.noteLanes.Count && nearestNoteScript.behindPlayer)
            {
                tc.nextNoteInBeats = tc.previousNoteBeatTime3 + 2;
            }

            if (nearestNoteScript.noteNumber == tc.noteLanes.Count)
            {
                missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
            }

            missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
        }
    }

    public void CalculateMissPointFrom2()
    {
        tc.nextNoteInBeats = tc.previousNoteBeatTime3 + (tc.newStartingNoteAccum - tc.oldNewStartingNoteAccum);
        //Debug.Log("NextNoteInBeats = " + tc.nextNoteInBeats + " (" + tc.previousNoteBeatTime3 + " + " + tc.newStartingNoteAccum + " + " + tc.oldNewStartingNoteAccum + ")");
    }

    void UpdateShield()
    {
        shieldMat.SetFloat("Vector1_A7E2E21E", gm.shieldOpacity);
        shieldColor = gm.shieldColor;
        shieldMat.SetColor("Color_58F8661B", shieldColor * gm.shieldEmissionInc);
    }

    void ShieldReset()
    {
        // Ensure that the shield is off when the game starts
        shieldMat.SetFloat("Vector1_A7E2E21E", 0);
        shieldMat.SetColor("Color_58F8661B", shieldColor * 0);
    }
    public void DestroyFurthestNoteNote()
    {
        Invoke("DestroyFurthestNote", 0.2f);
    }
    void FindNearestNote()
    {
        float minDist = Mathf.Infinity;

        // Detect the nearest note's position
        foreach (Transform t in activeNotes)
        {
            float dist = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            if (minDist > dist)
            {
                nearestAnyNote = t;
                minDist = dist;
            }
        }

        if (nearestAnyNote)
        {
            nearestAnyNoteScript = nearestAnyNote.gameObject.GetComponent<Note>();
        }

        float minDist2 = Mathf.Infinity;
        // Detect the nearest note's position
        foreach (Transform t in activeNotes)
        {
            float dist2 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            if (minDist2 > dist2 && t.GetComponent<Note>().noteType != "bomb")
            {
                nearestNote = t;
                minDist2 = dist2;
            }
        }

        if (nearestNote)
        {
            nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();
        }
    }
    public void UpdateNotesInfFront()
    {
        // If the track is not in progress
        // OR there is less then 1 active note, stop
        if (!tc.trackInProgress || activeNotes.Count < 1)
        {
            return;
        }

        // Add a note to a list if it's infront of the player
        //  If the note is infront of the player, continue
        if (activeNotes[activeNotes.Count - 1].gameObject.transform.position.z > transform.position.z)
        {
            // Only add the note if it NOT already in the list
            if (!notesInfront.Contains(activeNotes[activeNotes.Count - 1].gameObject.transform) && 
                activeNotes[activeNotes.Count - 1].gameObject.GetComponent<Note>().noteType != "bomb")
            {
                notesInfront.Add(activeNotes[activeNotes.Count - 1].gameObject.transform);
            }
        }

        if (notesInfront.Count > 0 && closestNoteInFront)
        {
            if (OldClosestNoteInFront != closestNoteInFront)
            {
                OldClosestNoteInFront = closestNoteInFront;
                closestNoteInFrontScript = closestNoteInFront.GetComponent<Note>();
            }
        }

        float FurthestDistance = 0;
        foreach (Transform Object in activeNotes)
        {
            float ObjectDistance = Vector3.Distance(transform.position, Object.transform.position);
            if (ObjectDistance > FurthestDistance && Object.gameObject.transform.position.z < transform.position.z)
            {
                furthestBehindNote = Object.gameObject;
                FurthestDistance = ObjectDistance;
            }
        }
    }
    void Inputs()
    {
        // This is a check for when both moving right and left movement are held
        if (Input.GetKey(KeyCode.L))
        {
            holdingMovingRightInp = true;
        }

        else
        {
            holdingMovingRightInp = false;
        }

        // This is a check for when both moving right and left movement are held
        if (Input.GetKey(KeyCode.A))
        {
            holdingMovingLeftInp = true;
        }

        else
        {
            holdingMovingLeftInp = false;
        }

        // If:
        //      player is pressing for moving right
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        //      blastInput is false
        //      player is shielding
        if (Input.GetKeyUp(KeyCode.L) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber && !blastInput)
        {
            movingRight = true;
        }

        // If:
        //      player is pressing for moving left
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        //      blastInput is false
        //      player is shielding
        if (Input.GetKeyUp(KeyCode.A) && !movingRight && !movingLeft && nearestLaneNumber != 1 && !blastInput)
        {
            movingLeft = true;
        }

        // If both the input for moving left and right are held down
        // do not allow the player to move left or right after they are let go TOGETHER
        if (holdingMovingLeftInp && holdingMovingRightInp)
        {
            blastInput = true;
        }

        // Blast action
        else if (!holdingMovingLeftInp && !holdingMovingRightInp && blastInput && isShielding)
        {
            blastInput = false;
            AssignFromAndToValuesBlast();
            //Debug.Break();
        }

        else if (!holdingMovingLeftInp && !holdingMovingRightInp)
        {
            blastInput = false;
        }

        // if the play inputs to stop shielding stop shielding
        if (Input.GetKey(KeyCode.Space))
        {
            isShielding = false;
        }

        // Shield is active when the shield button is not being held down
        else
        {
            isShielding = true;
        }
    }
    public void SetNearestLaneNumber(bool forScore, GameObject nearestPath)
    {
        // If not moving, AND there is a nearest path to begin with
        // Update the oldNearestLaneNumber and nearestLaneNumber accordingly

        oldNearestLaneNumber = nearestLaneNumber;
        nearestLaneNumber = nearestPath.GetComponent<Path>().laneNumber;
        //Debug.Log("nearestLaneNumber " + nearestLaneNumber);
        //Debug.Break();
        if (forScore)
        {
            AssignFromAndToValuesNoteAndLaunch();
        }
    }
    IEnumerator Movement()
    {
        // Stops strange drifting behaviour
        Vector3 playerPos2 = transform.position;
        playerPos2.z = 1;
        transform.position = playerPos2;

        playerPos = transform.position;

        // Ensures that there is a nearest path to begin with
        // There must be one for this code to work. 
        if (!pm.nearestPath)
        {
            yield return 0;
        }

        #region Normal - Moving Right
        // Functionality of moving right with shield
        if (movingRight && isShielding && tc.selectedMap.title != "Tutorial" || gm.tutorialStage >= 3 && movingRight && isShielding)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = true;

            playerPos.x = pathWidth * nearestLaneNumber;
            transform.position = playerPos;

            pm.FindNearestPath(true);
        }

        // Functionality of moving right without shield
        else if (movingRight && !isShielding && tc.selectedMap.title != "Tutorial" || gm.tutorialStage >= 3 && movingRight && !isShielding)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x = pathWidth * nearestLaneNumber;
            transform.position = playerPos;

            pm.FindNearestPath(true);
        }
        #endregion 
        #region Tutorial - Moving Right
        // Functionality of moving right with shield during first few stages of tutorial
        if (movingRight && isShielding && tc.selectedMap.title == "Tutorial" && gm.tutorialStage > 0 && gm.tutorialStage < 3)
        {
            //Debug.Log("2");
            movingRight = false;
            movingLeft = false;
            validMovement = true;

            pm.FindNearestPath(false);
            //Debug.Log("called nearestlanenumberfunction");

            playerPos.x = pathWidth * nearestLaneNumber;
            transform.position = playerPos;

            pm.FindNearestPath(true);
            //Debug.Log("called nearestlanenumberfunction");

            yield return new WaitForSeconds(gm.timeForMoveBack);

            playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
            //Debug.Log("called nearestlanenumberfunction");
        }

        // Functionality of moving right WITHOUT shield during first few stages of tutorial
        if (movingRight && !isShielding && tc.selectedMap.title == "Tutorial" && gm.tutorialStage > 0 && gm.tutorialStage < 3)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x = pathWidth * nearestLaneNumber;
            transform.position = playerPos;

            pm.FindNearestPath(true);

            yield return new WaitForSeconds(gm.timeForMoveBack);

            playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }

        #endregion
        #region Normal - Moving Left
        // Functionality of moving left with shield
        if (movingLeft && isShielding && tc.selectedMap.title != "Tutorial" || gm.tutorialStage >= 3 && movingLeft && isShielding)
        {
            //Debug.Log("1");
            movingLeft = false;
            movingRight = false;
            validMovement = true;

            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(true);
        }

        // Functionality of moving left without shield
        if (movingLeft && !isShielding && tc.selectedMap.title != "Tutorial" || gm.tutorialStage >= 3 && movingLeft && !isShielding)
        {
            movingLeft = false;
            movingRight = false;
            validMovement = false;

            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(true);
        }
        #endregion
        #region Tutorial - Moving Left
        // Functionality of moving left with shield during first few stages of tutorial
        if (movingLeft && isShielding && tc.selectedMap.title == "Tutorial" && gm.tutorialStage > 0 && gm.tutorialStage < 3)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = true;

            pm.FindNearestPath(false);
            //Debug.Log("called nearestlanenumberfunction");

            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(true);
            //Debug.Log("called nearestlanenumberfunction");

            yield return new WaitForSeconds(gm.timeForMoveBack);

            playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
            //Debug.Log("called nearestlanenumberfunction");
        }

        // Functionality of moving left without shield during first few stages of tutorial
        if (movingLeft && !isShielding && tc.selectedMap.title == "Tutorial" && gm.tutorialStage > 0 && gm.tutorialStage < 3)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(true);

            yield return new WaitForSeconds(gm.timeForMoveBack);

            playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }
        #endregion
    }
    void AssignFromAndToValuesBlast()
    {
        currentPointInBeats = tc.trackPosInBeatsGame;

        pointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, currentPointInBeats)));
        pointTo = 1 - pointFrom;
        gm.scoreIncreased = true;

        CheckForBlastEffect();
    }
    void AssignFromAndToValuesNoteAndLaunch()
    {
        currentPointInBeats = tc.trackPosInBeatsGame;

        //pointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, currentPointInBeats)));
        //pointTo = 1 - pointFrom;

        pointFrom = missPointFrom;
        pointTo = 1 - pointFrom;
        gm.scoreIncreased = true;
        //Debug.Log("``````````````````````");
        //Debug.Log("nextNoteInBeats3 = " + tc.nextNoteInBeats3);
        //Debug.Log("previousNoteBeatTime3 = " + tc.previousNoteBeatTime3);
        //Debug.Log("currentPointInBeats = " + currentPointInBeats);
        //Debug.Log("pointFrom " + pointFrom);
        //Debug.Log("pointTo " + pointTo);
        CheckForNoteAndLaunchEffect();
        //Debug.Break();
    }
    private void CheckForBlastEffect()
    {
        if (!nearestNote)
        {
            return;
        }

        // if the nearest not has already been hit once, miss the player for attempt
        if (nearestNoteScript.hitAmount > 1 && !nearestNoteScript.missed)
        {
            Missed(false);
        }

        nearestNoteScript.hitAmount++;

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteScript.canGetNote)
        {
            return;
        }

        nearestNoteScript.canGetNote = false;

        // in the case of a blast
        if (nearestNoteScript.noteType == "blast")
        {
            CheckHitAccuracy();
            return;
        }
        else
        {
            Missed(false);
        }
    }
    private void CheckForNoteAndLaunchEffect()
    {
        //Debug.Log("nearestNoteScript.noteDir " + nearestNoteScript.noteDir);
        //Debug.Log("nearestNoteScript.noteType " + nearestNoteScript.noteType);
        //Debug.Log("nearestLaneNumber " + nearestLaneNumber);
        //Debug.Log("nearestNoteScript.laneNumber " + nearestNoteScript.laneNumber);
        //Debug.Log("movingLeft " + movingLeft);
        //Debug.Log("movingRight " + movingRight);
        //Debug.Log("pointTo " + pointTo);
        //Debug.Log("pointFrom " + pointFrom);
        //Debug.Break();
        //Debug.Log("pointMin " + gm.goodMin);

        //Debug.Log(nearestNote.GetComponent<Note>().canGetNote);

        // if the nearest not has already been hit once, miss the player for attempt
        // OR if the movement that was made was made without the shield on
        if (!nearestNote || !validMovement)
        {
            return;
        }

        if (nearestNoteScript.hitAmount > 1 && !nearestNoteScript.missed)
        {
            Missed(false);
        }

        nearestNoteScript.hitAmount++;

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteScript.canGetNote)
        {
            return;
        }

        nearestNoteScript.canGetNote = false;

        // If an attempt on the up note was made, instantly miss the note
        if (nearestNoteScript.noteType == "blast")
        {
            Missed(false);
        }

        // If an attempt on the up note was made, instantly miss the note
        if (nearestNoteScript.noteDir == "up" && nearestNoteScript.noteType != "blast")
        {
            Missed(false);
        }

        // In the case of a note with a left arrow
        if (nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "launch" && nearestNoteScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteScript.laneNumber == oldNearestLaneNumber - 1 && nearestNoteScript.laneNumber == nearestLaneNumber)
            {
                //Debug.Log("1");
                //Debug.Log("oldNearestLaneNumber " + oldNearestLaneNumber);
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                return;
            }
        }
        // In the case of a note with a right arrow
        else if (nearestNoteScript.noteDir == "right" && nearestNoteScript.noteType != "launch" && nearestNoteScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteScript.laneNumber == oldNearestLaneNumber + 1 && nearestNoteScript.laneNumber == nearestLaneNumber)
            {
                //Debug.Log("2");
                //Debug.Log("oldNearestLaneNumber " + oldNearestLaneNumber);
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                return;
            }
        }

        // In the case of a launch with a right arrow
        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "right" && nearestNoteScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteScript.laneNumber == oldNearestLaneNumber + 1 && nearestNoteScript.laneNumber == nearestLaneNumber)
            {
                hitRLaunch = true;
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                return;
            }
        }

        // in the case of a launch with a left arrow
        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue 
            if (nearestNoteScript.laneNumber == oldNearestLaneNumber - 1 && nearestNoteScript.laneNumber == nearestLaneNumber)
            {
                hitLLaunch = true;
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                return;
            }
        }
    }
    public void DoNoteEffectUp()
    {
        // If there is no note active in the game, stop
        if (!nearestNote)
        {
            return;
        }

        // If the up arrow that is nearest has already given score, stop
        if (nearestNoteScript.doneUpArrow)
        {
            return;
        }

        // If the nearest up note has not been hit yet, and the player is shielding
        if (nearestNoteScript.noteDir == "up" && isShielding && nearestNoteScript.hitAmount == 0 && nearestNoteScript.noteType != "bomb" && nearestNoteScript.noteType != "blast")
        {
            // If the note is behind the player
            // AND the note is in the same note as the player
            if (nearestNoteScript.gameObject.transform.position.z < transform.position.z
                && nearestLaneNumber == nearestNoteScript.laneNumber)
            {
                // Tell the note that it has just given score, so it knows not to allow it again in the future
                nearestNoteScript.doneUpArrow = true;

                // Give 'perfect' score
                HitPerfect();

                // Increase the hit amounts of the up note by 1
                nearestNoteScript.hitAmount++;

                // Make it so the up arrow can not give score again once done once aready
                nearestNoteScript.canGetNote = false;
                //Debug.Break();
            }
        }
    }
    private void CheckHitAccuracy()
    {
        // If the nearest note has already given score once, stop.
        if (nearestNoteScript.hitAmount > 1)
        {
            return;
        }

        // Assign the new thresholds for accuracy based on how far the notes are from eachother
        newPerfect = gm.perfectMin / (nearestNoteScript.beatWait / gm.accuracy);
        newGreat = gm.greatMin / (nearestNoteScript.beatWait / gm.accuracy);
        newGood = gm.goodMin / (nearestNoteScript.beatWait / gm.accuracy);
        //Debug.Break();
        //Debug.Log("-------------------");
        //Debug.Log("newPerf " + newPerfect);
        //Debug.Log("newGreat " + newGreat);
        //Debug.Log("newGood " + newGood);
        //Debug.Log("POINTFROM " + pointFrom);
        //Debug.Log("POINTTO " + pointTo);

        if (tc.trackPosInBeatsGame < (tc.selectedMap.noteTimeTaken + tc.firstInterval))
        {
            CheckFirstHitAccuracy();
        }

        // If the player is closer to the previous note. 
        // track the distance between the next note and the current note
        if (pointTo > pointFrom && tc.trackPosInBeatsGame > (tc.selectedMap.noteTimeTaken + tc.firstInterval))
        {
            if (pointFrom <= newPerfect && pointFrom >= 0)
            {
                //Debug.Log("Perfect 1");
                //Debug.Log(newPerfect);
                HitPerfect();
            }
            else if (pointFrom <= newGreat && pointFrom >= newPerfect)
            {
                //Debug.Log("Great 1");
                //Debug.Log(newGreat);
                HitGreat();
            }
            else if (pointFrom <= newGood && pointFrom >= newGreat)
            {
                //Debug.Log("Good 1");
                //Debug.Log(newGood);
                HitGood();
            }
            else if (pointFrom >= newGood && pointFrom <= .5f)
            {
                //Debug.Log(newGood);
                Missed(false);
            }
        }
        // If the player is closer to the next note.
        // track the distance between the previous note and the current note
        else if (pointTo < pointFrom && tc.trackPosInBeatsGame > (tc.selectedMap.noteTimeTaken + tc.firstInterval))
        {
            if (pointTo <= newPerfect && pointTo >= 0)
            {
                //Debug.Log("Perfect 2");
                //Debug.Log(newPerfect);
                HitPerfect();
            }
            else if (pointTo <= newGreat && pointTo >= newPerfect)
            {
                //Debug.Log("Great 2");
                //Debug.Log(newGreat);
                HitGreat();
            }
            else if (pointTo <= newGood && pointTo >= newGreat)
            {
                //Debug.Log("Good 2");
                //Debug.Log(newGood);
                HitGood();
            }
            else if (pointTo >= newGood && pointTo <= .5f)
            {
                Missed(false);
            }
        }
    }
    private void CheckFirstHitAccuracy()
    {
        if (tc.trackPosInBeatsGame > (tc.selectedMap.noteTimeTaken + tc.firstInterval))
        {
            return;
        }

        if (pointTo <= gm.perfectMin && pointTo >= 0)
        {
            //Debug.Log("Perfect 3");
            HitPerfect();
        }
        else if (pointTo <= gm.greatMin && pointTo >= gm.perfectMin)
        {
            //Debug.Log("Great 3");
            HitGreat();
        }
        else if (pointTo <= gm.goodMin && pointTo >= gm.greatMin)
        {
            //Debug.Log("Good 3");
            HitGood();
        }
        else if (pointTo >= gm.goodMin && pointTo <= .5f)
        {
            Missed(false);
        }
    }

    private void HitPerfect()
    {
        // Check if the player needs to move for hitting a launch note
        MoveForLaunch();

        gm.UpdateHealth(gm.regenPerfect);
        gm.score += (gm.perfectScore * gm.comboMulti);
        gm.perfects++;
        gm.curAccuracy += 3;

        //Update player accuracy UI
        accuracyUI.text = gm.perfectScoreName;

        // Began the cooldown till the acuracy ui text vanishes
        StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        validMovement = false;
        //Debug.Break();
    }
    private void HitGreat()
    {
        // Check if the player needs to move for hitting a launch note
        MoveForLaunch();

        gm.UpdateHealth(gm.regenGreat);
        gm.score += (gm.goodScore * gm.comboMulti);
        gm.greats++;
        gm.curAccuracy += 2;

        //Update player accuracy UI
        accuracyUI.text = gm.greatScoreName;

        // Began the cooldown till the acuracy ui text vanishes
        StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        validMovement = false;
        //Debug.Break();
    }
    private void HitGood()
    {
        // Check if the player needs to move for hitting a launch note
        MoveForLaunch();

        gm.UpdateHealth(gm.regenGood);
        gm.score += (gm.badScore * gm.comboMulti);
        gm.goods++;
        gm.curAccuracy += 1;

        //Update player accuracy UI
        accuracyUI.text = gm.goodScoreName;

        // Began the cooldown till the acuracy ui text vanishes
        StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        validMovement = false;
        //Debug.Break();
    }
    public void Missed(bool hitByBomb)
    {
        if (!hitByBomb)
        {
            nearestNoteScript.canGetNote = false;
            nearestNoteScript.missed = true;
            gm.UpdateHealth(gm.lossMiss);
            gm.scoreIncreased = true;

            // Increase the max accuracy if the note got missed
            if (tc.notes[0].gameObject.GetComponent<Note>().missed && !tc.notes[0].gameObject.GetComponent<Note>().noteCalculatedAcc)
            {
                tc.notes[0].gameObject.GetComponent<Note>().noteCalculatedAcc = true;
                gm.totalAccuracyMax += 3;
            }

            gm.misses++;

            gm.score += gm.missScore;

            //Update player accuracy UI
            accuracyUI.text = gm.missScoreName;

            // Update total accuracy UI
            gm.UpdateTotalAccuracy();

            // Began the cooldown till the acuracy ui text vanishes
            StartCoroutine("DiminishAccuracyUI");

        }
        else
        {
            gm.UpdateHealth(gm.bombHit);
        }

        gm.comboMulti = 1;
        gm.UpdateUI();
        //Debug.Break();
    }

    void MakeNoteInvisible()
    {
        // Disable all aesthetics of a note
        nearestAnyNoteScript.hitMarkerCanvas.SetActive(false);
        nearestAnyNoteScript.noteObject.SetActive(false);
        nearestAnyNoteScript.spotLight.SetActive(false);
    }

    void MoveForLaunch()
    {
        if (hitLLaunch)
        {
            hitLLaunch = false;
            playerPos.x = 1.5f * (nearestNoteScript.laneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }

        else if (hitRLaunch)
        {
            hitRLaunch = false;
            playerPos.x = 1.5f * (nearestNoteScript.laneNumber);
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }
    }

    IEnumerator DiminishAccuracyUI()
    {
        yield return new WaitForSeconds(.75f);
        accuracyUI.text = "";   
    }
}