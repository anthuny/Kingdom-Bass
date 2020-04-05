﻿using System.Collections;
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
    private AudioManager am;
    public Animator animator;

    private float pathWidth;

    public bool movingRight;
    public bool movingRightNoShield;
    public bool movingLeft;
    public bool movingLeftNoShield;
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
    public float missCurrentPointInBeats;

    public List<Transform> activeNotes = new List<Transform>();
    public List<Transform> notesInfront = new List<Transform>();
    public List<GameObject> electricNotes = new List<GameObject>();
    public GameObject OldClosestNoteInFront;
    public GameObject closestNoteInFront;
    public Note closestNoteInFrontScript;

    // the nearest note of any type
    public Transform nearestAnyNote;
    // the nearest NON bomb note
    public Transform nearestNote;
    public Transform nearestSlider;
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
    public Slider nearestSliderScript;

    public Text accuracyUI;

    public bool validMovement;

    [HideInInspector]
    public Vector3 playerPos;
    [HideInInspector]
    public Vector3 oldPlayerPos;

    private bool movedLeft;
    private bool movedRight;

    private bool movingNoShield;
    private bool attemptedBlast;

    public Transform nearestSliderStartEnd;

    private Vector3 playerPos3;

    public float hitLocationInBeats;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        cm = Camera.main.GetComponent<CameraBehaviour>();
        am = FindObjectOfType<AudioManager>();

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
        ControllerInputs();
        StartCoroutine("KeyBoardMovement");
        FindNearestNote();
        UpdateShield();
        UpdateNotesInFront();
        AssignMisses();
        IncMaxAcc();
        CheckMissForSlider();

        missCurrentPointInBeats = tc.trackPosInBeatsGame;

        if (notesInfront.Count > 0)
        {
            closestNoteInFront = notesInfront[0].gameObject;
        }
    }
    // Assigns misses if the note is too far behind the player to get score from
    void AssignMisses()
    {
        if (nearestAnyNote)
        {
            nearestAnyNoteScript = nearestAnyNote.gameObject.GetComponent<Note>();

            CalculateMissPointFrom();

            if (missCurrentPointInBeats > (nearestNoteScript.beatWaitCur + tc.selectedMap.noteTimeTaken) + gm.goodMin && nearestNoteScript.hitAmount < 1 && !nearestNoteScript.missed && nearestNoteScript.noteType != "slider" && nearestNoteScript.noteType != "bomb")
            {
                //Debug.Log(nearestNote.transform.position.z);
                if (nearestNote.transform.position.z < transform.position.z)
                {
                    Debug.Log("3");
                    Missed(false);
                    //Debug.Break();
                }
            }
        }
    }

    void IncMaxAcc()
    {
        // If there is no nearest note, do not continue
        if (!nearestNote)
        {
            return;
        }

        if (1 - missPointFrom < newGoodMiss + 0.05f && nearestNote.transform.position.z > transform.position.z && !nearestNoteScript.noteCalculatedAcc && nearestNoteScript.noteType != "bomb" && nearestNoteScript.noteType != "slider")
        {
            nearestNoteScript.noteCalculatedAcc = true;

            // For every note, increase the max accuaracy by 3. (3 is the value perfect gives)
            gm.totalAccuracyMax += 3;
            //Debug.Log(nearestNote);
            //Debug.Log("2");
        }

        if (nearestSliderScript)
        {
            if (nearestSliderScript.sliderIntervalsInFront.Count <= 1)
            {
                if (!nearestSliderScript.noteCalculatedAcc && nearestNote.transform.position.z > transform.position.z && nearestNoteScript.noteType == "slider")
                {
                    nearestSliderScript.noteCalculatedAcc = true;

                    // For every note, increase the max accuaracy by 3. (3 is the value perfect gives)
                    gm.totalAccuracyMax += 3;
                    //Debug.Log("a");
                }
            }
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
            if (notesInfront.Count > 0)
            {
                tc.nextNoteInBeats = tc.selectedMap.noteTimeTaken + notesInfront[0].GetComponent<Note>().beatWaitCur;

                // i think this is for the last note of the map
                if (nearestNoteScript.noteNumber == tc.noteLanes.Count && nearestNoteScript.behindPlayer)
                {
                    tc.nextNoteInBeats = tc.previousNoteBeatTime3 + 2;
                }

                missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
                //Debug.Log("missPointFrom " + missPointFrom + " nextNoteInbeats " + tc.nextNoteInBeats + " previousnoteBeatTime3 " + (tc.previousNoteBeatTime3) + " missCurrentPointInBeats " + missCurrentPointInBeats);
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
                nearestAnyNote = t;
                minDist = dist;
            }
        }

        if (nearestAnyNote)
        {
            nearestAnyNoteScript = nearestAnyNote.gameObject.GetComponent<Note>();
        }


        float minDist2 = Mathf.Infinity;
        float minDist4 = Mathf.Infinity;

        // Detect the nearest non note position
        foreach (Transform t in activeNotes)
        {
            float dist2 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            float dist4 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            if (minDist2 > dist2 && t.GetComponent<Note>().noteType != "bomb")
            {
                nearestNote = t;
                minDist2 = dist2;
            }

            if (minDist4 > dist4 && t.GetComponent<Note>().isEndOfSlider || t.GetComponent<Note>().isStartOfSlider && minDist4 > dist4)
            {
                if (t.gameObject.GetComponent<Note>().sliderLr)
                {
                    nearestSliderStartEnd = t;
                    nearestSliderScript = t.gameObject.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>();
                    minDist4 = dist4;
                }
            }
        }

        if (nearestNote)
        {
            nearestNoteScript = nearestNote.gameObject.GetComponent<Note>();
        }

        float minDist3 = Mathf.Infinity;
        if (gm.sliders.Count > 1)
        {
            foreach (Transform t in gm.sliders)
            {
                float dist3 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                                new Vector3(transform.position.x, transform.position.y, transform.position.z));

                if (minDist3 > dist3)
                {
                    nearestSlider = t;
                    minDist3 = dist3;
                }
            }
        }
        else if (gm.sliders.Count == 1)
        {
            nearestSlider = gm.sliders[0];
        }
    }

    public void UpdateNotesInFront()
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
            // Add to the notesInFront list only add the note if it's NOT already in the list
            if (!notesInfront.Contains(activeNotes[activeNotes.Count - 1].gameObject.transform) &&
                activeNotes[activeNotes.Count - 1].gameObject.GetComponent<Note>().noteType != "bomb")
            {
                notesInfront.Add(activeNotes[activeNotes.Count - 1].gameObject.transform);
            }

            // Add to the electricNotes list only add the note if it's NOT already in the list
            if (!electricNotes.Contains(activeNotes[activeNotes.Count - 1].gameObject) &&
                activeNotes[activeNotes.Count - 1].gameObject.GetComponent<Note>().noteType != "bomb"
                && activeNotes[activeNotes.Count - 1].gameObject.GetComponent<Note>().noteDir != "up"
                && activeNotes[activeNotes.Count - 1].gameObject.GetComponent<Note>().noteDir != "down")
            {
                electricNotes.Add(activeNotes[activeNotes.Count - 1].gameObject);
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
        // Do not allow any player movements if the game is paused
        if (gm.gamePaused || gm.controllerConnected)
        {
            return;
        }

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
        if (Input.GetKeyUp(KeyCode.L) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber && !blastInput && isShielding)
        {
            movingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.L) && nearestLaneNumber != pm.maxPathNumber && !blastInput && !isShielding)
        {
            movingRightNoShield = true;
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            movingRightNoShield = false;
        }

        // If:
        //      player is pressing for moving left
        //      playing is NOT moving left or right already
        //      player is not in the most LEFT lane
        //      blastInput is false
        //      player is shielding
        if (Input.GetKeyUp(KeyCode.A) && !movingRight && !movingLeft && nearestLaneNumber != 1 && !blastInput && isShielding)
        {
            movingLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.A) && nearestLaneNumber != 1 && !blastInput && !isShielding)
        {
            movingLeftNoShield = true;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            movingLeftNoShield = false;
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
            AssignHitLocation();
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

    void ControllerInputsPost()
    {
        transform.position = playerPos;

        movingNoShield = true;

        pm.FindNearestPath(true);
    }

    void MoveShieldSFX()
    {
        am.PlaySound("MoveShield");
    }

    void ControllerInputs()
    {
        if (!gm.controllerConnected || gm.activeScene != "Game")
        {
            return;
        }

        // Stops strange drifting behaviour
        //Vector3 playerPos2 = transform.position;
        //playerPos2.z = 1;
        //transform.position = playerPos2;

        if (gm.shieldingVal == 0)
        {
            isShielding = true;
        }
        else if (gm.shieldingVal == 1)
        {
            isShielding = false;
        }

        #region Moving Horizontally with no shield
        #region Moving slowest speed

        // Moving left / right with no shield
        // If moving left
        if (gm.noShieldMove.x < 0 && gm.noShieldMove.x > -.9f && !isShielding && transform.position.x > 0.3f)
        {
            playerPos.x -= gm.shieldOffSpeed * gm.lowSpeed * Time.deltaTime;
            //Debug.Log(gm.noShieldMove.x);
            animator.SetTrigger("isMovingLeftNSSlow");
            ControllerInputsPost();
        }
        // If moving right
        else if (gm.noShieldMove.x > 0 && gm.noShieldMove.x < .9f && !isShielding && transform.position.x < 5.7f)
        {
            playerPos.x += gm.shieldOffSpeed * gm.lowSpeed * Time.deltaTime;
            //Debug.Log(gm.noShieldMove.x);
            animator.SetTrigger("isMovingRightNSSlow");
            ControllerInputsPost();
        }
        #endregion
        #region Moving fastest speed
        // Moving left / right with no shield

        // If moving left
        if (gm.noShieldMove.x < -.9f && !isShielding && transform.position.x > 0)
        {
            playerPos.x -= gm.shieldOffSpeed * gm.maxSpeed * Time.deltaTime;
            //Debug.Log(gm.noShieldMove.x);
            animator.SetTrigger("isMovingLeftNSFast");
            ControllerInputsPost();
        }
        // If moving right
        else if (gm.noShieldMove.x > .9f && !isShielding && transform.position.x < 5.7f)
        {
            playerPos.x += gm.shieldOffSpeed * gm.maxSpeed * Time.deltaTime;
            //Debug.Log(gm.noShieldMove.x);
            animator.SetTrigger("isMovingRightNSFast");
            ControllerInputsPost();
        }
        #endregion
        #endregion
        #region Moving Horizontally with shield
        // Moving right with shield
        if (gm.move.x > gm.movthreshHold && isShielding && !movedRight && transform.position.x < 5.7f)
        {
            movedRight = true;
            movingRight = true;
            validMovement = true;

            playerPos.x = pathWidth * nearestLaneNumber;
            playerPos.z = 1;
            transform.position = playerPos;

            pm.FindNearestPath(true);

            animator.SetBool("isIdle", false);
            animator.SetTrigger("isMovingRight");
            MoveShieldSFX();
        }
        // Moving left with shield
        else if (gm.move.x < -gm.movthreshHold && isShielding && !movedLeft && transform.position.x > 0.3f)
        {
            movedLeft = true;
            movingLeft = true;
            validMovement = true;

            playerPos.x = pathWidth * (nearestLaneNumber - 2);
            playerPos.z = 1;
            transform.position = playerPos;

            pm.FindNearestPath(true);

            animator.SetBool("isIdle", false);
            animator.SetTrigger("isMovingLeft");
            MoveShieldSFX();
        }
        #endregion

        if (gm.move.x == 0)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }


        // Blast maintenance
        if (gm.blastLVal == 0 && gm.blastRVal == 0)
        {
            attemptedBlast = false;
        }

        // Blast
        if (gm.blastLVal > 0 && gm.blastRVal > 0 && !attemptedBlast && isShielding)
        {
            attemptedBlast = true;
            AssignHitLocation();
        }


        #region Resetting player position to middle of lane
        // Reset player's postiion to middle of lane when shielding
        if (isShielding && movingNoShield && gm.move.x == 0)
        {
            movingNoShield = false;

            // Set player position to the middle of the nearest lane
            playerPos.x = (nearestLaneNumber - 1) * 1.5f;
            transform.position = playerPos;
        }

        // Resetting shield move left / right values if not moving
        if (gm.move.x == 0 && isShielding)
        {
            movedRight = false;
            movedLeft = false;
        }
        #endregion
    }
    IEnumerator KeyBoardMovement()
    {
        playerPos.z = 1.85f;
        transform.position = playerPos;

        if (gm.controllerConnected || gm.activeScene != "Game")
        {
            yield break;
        }

        // Stops strange drifting behaviour
        //Vector3 playerPos2 = transform.position;
        //playerPos2.z = 1;
        //transform.position = playerPos2;

        //playerPos = transform.position;

        // Ensures that there is a nearest path to begin with
        // There must be one for this code to work. 
        if (!pm.nearestPath)
        {
            yield break;
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


            animator.SetBool("isIdle", false);
            animator.SetTrigger("isMovingRight");

            pm.FindNearestPath(true);

            MoveShieldSFX();
        }

        // Functionality of moving right without shield   
        else if (movingRightNoShield && !isShielding && tc.selectedMap.title != "Tutorial" && transform.position.x < 5.7f || gm.tutorialStage >= 3 && movingRight && !isShielding && transform.position.x < 5.7f)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x += gm.shieldOffSpeed * Time.deltaTime;
            transform.position = playerPos;

            animator.SetTrigger("isMovingRightNSFast");

            movingNoShield = true;

            pm.FindNearestPath(false);
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

            //playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
            //Debug.Log("called nearestlanenumberfunction");
        }

        // Functionality of moving right WITHOUT shield during first few stages of tutorial
        if (movingRightNoShield && !isShielding && tc.selectedMap.title == "Tutorial" && gm.tutorialStage > 0 && gm.tutorialStage < 3)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x += gm.shieldOffSpeed * Time.deltaTime;
            transform.position = playerPos;

            pm.FindNearestPath(true);

            yield return new WaitForSeconds(gm.timeForMoveBack);

            //playerPos.x = 3;
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

            animator.SetBool("isIdle", false);
            animator.SetTrigger("isMovingLeft");

            pm.FindNearestPath(true);

            MoveShieldSFX();
        }


        // Functionality of moving left without shield
        if (movingLeftNoShield && !isShielding && tc.selectedMap.title != "Tutorial" && transform.position.x > 0.3f || gm.tutorialStage >= 3 && movingLeft && !isShielding && transform.position.x > 0.3f)
        {
            movingLeft = false;
            movingRight = false;
            validMovement = false;

            playerPos.x -= gm.shieldOffSpeed * Time.deltaTime;
            transform.position = playerPos;

            animator.SetTrigger("isMovingLeftNSFast");

            movingNoShield = true;

            pm.FindNearestPath(false);
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

            //playerPos.x = 3;
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

            playerPos.x += gm.shieldOffSpeed * Time.deltaTime;
            transform.position = playerPos;

            pm.FindNearestPath(true);

            yield return new WaitForSeconds(gm.timeForMoveBack);

            //playerPos.x = 3;
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }

        #endregion

        #region Resetting player position to middle of lane
        // Reset player's postiion to middle of lane when shielding
        if (isShielding && movingNoShield && gm.move.x == 0)
        {
            movingNoShield = false;

            // Set player position to the middle of the nearest lane
            playerPos.x = (nearestLaneNumber - 1) * 1.5f;
            transform.position = playerPos;
        }

        // Resetting shield move left / right values if not moving
        if (gm.move.x == 0 && isShielding)
        {
            movedRight = false;
            movedLeft = false;
        }

        if (!Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.L))
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
        #endregion
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
            //AssignFromAndToValuesNoteAndLaunch();
            AssignHitLocation();
        }
    }

    void AssignHitLocation()
    {
        if (nearestNote)
        {
            hitLocationInBeats = missCurrentPointInBeats;

            if (nearestNoteScript.noteType == "note" || nearestNoteScript.noteType == "launch")
            {
                CheckForNoteAndLaunchEffect();
            }

            else if (nearestNoteScript.noteType == "blast")
            {
                CheckForBlastEffect();
            }
        }
    }
    private void CheckForBlastEffect()
    {
        // if the nearest not has already been hit once, miss the player for attempt
        if (nearestNoteScript.hitAmount > 1)
        {
            Missed(false);
        }

        nearestNoteScript.hitAmount++;

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteScript.canGetNote)
        {
            return;
        }

        gm.scoreIncreased = true;

        nearestNoteScript.canGetNote = false;

        CheckHitAccuracy();
    }
    private void CheckForNoteAndLaunchEffect()
    {
        // if the nearest not has already been hit once, miss the player for attempt
        // OR if the movement that was made was made without the shield on
        if (!nearestNote || !validMovement)
        {
            return;
        }

        if (nearestNoteScript.hitAmount > 1 && !nearestNoteScript.missed && nearestNoteScript.noteType != "note" && nearestNoteScript.noteDir != "up")
        {
            Missed(false);
        }

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteScript.canGetNote)
        {
            return;
        }

        // If the nearest note is an up arrow, ignore it. 
        if (nearestNoteScript.noteDir == "up")
        {
            return;
        }

        gm.scoreIncreased = true;

        nearestNoteScript.canGetNote = false;

        nearestNoteScript.hitAmount++;

        // In the case of a note with a left arrow
        if (nearestNoteScript.noteDir == "left" && nearestNoteScript.noteType != "launch" && nearestNoteScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteScript.laneNumber == oldNearestLaneNumber - 1 && nearestNoteScript.laneNumber == nearestLaneNumber)
            {
                //Debug.Log("1");
                //Debug.Log("oldNearestLaneNumber " + oldNearestLaneNumber);
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                //Debug.Log("6");
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
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                //Debug.Log("7");
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
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                //Debug.Log("8");
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
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false);
                //Debug.Log("9");
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
        if (nearestNoteScript.noteDir == "up" && isShielding && nearestNoteScript.noteType == "note")
        {
            // If the note is behind the player
            if (nearestNoteScript.gameObject.transform.position.z < transform.position.z && !nearestNoteScript.missed && nearestNoteScript.laneNumber == nearestLaneNumber)
            {   
                // Tell the note that it has just given score, so it knows not to allow it again in the future
                nearestNoteScript.doneUpArrow = true;

                // Give 'perfect' score
                HitPerfect();
                //Debug.Log("a");

                // Increase the hit amounts of the up note by 1
                nearestNoteScript.hitAmount++;

                // Make it so the up arrow can not give score again once done once aready
                nearestNoteScript.canGetNote = false;
            }
        }
    }

    void CheckHitAccuracy()
    {
        Debug.Log("1");
        float difference = Mathf.Abs(nearestNoteScript.beatWaitCur + tc.selectedMap.noteTimeTaken - hitLocationInBeats);
        //Debug.Log("difference " + difference + " beatWaitCur " + nearestNoteScript.beatWaitCur + " hitLocationInBeats " + hitLocationInBeats);
        //Debug.Break();
        
        if (difference < gm.perfectMin)
        {
            HitPerfect();
        }

        else if (difference < gm.greatMin)
        {
            HitGreat();
        }

        else if (difference < gm.goodMin)
        {
            HitGood();
        }

        else if (difference > gm.goodMin)
        {
            Missed(false);
            Debug.Log("2");
        }
    }

    public void HitPerfect()
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

        //am.PlaySound("NoteHit");

        validMovement = false;
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

        //am.PlaySound("NoteHit");

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

        //am.PlaySound("NoteHit");

        validMovement = false;
        //Debug.Break();
    }

    void CheckMissForSlider()
    {
        if (nearestNote)
        {
            if (nearestNoteScript.noteType == "slider")
            {
                gm.checkForSliderIntervals = true;
            }
            else
            {
                gm.checkForSliderIntervals = false;
            }
        }
    }
    public void Missed(bool hitByBomb)
    {
        if (nearestAnyNote == null)
        {
            Debug.Log("?");
            return;
        }

        if (gm.comboMulti >= 2)
        {
            animator.SetTrigger("ComboReset");
        }

        if (!hitByBomb && nearestNoteScript.noteType != "slider")
        {
            nearestNoteScript.canGetNote = false;
            nearestNoteScript.missed = true;
            gm.UpdateHealth(gm.regenMiss);
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
            gm.comboMulti = 1;
            gm.updateGameUI();
            return;
        }

        if (!hitByBomb && nearestNoteScript.noteType == "slider")
        {
            // Increase the max accuracy if the note got missed
            if (!nearestSliderScript.noteCalculatedAcc)
            {
                nearestSliderScript.noteCalculatedAcc = true;
                gm.totalAccuracyMax += 3;
            }

            if (nearestSliderStartEnd)
            {
                if (!nearestSliderStartEnd.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().missed)
                {
                    nearestSliderScript.noteCalculatedAcc = true;
                    nearestNoteScript.missed = true;
                    nearestSliderStartEnd.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().Missed();
                    gm.UpdateHealth(gm.regenSlider);
                    gm.scoreIncreased = true;

                    gm.misses++;

                    gm.score += gm.missScore;

                    //Update player accuracy UI
                    accuracyUI.text = gm.missScoreName;

                    // Update total accuracy UI
                    gm.UpdateTotalAccuracy();

                    // Began the cooldown till the acuracy ui text vanishes
                    StartCoroutine("DiminishAccuracyUI");
                    gm.comboMulti = 1;
                    gm.updateGameUI();
                    return;
                }
            }
            else if (nearestNote)
            {
                nearestSliderScript.noteCalculatedAcc = true;
                nearestSliderScript.Missed();
                gm.UpdateHealth(gm.regenSlider);
                gm.scoreIncreased = true;

                gm.misses++;

                gm.score += gm.missScore;

                //Update player accuracy UI
                accuracyUI.text = gm.missScoreName;

                // Update total accuracy UI
                gm.UpdateTotalAccuracy();

                // Began the cooldown till the acuracy ui text vanishes
                StartCoroutine("DiminishAccuracyUI");
                gm.comboMulti = 1;
                gm.updateGameUI();
                return;
            }

            return;
        }

        else
        {
            gm.UpdateHealth(gm.regenBomb);
        }

        gm.comboMulti = 1;
        gm.updateGameUI();
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