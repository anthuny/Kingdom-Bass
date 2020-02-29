using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    private Gamemode gm;
    private PathManager pm;
    private TrackCreator tc;
    private CameraBehaviour cm;

    private float pathWidth;

    private bool movingRight;
    private bool movingLeft;
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

    private float pointFrom;
    private float missPointFrom;
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

    float newPerfect;
    float newGreat;
    float newGood;



    private Note nearestNoteScript;

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
    }

    void AssignMisses()
    {
        // If there is a note alive 
        if (nearestNote)
        {
            // If there is a note behind the player and it has not been hit yet AND
            // the note hasn't been missed yet AND
            // the player is unable to hit the note anymore give a miss to the player
            
            missCurrentPointInBeats = tc.trackPosInBeatsGame;
            missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats3, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
            if (nearestNote.GetComponent<Note>().canGetNote && missPointFrom > gm.goodMin + 0.05f && nearestNote.transform.position.z < transform.position.z && !nearestNote.GetComponent<Note>().missed)
            {
                nearestNote.GetComponent<Note>().missed = true;
                Missed();
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
    public void Hey()
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
        // If:
        //      player is pressing D
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        if (Input.GetKeyDown(KeyCode.RightShift) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber)
        {
            movingRight = true;
            // Ensure that the player cannot get score until 1 beat before the first note
            //Debug.Break();
            /*
            if (tc.trackPosInBeatsGame > tc.firstNote - 1)
            {
                //Debug.Break();
                AssignFromAndToValues();
                scoreAllowed = false;
                canIncreaseScore = true;
            }
            */
            //Debug.Log("1");
            AssignFromAndToValues();
            scoreAllowed = false;
            canIncreaseScore = true;
        }

        // If:
        //      player is pressing A
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        if (Input.GetKeyDown(KeyCode.LeftShift) && !movingRight && !movingLeft && nearestLaneNumber != 1)
        {
            movingLeft = true;
            // Ensure that the player cannot get score until 1 beat before the first note
            /*
            if (tc.trackPosInBeatsGame > tc.firstNote - 1)
            {
                AssignFromAndToValues();
                scoreAllowed = false;
                canIncreaseScore = true;
            }
            */
            AssignFromAndToValues();
            scoreAllowed = false;
            canIncreaseScore = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isShielding = false;
        }

        else if (Input.GetKeyUp(KeyCode.Space))
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
        // There must be on for this code to work. 
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
    void AssignFromAndToValues()
    {
        //Debug.Log("2");
        if (!isShielding)
        {
            return;
        }
        currentPointInBeats = tc.trackPosInBeatsGame;

        pointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats3, tc.previousNoteBeatTime3, currentPointInBeats)));
        //pointFrom *= tc.nextNoteInBeats3 - tc.previousNoteBeatTime3;
        pointTo = 1 - pointFrom;
        gm.scoreIncreased = true;
        Debug.Log("``````````````````````");
        Debug.Log("nextNoteInBeats3 = " + tc.nextNoteInBeats3);
        Debug.Log("previousNoteBeatTime3 = " + tc.previousNoteBeatTime3);
        Debug.Log("currentPointInBeats = " + currentPointInBeats);
        Debug.Log("pointFrom " + pointFrom);
        Debug.Log("pointTo " + pointTo);

        CheckForNoteEffect();
    }

    private void CheckForNoteEffect()
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
        if (nearestNote.GetComponent<Note>().hitAmount == 1)
        {
            Missed();
        }

        nearestNote.GetComponent<Note>().hitAmount++;

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNote.GetComponent<Note>().canGetNote)
        {
            return;
        }

        nearestNote.GetComponent<Note>().canGetNote = false;


        //Debug.Log(nearestNote.GetComponent<Note>().canGetNote);
        //Debug.Break();

        // In the case of a note with a left arrow
        if (nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "launch" && nearestLaneNumber == nearestNoteScript.laneNumber + 1 && movingLeft)
        {
            CheckHitAccuracy();
            return;
        }
        // In the case of a note with a right arrow
        else if (nearestNoteScript.noteDir == "right" && nearestNoteScript.noteType != "launch" && nearestLaneNumber == nearestNoteScript.laneNumber - 1 && movingRight)
        {
            CheckHitAccuracy();
            return;
        }

        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "right" && nearestLaneNumber == nearestNoteScript.laneNumber - 1 && movingRight)
        {
            //Debug.Log("a");
            if (nearestNoteScript.behindPlayer)
            {
                //Debug.Log("b");
                if (pointFrom < gm.goodMin)
                {
                    CheckHitAccuracy();
                    playerHitLaunch = true;
                    return;
                }
            }
            else
            {
                if (pointTo < gm.goodMin)
                {
                    CheckHitAccuracy();
                    playerHitLaunch = true;
                    return;
                }
            }
        }

        else if (nearestNoteScript.noteType == "launch" && nearestNoteScript.noteDir == "left" && nearestLaneNumber == nearestNoteScript.laneNumber + 1 && movingLeft)
        {
            if (nearestNoteScript.behindPlayer)
            {
                if (pointFrom < gm.goodMin)
                {
                    CheckHitAccuracy();
                    playerHitLaunch = true;
                    return;
                }
            }
            else
            {
                if (pointTo < gm.goodMin)
                {
                    CheckHitAccuracy();
                    playerHitLaunch = true;
                    return;
                }
            }
        }
    }

    public void DoNoteEffectUp()
    {
        if (nearestNoteScript.noteDir == "up" && isShielding)
        {
            if (nearestNoteScript.gameObject.transform.position.z < transform.position.z 
                && nearestLaneNumber == nearestNoteScript.laneNumber && !nearestNoteScript.doneUpArrow)
            {
                nearestNoteScript.doneUpArrow = true;
                //AssignFromAndToValues();
                CheckHitAccuracy();
                // Increase the hit amounts of the up note by 1
                nearestNote.GetComponent<Note>().hitAmount++;

                // Make it so the up arrow can not give score again once done once aready
                nearestNote.GetComponent<Note>().canGetNote = false;
                //Debug.Break();
            }
        }

    }

    private void CheckHitAccuracy()
    {
        newPerfect = gm.perfectMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);
        Debug.Log("newPerfect " + newPerfect);
        newGreat = gm.greatMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);
        Debug.Log("newGreat " + newGreat);
        newGood = gm.goodMin / (nearestNoteScript.eighthWait / gm.defaultBeatsBetNotes);
        Debug.Log("newGood " + newGood);

        //Debug.Log("eighthWait " + nearestNoteScript.eighthWait);
        // Also the player doesn't recieve any misses for not performing a movement input at all.

        //Debug.Break();

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
                //Debug.Log("Miss 1");
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
                //Debug.Log("Miss 2");
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
        //CheckForNoteEffect();
    }
    private void HitGreat()
    {
        gm.UpdateHealth(gm.regenGreat);
        gm.score += (gm.goodScore * gm.comboMulti);
        gm.greats++;
        //CheckForNoteEffect();
    }
    private void HitGood()
    {
        gm.UpdateHealth(gm.regenGood);
        gm.score += (gm.badScore * gm.comboMulti);
        gm.goods++;
        //CheckForNoteEffect();
    }
    public void Missed()
    {
        gm.UpdateHealth(gm.lossMiss);
        //Debug.Log("Missed");
        gm.comboMulti = 1;
        gm.score += gm.missScore;
        gm.misses++;
        // Doing this because this variable needs to be activaed whenever any score is gained
        // even a miss. 
        gm.scoreIncreased = true;
        // Update score UI because getting a 'Miss' will not trigger score UI change
        gm.UpdateUI();
    }
}