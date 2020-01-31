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

    private float pathWidth;

    private bool movingRight;
    private bool movingLeft;
    [HideInInspector]
    public int nearestLaneNumber;

    public bool passedBeat;

    bool playerHitLaunch;

    private Renderer rend;
    public float playerWidth;

    bool hitNote;
    bool doneOnce;
    bool doneOnce3;
    public bool doneOnce2;

    float minDist;

    private float pointFrom;
    private float pointTo;

    private bool stopTimer;
    [HideInInspector]
    public float timeFromLastMovement;

    private int inputAmounts;

    public bool canIncreaseScore;
    public bool scoreAllowed;
    public float elapsedTimeSinceMove;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();

        rend = GetComponentInChildren<Renderer>();
        playerWidth = rend.bounds.size.z;   

        pathWidth = pm.initialPath.GetComponent<Path>().pathWidth;

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
        CheckHitAccuracy();
    }

    void Inputs()
    {
        // If:
        //      player is pressing D
        //      playing is NOT moving left or right already
        //      player is not in the most RIGHT lane
        if (Input.GetKeyDown("d") && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber)
        {
            AssignFromAndToValues();
            scoreAllowed = false;
            timeFromLastMovement = tc.trackPos;
            startTime = tc.trackPos;
            canIncreaseScore = true;
            movingRight = true;
            if (!passedBeat)
            {
                passedBeat = true;

                doneOnce = false;
            }
        }

        // If:
        //      player is pressing A
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        if (Input.GetKeyDown("a") && !movingRight && !movingLeft && nearestLaneNumber != 1)
        {
            AssignFromAndToValues();
            scoreAllowed = false;
            startTime = tc.trackPos;
            canIncreaseScore = true;
            movingLeft = true;
            if (!passedBeat)
            {
                passedBeat = true;

                doneOnce = false;
            }
        }
    }

    void AssignFromAndToValues()
    {
        //pointFrom = tc.pointFromLastBeat / tc.nextIndex2;
        //pointTo = tc.pointToNextBeat / tc.nextIndex2;

        tc.pointFromLastBeatInstant = tc.pointFromLastBeat;
        //Debug.Log("trackPosInbeatsGameInstant" + tc.trackPosInBeatsGame);
        //Debug.Break();
    }
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

    private void CheckHitAccuracy()
    {
        // If the player has already inputed a legal move for the note, do not allow it
        if (!tc.canGetNote)
        {
            ResetNotes();
            return;
        }

        // If enough time from the last movement has passed, count the move, toward the score count
        if (!scoreAllowed && passedBeat)
        {
            elapsedTimeSinceMove = (tc.trackPos - startTime);
            //Debug.Log(elapsedTimeSinceMove);
            canIncreaseScore = true;
        }

        if (elapsedTimeSinceMove >= gm.maxTimeBetweenInputs && !scoreAllowed)
        {
            scoreAllowed = true;
            //Debug.Log("setting start time");
            elapsedTimeSinceMove = 0;
            //inputAmounts = 1;


            //Debug.Log("Score given");
            //stopTimer = false;
            return;
        }



        // Check if the movement is allowed to increase the score. If not, do not allow it. Otherwise, allow it
        if (!scoreAllowed || !canIncreaseScore)
        {
            return;
        }

        // This should be okay to leave here. The player must go through all of the above to make the note count
        tc.canGetNote = false;
        //Debug.Log("score allowed " + scoreAllowed);
        //Debug.Log("canIncreaseScore " + canIncreaseScore);

        //Debug.Break();
        //Debug.Log("moving");


        // Line reset
        //Debug.Log("================================");

        //Debug.Log("pointFrom raw " + tc.pointFromLastBeat);
        //Debug.Log("pointTo Raw " + tc.pointToNextBeat);
        //Debug.Log("nextIndex2 " + tc.nextIndex2);
        //Debug.Log("point from index val " + tc.trackPosIntervalsList[0]);

        //Debug.Log("trackPosInBeatsGame " + tc.trackPosInBeatsGame);
        //Debug.Log("trackPosIntervals " + tc.trackPosIntervals);
        //Debug.Log("next beat " + tc.nextBeat);
        //Debug.Log("trackPosInBeatsGame + " + tc.trackPosInBeatsGame);
        tc.pointFromLastBeatWait = tc.pointFromLastBeat;
        pointFrom = tc.pointFromLastBeatInstant - Mathf.FloorToInt(tc.pointFromLastBeatInstant);
        pointTo = 1 - pointFrom;
        //Debug.Log("trackPosInBeatsGame instant 2 " + tc.pointFromLastBeatInstant);
        //Debug.Log("trackPosInBeatsGame wait " + tc.pointFromLastBeatWait);
        //Debug.Log("trackPosInBeatsGame after wait " + tc.trackPosInBeatsGame);
        //Debug.Log("difference" + (tc.pointFromLastBeatWait - tc.pointFromLastBeatInstant));
        Debug.Log("pointFrom " + pointFrom);
        Debug.Log("PointTo " + pointTo);

        // Future Anthony - Currently working on not allowing score increase if movements are too fast
        // This works for the most part. Except sometimes the player doesn't get score for notes even
        // when enough time has passed

        // Also the player doesn't recieve any misses for not performing a movement input at all.


        CheckFirstHitAccuracy();

        // need to divide point to and pointfrom by 2 if it's double the eighth lengths. 
        // need to do this in a formula that works for every eighth length.
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
                Debug.Log("Perfect 1");
                HitPerfect();
            }
            else if (pointFrom <= gm.greatMin && pointFrom >= gm.perfectMin)
            {
                Debug.Log("Great 1");
                HitGood();
            }
            else if (pointFrom <= gm.goodMin && pointFrom >= gm.greatMin)
            {
                Debug.Log("Good 1");
                HitBad();
            }
            else if (pointFrom >= gm.goodMin && pointFrom <= .5f)
            {
                Debug.Log("Miss 1");
                Missed();
            }
        }
        // If the player is closer to the next note.
        else if (pointTo < pointFrom && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
        {
            if (pointTo <= gm.perfectMin && pointTo >= 0)
            {
                Debug.Log("Perfect 2");
                HitPerfect();
            }
            else if (pointTo <= gm.greatMin && pointTo >= gm.perfectMin)
            {
                Debug.Log("Great 2");
                HitGood();
            }
            else if (pointTo <= gm.goodMin && pointTo >= gm.greatMin)
            {
                Debug.Log("Good 2");
                HitBad();
            }
            else if (pointTo >= gm.goodMin && pointTo <= .5f)
            {
                Debug.Log("Miss 2");
                Missed();
            }
        }
        Debug.Break();
        Debug.Log("==================================================");
        ResetNotes();
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
            HitGood();
        }
        else if (pointTo <= gm.goodMin && pointTo >= gm.greatMin)
        {
            //Debug.Log("Good 3");
            HitBad();
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
        passedBeat = false;
    }

    private void HitPerfect()
    {
        gm.score += gm.perfectScore;
        gm.perfects++;
    }
    private void HitGood()
    {
        gm.score += gm.goodScore;
        gm.greats++;
    }
    private void HitBad()
    {
        gm.score += gm.badScore;
        gm.goods++;
    }
    private void Missed()
    {
        Debug.Log("Missed");
        gm.score += gm.missScore;
        gm.misses++;
        // Update score UI because getting a 'Miss' will not trigger score UI change
        gm.UpdateUI();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Note")
        {
            //Debug.Log("hitnote on");
            hitNote = true;
        }

        if (other.transform.tag == "NoteWall")
        {
            // Ensure that the player enters the note from the correct direction
            // If the player does. Recieve points.
            // If not, nothing happens atm.
            switch (other.transform.parent.GetComponent<Note>().arrowDir)
            {
                case "left":
                    if (other.transform.parent.GetComponent<Note>().laneNumber < nearestLaneNumber)
                    {
                        other.gameObject.SetActive(false);
                        if (other.transform.parent.GetComponent<Note>().isLaunch)
                        {
                            playerHitLaunch = true;
                            StartCoroutine(Camera.main.GetComponent<CameraBehaviour>().RollCamera("left"));
                        }
                        gm.score++;
                    }
                    break;

                case "right":
                    if (other.transform.parent.GetComponent<Note>().laneNumber > nearestLaneNumber)
                    {
                        other.gameObject.SetActive(false);
                        if (other.transform.parent.GetComponent<Note>().isLaunch)
                        {
                            playerHitLaunch = true;
                            StartCoroutine(Camera.main.GetComponent<CameraBehaviour>().RollCamera("right"));
                        }
                        gm.score++;
                    }
                    break;

                case "up":
                    if (other.transform.parent.GetComponent<Note>().laneNumber == nearestLaneNumber)
                    {
                        other.gameObject.SetActive(false);
                        gm.score++;
                    }
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Note")
        {
            //Player hitting the hit
            if (Input.GetKey(KeyCode.Space))
            {
                // Spawn hit particle effect
                Instantiate(gm.jet.GetComponent<Jet>().hitParticle, other.transform.position, transform.rotation);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Note")
        {
            //Debug.Log("hitnote off");
            hitNote = false;
        }
    }
}