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
    private float pointTo;

    public bool canIncreaseScore;
    public bool scoreAllowed;
    public float elapsedTimeSinceMove;

    private float startTime;
    public bool isBlocking;

    private float currentPointInBeats;

    public List<Transform> activeNotes = new List<Transform>();
    public Transform nearestNote;

    public GameObject[] lanePos = new GameObject[5];
    private int playerCurrentLane;

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

        lanePos = GameObject.FindGameObjectsWithTag("PlayerPos");
        playerCurrentLane = 3;
        gameObject.GetComponent<Transform>().position = lanePos[playerCurrentLane].GetComponent<Transform>().position;
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
        gameObject.GetComponent<Transform>().position = lanePos[playerCurrentLane].gameObject.GetComponent<Transform>().position;
        gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
        // Movement();
        //FindNearestNote();
    }
    /*
    void FindNearestNote()
    {
        float minDist = Mathf.Infinity;

        // Detect the nearest enemy's position
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
    */
    void Inputs()
    {
        /*
        // If:
        //      player is pressing D
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        if (Input.GetKeyDown("d") && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber)
        {
            movingRight = true;
            /*
            // Ensure that the player cannot get score until 1 beat before the first note
            if (tc.trackPosInBeatsGame > tc.firstNote - 1)
            {
                AssignFromAndToValues();
                scoreAllowed = false;
                canIncreaseScore = true;
            }

        }

        // If:
        //      player is pressing A
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        if (Input.GetKeyDown("a") && !movingRight && !movingLeft && nearestLaneNumber != 1)
        {
            movingLeft = true;
            /*
            // Ensure that the player cannot get score until 1 beat before the first note
            if (tc.trackPosInBeatsGame > tc.firstNote - 1)
            {
                AssignFromAndToValues();
                scoreAllowed = false;
                canIncreaseScore = true;
            }
            }
            */
        if (Input.GetKeyDown("d"))
        {
            MoveRight(false);
        }
        else if (Input.GetKeyDown("a"))
        {
            MoveLeft(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isBlocking = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isBlocking = false;
        }
    }
  
    void MoveLeft(bool launch)
    {
        if (launch)
        {
            playerCurrentLane = 0;
        }
        else if (playerCurrentLane!=0)
        {
            playerCurrentLane -= 1;
        }
        gameObject.GetComponent<Transform>().position = lanePos[playerCurrentLane].gameObject.GetComponent<Transform>().position;
    }
    void MoveRight(bool launch)
    {
        if (launch)
        {
            playerCurrentLane = 4;
        }
       else if (playerCurrentLane != 4)
        {
            playerCurrentLane += 1;
        }
        gameObject.GetComponent<Transform>().position = lanePos[playerCurrentLane].gameObject.GetComponent<Transform>().position;
    }

    /*
    void Movement()
    {
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
    */
    void AssignFromAndToValues()
    {
        if (!isBlocking)
        {
            return;
        }

        currentPointInBeats = tc.trackPosInBeatsGame;

        pointFrom = 1 - (Mathf.InverseLerp(tc.nextNoteInBeats3, tc.previousNoteBeatTime3, currentPointInBeats));

        pointTo = 1 - pointFrom;

        gm.scoreIncreased = true;

        //Debug.Log("pointFrom " + pointFrom);
        //Debug.Log("pointTo " + pointTo);
        //Debug.Log("``````````````````````");
        //Debug.Break();

        //CheckHitAccuracy();
    }

    private void DoNoteEffect()
    {
        Note nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();

        // In the case of a note with a left arrow
        if (nearestNoteScript.noteDir == "left" && nearestLaneNumber > nearestNoteScript.laneNumber)
        {

            if (nearestNoteScript.noteType == "launch")
            {
                //StartCoroutine(cm.RollCamera("left"));
                playerHitLaunch = true;
            }
        }

        // In the case of a note with a right arrow
        else if (nearestNoteScript.noteDir == "right" && nearestLaneNumber < nearestNoteScript.laneNumber)
        {

            if (nearestNoteScript.noteType == "launch")
            {
                //StartCoroutine(cm.RollCamera("right"));
                playerHitLaunch = true;
            }
        }
    }

    public void DoNoteEffectUp()
    {
        if (!nearestNote)
        {
            return;
        }
        Note nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();

        if (nearestNoteScript.noteDir == "up" && isBlocking)
        {
            if (nearestNoteScript.gameObject.transform.position.z < transform.position.z
                && nearestLaneNumber == nearestNoteScript.laneNumber && !nearestNoteScript.doneUpArrow)
            {
                nearestNoteScript.doneUpArrow = true;
                AssignFromAndToValues();
                //Debug.Break();
            }
        }

    }

    /*
    private void CheckHitAccuracy()
    {
        // If the player has already inputed a legal move for the note, do not allow it
        if (!tc.canGetNote)
        {
            //ResetNotes();
            return;
        }

        tc.canGetNote = false;

        // Also the player doesn't recieve any misses for not performing a movement input at all.

        //Debug.Break();

        CheckFirstHitAccuracy();

        if (pointTo < 0 || pointFrom > 1 && (tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval)))
        {
            //Debug.Log("Perfect - glitch");
            HitPerfect();
            ResetNotes();
            return;
        }

        // If the player is closer to the previous note.
        if (pointTo > pointFrom && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
        {
            if (pointFrom <= gm.perfectMin && pointFrom >= 0)
            {
                //Debug.Log("Perfect 1");
                HitPerfect();
            }
            else if (pointFrom <= gm.greatMin && pointFrom >= gm.perfectMin)
            {
                //Debug.Log("Great 1");
                HitGreat();
            }
            else if (pointFrom <= gm.goodMin && pointFrom >= gm.greatMin)
            {
                //Debug.Log("Good 1");
                HitGood();
            }
            else if (pointFrom >= gm.goodMin && pointFrom <= .5f)
            {
                //Debug.Log("Miss 1");
                Missed();
            }
        }
        // If the player is closer to the next note.
        else if (pointTo < pointFrom && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
        {
            if (pointTo <= gm.perfectMin && pointTo >= 0)
            {
                //Debug.Log("Perfect 2");
                HitPerfect();
            }
            else if (pointTo <= gm.greatMin && pointTo >= gm.perfectMin)
            {
                //Debug.Log("Great 2");
                HitGreat();
            }
            else if (pointTo <= gm.goodMin && pointTo >= gm.greatMin)
            {
                //Debug.Log("Good 2");
                HitGood();
            }
            else if (pointTo >= gm.goodMin && pointTo <= .5f)
            {
                //Debug.Log("Miss 2");
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
    */
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("coll enter" + collision.gameObject.name);
    }
    private void ResetNotes()
    {
        canIncreaseScore = false;
        noteCalculationOver = false;
        scoreAllowed = false;
    }

    private void HitPerfect()
    {
        gm.score += gm.perfectScore;
        gm.perfects++;
        DoNoteEffect();
    }
    private void HitGreat()
    {
        gm.score += gm.goodScore;
        gm.greats++;
        DoNoteEffect();
    }
    private void HitGood()
    {
        gm.score += gm.badScore;
        gm.goods++;
        DoNoteEffect();
    }
    public void Missed()
    {
        //Debug.Log("Missed");
        gm.score += gm.missScore;
        gm.misses++;
        // Doing this because this variable needs to be activaed whenever any score is gained
        // even a miss. 
        gm.scoreIncreased = true;
        // Update score UI because getting a 'Miss' will not trigger score UI change
        gm.UpdateUI();
    }
}