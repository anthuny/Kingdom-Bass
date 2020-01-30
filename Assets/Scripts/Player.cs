using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool passedBeat;

    bool playerHitLaunch;

    private Renderer rend;
    public float playerWidth;

    bool hitNote;
    bool doneOnce;
    bool doneOnce3;
    public bool doneOnce2;

    float minDist;
    Transform nearestNote;
    public Transform nearestDeadNote;
    public Transform oldNearestDeadNote;
    public List<Transform> activeNotes = new List<Transform>();

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

        for (int i = 0; i < tc.notes.transform.childCount - 1; i++)
        {
            if (tc.notes.transform.GetChild(i).gameObject.GetComponent<Note>().canMove && !activeNotes.Contains(tc.notes.transform.GetChild(i)))
            {
                activeNotes.Add(tc.notes.transform.GetChild(i));
            }
        }

        minDist = Mathf.Infinity;

        for (int i = 0; i < tc.notes.transform.childCount; i++)
        {
            float dist = Vector3.Distance(transform.position, new Vector3(transform.position.x, transform.position.y, tc.notes.transform.GetChild(i).transform.position.z));

            if (minDist > dist)
            {
                nearestNote = tc.notes.transform.GetChild(i);
                minDist = dist;
            }
        }


        nearestDeadNote = nearestNote;

        if (oldNearestDeadNote != nearestDeadNote)
        {
            oldNearestDeadNote = nearestDeadNote;
            doneOnce2 = false;
        }

        if (nearestDeadNote && tc.deadNoteAssigned)
        {
            if (nearestDeadNote.transform.position.z <= transform.position.z && !doneOnce2)
            {
                doneOnce2 = true;
                tc.trackPosIntervalsList.RemoveAt(0);
                Debug.Log("removed index 1 of interval list. Point From " + tc.pointFromLastBeat + "Point to " + tc.pointToNextBeat);
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
        if (Input.GetKeyDown("d") && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber)
        {
            movingRight = true;
            if (!passedBeat)
            {
                passedBeat = true;
                doneOnce = false;
                CheckHitAccuracy();
            }
        }

        // If:
        //      player is pressing A
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        if (Input.GetKeyDown("a") && !movingRight && !movingLeft && nearestLaneNumber != 1)
        {
            movingLeft = true;
            if (!passedBeat)
            {
                passedBeat = true;
                doneOnce = false;
                CheckHitAccuracy();
            }
        }
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
        Debug.Log("pointFrom " + tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex));
        Debug.Log("point from index val " + tc.trackPosIntervalsList[0]);
        Debug.Log("PointTo " + tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex));
        Debug.Log("trackPosInBeatsGame + " + tc.trackPosInBeatsGame);
        Debug.Log("trackPosIntervals + " + tc.trackPosIntervalsList[0]);

        // Debug for next beat


        //Debug.Log("next beat " + tc.nextBeat);
        //Debug.Log("trackPosInBeatsGame + " + tc.trackPosInBeatsGame);
        
        // TODO - Future Anthony. Everytime there is a perfect - glitch, all this code underneath doesn't work correctly
        // Decent progress today. The game somewhat works with various levels of eighth waits.

        Debug.Break();

        CheckFirstHitAccuracy();
        
        // need to divide point to and polintfrom by 2 if it's double the eighth lengths. 
        // need to do this in a formula that works for every eighth length.
        if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) < 0 || tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) < 0 && (tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval)))
        {
            Debug.Log("Perfect - glitch");
            HitPerfect();
            passedBeat = false;
            return;
        }

        if (tc.pointFromLastBeat < tc.pointToNextBeat && tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
        {
            if (tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.perfectMin && tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) >= 0)
            {
                Debug.Log("Perfect 1");
                HitPerfect();
            }
            else if (tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.goodMin && tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.perfectMin)
            {
                Debug.Log("Good 1");
                HitGood();
            }
            else if (tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.badMin && tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.goodMin)
            {
                Debug.Log("Bad 1");
                HitBad();
            }
            else if (tc.pointFromLastBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.badMin)
            {
                Debug.Log("Miss 1");
                Missed();
            }
        }
        else if (tc.pointToNextBeat < tc.pointFromLastBeat)
        {
            if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.perfectMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= 0)
            {
                Debug.Log("Perfect 2");
                HitPerfect();
            }
            else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.goodMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.perfectMin)
            {
                Debug.Log("Good 2");
                HitGood();
            }
            else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.badMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.goodMin)
            {
                Debug.Log("Bad 2");
                HitBad();
            }
            else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.badMin)
            {
                Debug.Log("Miss 2");
                Missed();
            }
        }

        passedBeat = false;
    }
    private void CheckFirstHitAccuracy()
    {
        if (tc.trackPosInBeatsGame > (tc.noteTimeTaken + tc.firstInterval))
        {
            return;
        }

        if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.perfectMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= 0)
        {
            Debug.Log("Perfect 3");
            HitPerfect();
        }
        else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.goodMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.perfectMin)
        {
            Debug.Log("Good 3");
            HitGood();
        }
        else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) <= gm.badMin && tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.goodMin)
        {
            Debug.Log("Bad 3");
            HitBad();
        }
        else if (tc.pointToNextBeat / (tc.trackPosIntervals / tc.nextIndex) >= gm.badMin)
        {
            Debug.Log("Miss 3");
            Missed();
        }

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
        gm.goods++;
    }
    private void HitBad()
    {
        gm.score += gm.badScore;
        gm.bads++;
    }
    private void Missed()
    {
        Debug.Log("Missed");
        gm.score += gm.missScore;
        gm.misses++;
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