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
    [HideInInspector]
    public int nearestLaneNumber;

    public bool noteCalculationOver;

    bool playerHitLaunch;

    private Renderer rend;
    public float playerWidth;

    public bool hitNote;
    bool doneOnce;
    bool doneOnce3;
    public bool doneOnce2;

    [HideInInspector]
    public float pointFrom;
    //[HideInInspector]
    public float missPointFrom;
    private float pointTo;

    public bool canIncreaseScore;
    public bool scoreAllowed;
    public float elapsedTimeSinceMove;

    private float startTime;
    public bool isShielding;

    private float currentPointInBeats;
    private float missCurrentPointInBeats;

    public List<Transform> activeNotes = new List<Transform>();
    public List<Transform> notesBehind = new List<Transform>();
    public List<float> distances = new List<float>();

    public Transform nearestNote;
    public GameObject furthestBehindNote;

    public Material shieldMat;
    private Color shieldColor;
    public GameObject shield;

    public float newPerfect;
    public float newGreat;
    public float newGood;

    public Note nearestNoteScript;

    public Text accuracyUI;

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

    public void RepositionPlayer(GameObject go)
    {
        Vector3 p = transform.position;
        p.x = go.GetComponent<Path>().laneNumber * pm.initialPath.GetComponent<Path>().pathWidth;
        transform.position = p;

        //Find the path the player is on
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            //Debug.DrawRay(player.transform.position, Vector3.down, Color.green);
            pm.nearestPath = hit.collider.gameObject;
        }
        else
        {
            //Debug.DrawRay(player.transform.position, Vector3.down, Color.red);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        Movement();
        FindNearestNote();
        UpdateShield();
        AssignMisses();

        if (!nearestNote)
        {
            return;
        }

        nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();

        missCurrentPointInBeats = tc.trackPosInBeatsGame;
    }

    void AssignMisses()
    {
        // If there is a note alive 
        if (nearestNote)
        {

            // This must be here for some reason
            nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();

            // Calculate whether the player is far enough from the nearest note to determine that it's too late to get 'good' or higher score from it
            CalculateMissPointFrom();

            // If there is a note behind the player and it has not been hit yet AND
            // the note hasn't been missed yet AND
            // the player is unable to hit the note anymore give a miss to the player

            if (missPointFrom > newGood + 0.05f && nearestNote.transform.position.z < transform.position.z && nearestNoteScript.canGetNote)
            {
                Debug.Log("missed");    
                Missed();
            }
        }
    }

    public void CalculateMissPointFrom()
    {
        if (!tc.trackInProgress)
        {
            return;
        }

        if (tc.nextIndex3 <= tc.allNotes.Count)
        {
            if (nearestNoteScript.noteNumber == tc.allNotes.Count && nearestNoteScript.behindPlayer)
            {
                tc.nextNoteInBeats = tc.previousNoteBeatTime3 + 2;
            }

            if (nearestNoteScript.noteNumber == tc.allNotes.Count)
            {
                missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
            }

            else
            {
                tc.nextNoteInBeats = tc.previousNoteBeatTime3 + (tc.noteEighthCount[tc.nextIndex3]);
                missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
            }
        }
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
                nearestNote = t;
                minDist = dist;
            }
        }
    }

    public void DestroyFurthestNote()
    {
        if (!tc.trackInProgress)
        {
            return;
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

        // If there are 2 or more notes behind the enemy, destroy the most furthest one
        for (int i = 0; i < activeNotes.Count; i++)
        {
            if (activeNotes[i].gameObject.transform.position.z < transform.position.z)
            {
                // Only add the note if it NOT already in the 'notesBehind' list
                if (!notesBehind.Contains(activeNotes[i].gameObject.transform))
                {
                    notesBehind.Add(activeNotes[i].gameObject.transform);
                }
            }

            if (notesBehind.Count >= 2)
            {
                StartCoroutine(furthestBehindNote.GetComponent<Note>().DestroyNote());
                //Debug.Break();
            }
        }
    }

    void Inputs()
    {
        // This is a check for when both moving right and left movement are held
        if (Input.GetKey(KeyCode.RightShift))
        {
            holdingMovingRightInp = true;
        }

        else
        {
            holdingMovingRightInp = false;
        }

        // This is a check for when both moving right and left movement are held
        if (Input.GetKey(KeyCode.LeftShift))
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
        if (Input.GetKeyUp(KeyCode.RightShift) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber && !blastInput && isShielding)
        {
            movingRight = true;
            AssignFromAndToValuesNoteAndLaunch();
            scoreAllowed = false;
            canIncreaseScore = true;
        }

        // If:
        //      player is pressing for moving left
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        //      blastInput is false
        //      player is NOT shielding
        if (Input.GetKey(KeyCode.LeftShift) && !movingLeft && !movingRight && nearestLaneNumber != 1 && !blastInput && !isShielding)
        {
            movingLeft = true;
        }

        // If:
        //      player is pressing for moving left
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        //      blastInput is false
        //      player is shielding
        if (Input.GetKeyUp(KeyCode.LeftShift) && !movingRight && !movingLeft && nearestLaneNumber != 1 && !blastInput && isShielding)
        {
            movingLeft = true;
            AssignFromAndToValuesNoteAndLaunch();
            scoreAllowed = false;
            canIncreaseScore = true;
        }

        // If:
        //      player is pressing for moving right
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        //      blastInput is false
        //      player is NOT shielding
        if (Input.GetKey(KeyCode.RightShift) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber && !blastInput && !isShielding)
        {
            movingRight = true;
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

    void Movement()
    {
        // Stops strange drifting behaviour
        Vector3 playerPos2 = transform.position;
        playerPos2.z = 1;
        transform.position = playerPos2;

        // Ensures that there is a nearest path to begin with
        // There must be one for this code to work. 
        if (!pm.nearestPath)
        {
            return;
        }

        Vector3 playerPos = transform.position;

        // Adding movement in the forward direction of the player
        rb.velocity = Vector3.zero;

        // If not moving, Reference the nearest lane number.
        if (!movingLeft && !movingRight)
        {
            nearestLaneNumber = pm.nearestPath.GetComponent<Path>().laneNumber;
        }

        // After moving Left. Stop the player from moving into the next lane
        if (movingLeft && playerPos.x <= pathWidth * (nearestLaneNumber - 2) && !playerHitLaunch)
        {
            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            transform.position = playerPos;
            movingLeft = false;
        }

        // After moving Right. Stop the player from moving into the next lane
        if (movingRight && playerPos.x >= pathWidth * nearestLaneNumber && !playerHitLaunch)
        {
            playerPos.x = pathWidth * nearestLaneNumber;
            transform.position = playerPos;
            movingRight = false;
        }

        // If the player moved left to contact a launch note. Do not stop the player at the next lane. Let them reach the end lane
        if (movingRight && playerPos.x >= ((pathWidth * pm.laneNumbers[0]) - pathWidth) && playerHitLaunch)
        {
            playerPos.x = 6;
            transform.position = playerPos;
            movingRight = false;
            playerHitLaunch = false;
        }

        // If the player moved right to contact a launch note. Do not stop the player at the next lane. Let them reach the end lane
        if (movingLeft && playerPos.x <= ((pathWidth * pm.maxLanes) - pathWidth) && playerHitLaunch)
        {
            playerPos.x = 0;
            transform.position = playerPos;
            movingLeft = false;
            playerHitLaunch = false;
        }

        // Functionality of moving right
        if (movingRight)
        {
            movingLeft = false;
            rb.AddForce(Vector3.right * gm.playerEvadeStr);
        }


        // Functionality of moving left
        if (movingLeft)
        {
            movingRight = false;
            rb.AddForce(Vector3.left * gm.playerEvadeStr);
        }
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

        pointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, currentPointInBeats)));
        pointTo = 1 - pointFrom;
        gm.scoreIncreased = true;
        //Debug.Log("``````````````````````");
        //Debug.Log("nextNoteInBeats3 = " + tc.nextNoteInBeats3);
        //Debug.Log("previousNoteBeatTime3 = " + tc.previousNoteBeatTime3);
        //Debug.Log("currentPointInBeats = " + currentPointInBeats);
        //Debug.Log("pointFrom " + pointFrom);
        //Debug.Log("pointTo " + pointTo);
        CheckForNoteAndLaunchEffect();
    }
    private void CheckForBlastEffect()
    {
        // if the nearest not has already been hit once, miss the player for attempt
        if (nearestNote)
        {
            if (nearestNoteScript.hitAmount > 1 && !nearestNoteScript.missed)
            {
                Missed();
            }

            nearestNoteScript.hitAmount++;
        }


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
            Missed();
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
        //Debug.Log("pointMin " + gm.goodMin);

        //Debug.Log(nearestNote.GetComponent<Note>().canGetNote);

        // if the nearest not has already been hit once, miss the player for attempt
        if (!nearestNote)
        {
            return;
        }
        if (nearestNoteScript.hitAmount > 1 && !nearestNoteScript.missed)
        {
            //Debug.Break();
            Missed();
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
            Missed();
        }

        // If an attempt on the up note was made, instantly miss the note
        if (nearestNoteScript.noteDir == "up" && nearestNoteScript.noteType != "blast")
        {
            Missed();
        }

        // In the case of a note with a left arrow
        if (nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "launch" && nearestNoteScript.noteType != "blast")
        {
            if (nearestLaneNumber == nearestNoteScript.laneNumber + 1 && movingLeft)
            {
                CheckHitAccuracy();
                return;
            }
            else
            {
                Missed();
                return;
            }
        }
        // In the case of a note with a right arrow
        else if (nearestNoteScript.noteDir == "right" && nearestNoteScript.noteType != "launch" && nearestNoteScript.noteType != "blast")
        {
            if (nearestLaneNumber == nearestNoteScript.laneNumber - 1 && movingRight)
            {
                CheckHitAccuracy();
                return;
            }
            else
            {
                Missed();
                return;
            }
        }

        // In the case of a launch with a right arrow
        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "right" && nearestNoteScript.noteType != "blast")
        {
            if (nearestLaneNumber == nearestNoteScript.laneNumber - 1 && movingRight)
            {
                CheckHitAccuracy();
                playerHitLaunch = true;
                return;
            }
            else
            {
                Missed();
                return;
            }
        }

        // in the case of a launch with a left arrow
        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "blast")
        {
            if (nearestLaneNumber == nearestNoteScript.laneNumber + 1 && movingLeft)
            {
                CheckHitAccuracy();
                playerHitLaunch = true;
                return;
            }
            else
            {
                Missed();
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
        if (nearestNoteScript.noteDir == "up" && isShielding && nearestNoteScript.hitAmount == 0)
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
             return;
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
        newPerfect = gm.perfectMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);
        newGreat = gm.greatMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);
        newGood = gm.goodMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);

        // Not sure if this is used anymore
        CheckFirstHitAccuracy();

        // This is score for a glitch? not sure if this happens anymore
        if (pointTo < 0 || pointFrom > 1 && (tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval)))
        {
            Debug.Log("Perfect - glitch");
            HitPerfect();
            ResetNotes();
            return;
        }
        // If the player is closer to the previous note. 
        // track the distance between the next note and the current note
        if (pointTo > pointFrom && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
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
                Missed();
            }
        }
        // If the player is closer to the next note.
        // track the distance between the previous note and the current note
        else if (pointTo < pointFrom && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
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
                //Debug.Log(newGood);
                Missed();
            }
        }
        //Debug.Log("==================================================");
        ResetNotes();
        //Debug.Break();
    }
    private void CheckFirstHitAccuracy()
    {
        if (tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
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
            //Debug.Log("Miss 3");
            Missed();
        }

        ResetNotes();
    }
    private void ResetNotes()
    {
        canIncreaseScore = false;
        noteCalculationOver = false;
        scoreAllowed = false;
    }

    private void HitPerfect()
    {
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
    }
    private void HitGreat()
    {
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
    }
    private void HitGood()
    {
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
    }
    public void Missed()
    {
        nearestNoteScript.canGetNote = false;
        nearestNoteScript.missed = true;
        gm.UpdateHealth(gm.lossMiss);
        gm.comboMulti = 1;
        gm.score += gm.missScore;
        gm.misses++;
        // Doing this because this variable needs to be activaed whenever any score is gained
        // even a miss. 
        gm.scoreIncreased = true;
        // Update score UI because getting a 'Miss' will not trigger score UI change
        gm.UpdateUI();

        // Do not allow gamemode to do the calculation for total accuracy
        nearestNoteScript.noteCalculatedAcc = true;
        gm.UpdateTotalAccuracy();

        //Update player accuracy UI
        accuracyUI.text = gm.missScoreName;

        // Began the cooldown till the acuracy ui text vanishes
        StartCoroutine("DiminishAccuracyUI");
    }

    void MakeNoteInvisible()
    {
        // Disable all aesthetics of a note
        nearestNoteScript.hitMarkerCanvas.SetActive(false);
        nearestNoteScript.noteObject.SetActive(false);
        nearestNoteScript.spotLight.SetActive(false);
    }

    IEnumerator DiminishAccuracyUI()
    {
        yield return new WaitForSeconds(.75f);
        accuracyUI.text = "";   
    }
}