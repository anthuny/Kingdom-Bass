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


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();

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

        //Debug.Log(nearestLaneNumber);
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
            //print(pm.maxPathNumber);
        }

        // If:
        //      player is pressing A
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        if (Input.GetKeyDown("a") && !movingRight && !movingLeft && nearestLaneNumber != 1)
        {
            movingLeft = true;
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
            float a = tc.trackPosInBeats - tc.m_LastBeat;
            float b = tc.nextBeat - tc.trackPosInBeats;
            if (a > b)
            {
                passedBeat = true;
            }
            else
            {
                passedBeat = false;
            }

            Debug.Log("trackPosInbeats is " + tc.trackPosInBeats);
            Debug.Log("a is " + a);
            Debug.Log("b is " + b);
            if (passedBeat)
            {
                Debug.Log(tc.trackPosInBeats);
            }
            //Debug.Log(passedBeat);
            movingLeft = false;
            rb.AddForce(Vector3.right * gm.playerEvadeStr);
            
        }


        // Functionality of moving left
        if (movingLeft)
        {
            /*
            float a = tc.trackPosInBeats - tc.m_LastBeat;
            float b = tc.nextBeat - tc.trackPosInBeats;
            if (a > b)
            {
                passedBeat = true;
            }
            else
            {
                passedBeat = false;
            }

            if (passedBeat)
            {
                Debug.Log(tc.trackPosInBeats);
            }
            Debug.Log(passedBeat);
            */
            movingRight = false;
            rb.AddForce(Vector3.left * gm.playerEvadeStr);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Note")
        {
            //Jet Shoots at player
            gm.jet.GetComponent<Jet>().Shoot();
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
}