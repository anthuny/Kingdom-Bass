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
    private bool oldIsShielding;

    private float currentPointInBeats;
    public float missCurrentPointInBeats;

    public List<Transform> activeNotes = new List<Transform>();
    public List<Transform> activeAllNotes = new List<Transform>();
    public List<Transform> notesInfront = new List<Transform>();
    public List<GameObject> electricNotes = new List<GameObject>();
    public GameObject OldClosestNoteInFront;
    public GameObject closestNoteInFront;
    public GameObject closestElectricNoteInFront;
    public Note closestElectricNoteInFrontScript;
    public Note closestNoteInFrontScript;

    // the nearest note of any type
    public Transform nearestAnyNote;
    // the nearest NON bomb note
    public Transform nearestNote;
    public Transform nearestBlast;
    public Transform nearestBomb;
    public Note nearestBlastScript;
    public Transform nearestSlider;
    public GameObject closestBehindNote;
    public Note closestBehindNoteScript;

    public Material shieldMat;
    private Color shieldColor;
    public GameObject shield;
    private Animator shieldAnimator;

    public float newPerfect;
    public float newGreat;
    public float newGood;
    public float newGoodMiss;
    private float oldNewGoodMiss;

    public Note nearestAnyNoteScript;
    public Note nearestNoteScript;
    public Note nearestNoteGameScript;
    public Transform nearestNoteGame;
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

    private Coroutine blockCoroutine;
    private Coroutine diminishAccuracyUI;
    private Coroutine turnOffShield;

    public TrailRenderer trail;
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

        shieldAnimator = shield.GetComponent<Animator>();
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
        UpdateNoteCollection();
        AssignMisses();
        IncMaxAcc();
        CheckMissForSlider();
        Shield();
        ShieldCheck();
        Trail();

        missCurrentPointInBeats = tc.trackPosInBeatsGame;
    }

    void Trail()
    {
        if (isShielding && trail.enabled)
        {
            trail.enabled = false;
        }
        else if (!isShielding && !trail.enabled)
        {
            trail.enabled = true;
        }
    }
    void ShieldCheck()
    {
        // Sometimes the shield object is off and cant easily turn back on, this stops it
        if (tc.selectedMap)
        {
            if (!shield.activeSelf && isShielding && tc.selectedMap.trackCodeName != "Tutorial" && !gm.playerDead)
            {
                shield.SetActive(true);
            }
        }

    }

    public IEnumerator RepositionModSlider(Note noteScript)
    {
        yield return new WaitForSeconds(gm.tutPosResetTime);
        for (int i = 1; i < 6; i++)
        {
            if (noteScript.laneNumber == i)
            {
                playerPos.x = pathWidth * (i - 1);
                transform.position = playerPos;
                pm.FindNearestPath(false);
                yield break;
            }
        }
    }
    public IEnumerator RepositionMod(Note noteScript)
    {
        yield return new WaitForSeconds(gm.tutPosResetTime);

        if (noteScript.noteDir == "up" && noteScript.noteType == "note" || noteScript.noteType == "slider" && noteScript.isStartOfSlider)
        {
            for (int i = 1; i < 6; i++)
            {
                if (noteScript.laneNumber == i)
                {
                    playerPos.x = pathWidth * (i - 1);
                    transform.position = playerPos;
                    pm.FindNearestPath(false);
                    yield break;
                }
            }
        }


        if (noteScript.noteType == "note" || noteScript.noteType == "launch")
        {
            switch (noteScript.noteDir)
            {
                case "left":
                    for (int i = 1; i < 5; i++)
                    {
                        if (noteScript.noteDir == "left" && noteScript.laneNumber == i)
                        {
                            if (noteScript.noteType == "note")
                            {
                                playerPos.x = pathWidth * i;
                                transform.position = playerPos;
                                pm.FindNearestPath(false);
                                yield break;
                            }
                            else if (noteScript.noteType == "launch" && i > 1)
                            {
                                playerPos.x = pathWidth * i;
                                transform.position = playerPos;
                                pm.FindNearestPath(false);
                                yield break;
                            }
                        }
                    }
                    break;

                case "right":
                    for (int i = 2; i < 6; i++)
                    {
                        if (noteScript.noteDir == "right" && noteScript.laneNumber == i)
                        {
                            if (noteScript.noteType == "note")
                            {
                                playerPos.x = pathWidth * (i - 2);
                                transform.position = playerPos;
                                pm.FindNearestPath(false);
                                yield break;
                            }
                            else if (noteScript.noteType == "launch" && i < 5)
                            {
                                playerPos.x = pathWidth * (i - 2);
                                transform.position = playerPos;
                                pm.FindNearestPath(false);
                                yield break;
                            }
                        }
                    }
                    break;
            }
        }
    }

    // Assigns misses if the note is too far behind the player to get score from
    void AssignMisses()
    {
        if (closestBehindNote)
        {
            CalculateMissPointFrom();

            if (missCurrentPointInBeats > (closestBehindNoteScript.beatWaitCur + tc.selectedMap.noteTimeTaken) + gm.goodMin 
                && closestBehindNoteScript.hitAmount < 1 && !closestBehindNoteScript.missed && closestBehindNoteScript.noteType != "slider" && closestBehindNoteScript.noteType != "bomb")
            {
                //Debug.Log(nearestNote.transform.position.z);
                if (closestBehindNote.transform.position.z < transform.position.z)
                {
                    //nearestNoteGameScript.missed = true;
                    Missed(false, closestBehindNoteScript, gameObject.name);
                }
            }
        }
    }

    void IncMaxAcc()
    {
        // If there is no nearest note, do not continue
        if (!nearestNoteGame)
        {
            return;
        }

        if (nearestNoteGame.transform.position.z > transform.position.z && !nearestNoteGameScript.noteCalculatedAcc && nearestNoteGameScript.noteType != "bomb" && nearestNoteGameScript.noteType != "slider")
        {
            nearestNoteGameScript.noteCalculatedAcc = true;

            // For every note, increase the max accuaracy by 3. (3 is the value perfect gives)
            gm.totalAccuracyMax += 3;
            //Debug.Log(nearestNoteGame);
            //Debug.Log("2");
        }

        if (nearestSliderScript)
        {
            if (nearestSliderScript.sliderIntervalsInFront.Count <= 1)
            {
                if (!nearestSliderScript.noteCalculatedAcc && nearestNoteGame.transform.position.z > transform.position.z && nearestNoteGameScript.noteType == "slider")
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

        if (tc.nextIndex3 < tc.noteLanes.Count)
        {
            if (notesInfront.Count > 0)
            {
                tc.nextNoteInBeats = tc.selectedMap.noteTimeTaken + notesInfront[0].GetComponent<Note>().beatWaitCur;
            }

            // i think this is for the last note of the map
            if (nearestNoteScript.noteNumber == tc.noteLanes.Count && nearestNoteScript.behindPlayer)
            {
                tc.nextNoteInBeats = tc.previousNoteBeatTime3 + 2;
            }

            missPointFrom = 1 - ((Mathf.InverseLerp(tc.nextNoteInBeats, tc.previousNoteBeatTime3, missCurrentPointInBeats)));
            //Debug.Log("missPointFrom " + missPointFrom + " nextNoteInbeats " + tc.nextNoteInBeats + " previousnoteBeatTime3 " + (tc.previousNoteBeatTime3) + " missCurrentPointInBeats " + missCurrentPointInBeats);
        }
    }

    public void DestroyFurthestNoteNote()
    {
        Invoke("DestroyFurthestNote", 0.2f);
    }
    void FindNearestNote()
    {
        if (gm.activeScene != "Game")
        {
            return;
        }

        if (notesInfront.Count > 0)
        {
            closestNoteInFront = notesInfront[0].gameObject;
        }

        for (int i = 0; i < notesInfront.Count - 1; i++)
        {
            //closestElectricNoteInFront
            if (notesInfront[i].GetComponent<Note>().noteDir == "left" || notesInfront[i].GetComponent<Note>().noteDir == "right")
            {
                closestElectricNoteInFront = notesInfront[i].gameObject;
                closestElectricNoteInFrontScript = closestElectricNoteInFront.GetComponent<Note>();
                break;
            }
        }

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
        float minDist5 = Mathf.Infinity;

        // Detect the nearest non note position
        foreach (Transform t in activeNotes)
        {
            float dist2 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            float dist4 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            float dist5 = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, t.position.z),
                            new Vector3(transform.position.x, transform.position.y, transform.position.z));

            if (minDist5 > dist5 && t.GetComponent<Note>().noteType == "blast")
            {
                nearestBlast = t;
                nearestBlastScript = nearestBlast.GetComponent<Note>();
                minDist5 = dist5;
            }

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

            if (missCurrentPointInBeats > (nearestNoteScript.beatWaitCur + tc.selectedMap.noteTimeTaken) + gm.goodMin)
            {
                if (notesInfront.Count > 0)
                {
                    nearestNoteGameScript = notesInfront[0].GetComponent<Note>();
                    nearestNoteGame = nearestNoteGameScript.gameObject.transform;

                }
                else
                {
                    nearestNoteGameScript = null;
                }
            }
            else
            {
                nearestNoteGameScript = nearestNoteScript;
                nearestNoteGame = nearestNote;
            }
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

    public void UpdateNoteCollection()
    {
        // If the track is not in progress
        // OR there is less then 1 active note, stop
        if (!tc.trackInProgress || activeNotes.Count < 1)
        {
            return;
        }

        else if (notesInfront.Count > 1)
        {
            gm.doneOnce2 = true;
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

        //float closestDistance = 0;
        foreach (Transform Object in activeNotes)
        {
            //float ObjectDistance = Vector3.Distance(transform.position, Object.transform.position);
            if (Object.gameObject.transform.position.z < transform.position.z)
            {
                closestBehindNote = Object.gameObject;
                closestBehindNoteScript = closestBehindNote.GetComponent<Note>();
                //closestDistance = ObjectDistance;
            }
        }
    }
    void Inputs()
    {
        // Do not allow any player movements if the game is paused
        if (gm.gamePaused || gm.controllerConnected || gm.activeScene != "Game")
        {
            return;
        }

        #region Moving Right Input
        if (Input.GetKeyDown(KeyCode.L) && !movingLeft && !movingRight && nearestLaneNumber != pm.maxPathNumber && !blastInput && isShielding)
        {
            movingRight = true;

            // Reposition player for the first 2 stages of the tutorial
            if (tc.selectedMap.title == "Tutorial" && gm.tutorialStage <= 2)
            {
                Invoke("RepositionPlayer", .2f);
            }
        }

        if (Input.GetKeyDown(KeyCode.L) && nearestLaneNumber != pm.maxPathNumber && !blastInput && !isShielding ||
            Input.GetKeyDown(KeyCode.L) && nearestLaneNumber != pm.maxPathNumber && !blastInput && !isShielding)
        {
            movingRightNoShield = true;
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            movingRightNoShield = false;
        }

        #endregion

        #region Moving Left Input
        if (Input.GetKeyDown(KeyCode.A) && !movingRight && !movingLeft && nearestLaneNumber != 1 && !blastInput && isShielding)
        {
            movingLeft = true;

            // Reposition player for the first 2 stages of the tutorial
            if (tc.selectedMap.title == "Tutorial" && gm.tutorialStage <= 2)
            {
                Invoke("RepositionPlayer", .2f);
            }
        }

        if (Input.GetKeyDown(KeyCode.A) && nearestLaneNumber != 1 && !blastInput && !isShielding ||
            Input.GetKeyDown(KeyCode.A) && nearestLaneNumber != 1 && !blastInput && !isShielding)
        {
            movingLeftNoShield = true;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            movingLeftNoShield = false;
        }
        #endregion

        #region Slider Speed alteration
        if (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.K)))
        {
            gm.slowSpeedMultCur = gm.slowSpeedMult;
        }
        else
        {
            gm.slowSpeedMultCur = 1;
        }
        #endregion

        #region Blast Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            blastInput = true;
            if (nearestNoteGame)
            {
                if (nearestNoteGameScript.noteType == "blast")
                {
                    AssignHitLocation(true);
                }
            }
        }
        else
        {
            blastInput = false;
        }
        #endregion

        #region Shielding Input
        // if the play inputs to stop shielding stop shielding
        if (Input.GetKey(KeyCode.Space) && tc.selectedMap.title != "Tutorial" || Input.GetKey(KeyCode.Space) && gm.tutorialStage >= 8)
        {
            isShielding = false;
        }

        // Shield is active when the shield button is not being held down
        else if (!Input.GetKey(KeyCode.Space))
        {
            isShielding = true;
        }
        #endregion  
    }

    void ControllerInputsPost()
    {
        transform.position = playerPos;

        movingNoShield = true;

        pm.FindNearestPath(true);
    }

    void MoveShieldSFX()
    {
        am.PlaySound("Player_ShieldMove");
    }

    void Shield()
    {
        if (gm.playerDead && shield.activeSelf)
        {
            shield.SetActive(false);
            return;
        }

        if (tc.selectedMap)
        {
            if (tc.selectedMap.trackCodeName == "Tutorial" && gm.tutorialStage <= 7)
            {
                if (shield.activeSelf)
                {
                    shield.SetActive(false);
                }
                return;
            }
        }

        if (oldIsShielding != isShielding)
        {
            oldIsShielding = isShielding;

            if (isShielding)
            {
                if (nearestBlast)
                {
                    if (nearestBlastScript.distToPlayer < 10)
                    {
                        return;
                    }
                }

                if (!shield.activeSelf)
                {
                    if (turnOffShield != null)
                    {
                        StopCoroutine(turnOffShield);
                    }
                    shield.SetActive(true);
                }

                am.PlaySound("ShieldOn");

                // Play shield turn on animation     
                shieldAnimator.SetBool("Idle", false);
                shieldAnimator.SetTrigger("ShieldOn");
                shieldAnimator.SetBool("Idle", true);
            }

            else
            {
                if (nearestBlast)
                {
                    if (nearestBlastScript.distToPlayer < 10)
                    {
                        return;
                    }
                }

                am.PlaySound("ShieldOff");

                // Play shield turn off animation
                shieldAnimator.SetBool("Idle", false);
                shieldAnimator.SetTrigger("ShieldOff");
                shieldAnimator.SetBool("Idle", true);

                if (turnOffShield != null)
                {
                    StopCoroutine(turnOffShield);
                }
                turnOffShield = StartCoroutine(TurnOffShield());
            }
        }
    }

    IEnumerator TurnOffShield()
    {
        yield return new WaitForSeconds(.15f);
        shield.SetActive(false);
    }
    public void KillPlayer()
    {
        animator.SetTrigger("Death");
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
            AssignHitLocation(true);
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

        if (!pm.nearestPath)
        {
            yield break;
        }

        #region Normal - Moving Right
        // Functionality of moving right with shield
        if (movingRight && isShielding)
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
        else if (movingRightNoShield && !isShielding && transform.position.x < 5.7f)
        {
            movingRight = false;
            movingLeft = false;
            validMovement = false;

            playerPos.x += (gm.shieldOffSpeed * gm.slowSpeedMultCur) * Time.deltaTime;
            transform.position = playerPos;

            animator.SetTrigger("isMovingRightNSFast");

            movingNoShield = true;

            pm.FindNearestPath(false);
        }

        #endregion
        #region Normal - Moving Left
        // Functionality of moving left with shield
        if (movingLeft && isShielding)
        {
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
        if (movingLeftNoShield && !isShielding && transform.position.x > 0.3f)
        {
            movingLeft = false;
            movingRight = false;
            validMovement = false;

            playerPos.x -= (gm.shieldOffSpeed * gm.slowSpeedMultCur) * Time.deltaTime;
            transform.position = playerPos;

            animator.SetTrigger("isMovingLeftNSFast");

            movingNoShield = true;

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
    public void SetNearestLaneNumber(bool forScore, bool forBlast, GameObject nearestPath)
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
            AssignHitLocation(forBlast);
        }
    }

    void AssignHitLocation(bool forBlast)
    {
        //Debug.Log(nearestNoteGame);
        if (nearestNoteGame)
        {
            hitLocationInBeats = missCurrentPointInBeats;

            //Debug.Log(forBlast + " " + nearestNoteGameScript.noteType);
            if (!forBlast && nearestNoteGameScript.noteType == "note" || nearestNoteGameScript.noteType == "launch")
            {
                //Debug.Log("1");
                CheckForNoteAndLaunchEffect();
            }

            else if (forBlast && nearestNoteGameScript.noteType == "blast")
            {
                //Debug.Log("2");
                CheckForBlastEffect();
            }
        }
    }
    private void CheckForBlastEffect()
    {
        // if the nearest not has already been hit once, miss the player for attempt
        if (nearestNoteGameScript.hitAmount > 1)
        {
            Missed(false, nearestNoteGameScript, gameObject.name);
        }

        nearestNoteGameScript.hitAmount++;

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteGameScript.canGetNote)
        {
            return;
        }

        gm.scoreIncreased = true;

        nearestNoteGameScript.canGetNote = false;
        //Debug.Log("12");
        CheckHitAccuracy();

    }
    private void CheckForNoteAndLaunchEffect()
    {
        //Debug.Log("a");
        // if the nearest not has already been hit once, miss the player for attempt
        // OR if the movement that was made was made without the shield on
        if (!nearestNoteGame || !validMovement)
        {
            return;
        }

        if (nearestNoteGameScript.hitAmount > 1 && !nearestNoteGameScript.missed && nearestNoteGameScript.noteType != "note" && nearestNoteGameScript.noteDir != "up")
        {
            Missed(false, nearestNoteGameScript, gameObject.name);
        }

        // If the player has already got score for the nearest note, do not allow the note give score.
        if (!nearestNoteGameScript.canGetNote)
        {
            return;
        }

        // If the nearest note is an up arrow, ignore it. 
        if (nearestNoteGameScript.noteDir == "up")
        {
            return;
        }

        gm.scoreIncreased = true;

        nearestNoteGameScript.canGetNote = false;

        nearestNoteGameScript.hitAmount++;

        // In the case of a note with a left arrow
        if (nearestNoteGameScript.noteDir == "left" && nearestNoteGameScript.noteType != "launch" && nearestNoteGameScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteGameScript.laneNumber == oldNearestLaneNumber - 1 && nearestNoteGameScript.laneNumber == nearestLaneNumber)
            {
                //Debug.Log("1");
                //Debug.Log("oldNearestLaneNumber " + oldNearestLaneNumber);
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false, nearestNoteGameScript, gameObject.name);
                return;
            }
        }
        // In the case of a note with a right arrow
        else if (nearestNoteGameScript.noteDir == "right" && nearestNoteGameScript.noteType != "launch" && nearestNoteGameScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteGameScript.laneNumber == oldNearestLaneNumber + 1 && nearestNoteGameScript.laneNumber == nearestLaneNumber)
            {
                //Debug.Log("2");
                //Debug.Log("oldNearestLaneNumber " + oldNearestLaneNumber);
                //CheckHitAccuracy();
                CheckHitAccuracy();
            }
            else
            {
                Missed(false, nearestNoteGameScript, gameObject.name);
                return;
            }
        }

        // In the case of a launch with a right arrow
        else if (nearestNoteGameScript.noteType == "launch" && nearestNoteGameScript.noteDir == "right" && nearestNoteGameScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue
            if (nearestNoteGameScript.laneNumber == oldNearestLaneNumber + 1 && nearestNoteGameScript.laneNumber == nearestLaneNumber)
            {
                hitRLaunch = true;
                CheckHitAccuracy();
                //CheckHitAccuracy();
                //Debug.Log("15");
            }
            else
            {
                Missed(false, nearestNoteGameScript, gameObject.name);
                Debug.Log("8");
                return;
            }
        }

        // in the case of a launch with a left arrow
        else if (nearestNoteGameScript.noteType == "launch" && nearestNoteGameScript.noteDir == "left" && nearestNoteGameScript.noteType != "blast")
        {
            // If the player moved into the correct lane, continue 
            if (nearestNoteGameScript.laneNumber == oldNearestLaneNumber - 1 && nearestNoteGameScript.laneNumber == nearestLaneNumber)
            {
                hitLLaunch = true;
                //CheckHitAccuracy();
                CheckHitAccuracy();
                //Debug.Log("15");
            }
            else
            {
                Missed(false, nearestNoteGameScript, gameObject.name);
                Debug.Log("9");
                return;
            }
        }
    }
    public void DoNoteEffectUp(Note noteScript)
    {
        // If there is no note active in the game, stop
        if (!nearestNote)
        {
            return;
        }

        // If the up arrow that is nearest has already given score, stop
        if (noteScript.doneUpArrow)
        {
            return;
        }

        // If the nearest up note has not been hit yet, and the player is shielding
        if (noteScript.noteDir == "up" && isShielding && noteScript.noteType == "note")
        {
            // If the note is behind the player
            if (noteScript.gameObject.transform.position.z < transform.position.z && !noteScript.missed && noteScript.laneNumber == nearestLaneNumber)
            {
                // Tell the note that it has just given score, so it knows not to allow it again in the future
                noteScript.doneUpArrow = true;

                // Give 'perfect' score
                HitPerfect();
                //Debug.Log("a");

                // Increase the hit amounts of the up note by 1
                noteScript.hitAmount++;

                // Make it so the up arrow can not give score again once done once aready
                noteScript.canGetNote = false;
            }
        }
    }

    void CheckHitAccuracy()
    {
        //Debug.Log("1");
        float difference = Mathf.Abs(nearestNoteGameScript.beatWaitCur + tc.selectedMap.noteTimeTaken - hitLocationInBeats);
        //Debug.Log("difference " + difference + " beatWaitCur " + nearestNoteGameScript.beatWaitCur + " hitLocationInBeats " + hitLocationInBeats);
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
            Missed(false, nearestNoteGameScript, gameObject.name);
        }
    }
    IEnumerator DoShieldBlock()
    {
        if (tc.selectedMap.title != "Tutorial")
        {
            am.PlaySound("Shield_Block");

            shieldAnimator.SetBool("Idle", false);
            shieldAnimator.SetTrigger("Block");

            yield return new WaitForSeconds(.2f);

            shieldAnimator.SetBool("Idle", true);

            // Sometimes the visual for shield stays on after blocking, even when not shielding anymore, this stops it
            if (!isShielding && shield.activeSelf)
            {
                shield.SetActive(false);
            }
        }
    }

    public IEnumerator FixVisibility()
    {
        shield.GetComponent<MeshRenderer>().enabled = false;
        animator.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        animator.SetBool("isIdle", false);
        animator.SetTrigger("isMovingLeft");

        yield return new WaitForSeconds(.25f);

        animator.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        shield.GetComponent<MeshRenderer>().enabled = true;
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

        if (diminishAccuracyUI != null)
        {
            StopCoroutine(diminishAccuracyUI);
        }

        // Began the cooldown till the acuracy ui text vanishes
        diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        if (nearestNoteScript.noteType == "blast")
        {
            if (blockCoroutine != null)
            {
                StopCoroutine(blockCoroutine);
            }

            blockCoroutine = StartCoroutine(DoShieldBlock());
        }

        validMovement = false;

        if (gm.lm.activeCoroutine != null)
        {
            StopCoroutine(gm.lm.activeCoroutine);
        }

        gm.lm.activeCoroutine = StartCoroutine(gm.lm.JetLights(nearestNoteGameScript.noteType, nearestNoteGameScript.noteDir));
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

        if (diminishAccuracyUI != null)
        {
            StopCoroutine(diminishAccuracyUI);
        }

        // Began the cooldown till the acuracy ui text vanishes
        diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        if (nearestNoteScript.noteType == "blast")
        {
            if (blockCoroutine != null)
            {
                StopCoroutine(blockCoroutine);
            }
            blockCoroutine = StartCoroutine(DoShieldBlock());
        }

        validMovement = false;

        if (gm.lm.activeCoroutine != null)
        {
            StopCoroutine(gm.lm.activeCoroutine);
        }

        gm.lm.activeCoroutine = StartCoroutine(gm.lm.JetLights(nearestNoteGameScript.noteType, nearestNoteGameScript.noteDir));
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

        if (diminishAccuracyUI != null)
        {
            StopCoroutine(diminishAccuracyUI);
        }

        // Began the cooldown till the acuracy ui text vanishes
        diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

        // Make not invisible
        MakeNoteInvisible();

        // Update total accuracy UI
        gm.UpdateTotalAccuracy();

        if (nearestNoteScript.noteType == "blast")
        {
            if (blockCoroutine != null)
            {
                StopCoroutine(blockCoroutine);
            }
            blockCoroutine = StartCoroutine(DoShieldBlock());
        }
        validMovement = false;
        //Debug.Break();

        if (gm.lm.activeCoroutine != null)
        {
            StopCoroutine(gm.lm.activeCoroutine);
        }

        gm.lm.activeCoroutine = StartCoroutine(gm.lm.JetLights(nearestNoteGameScript.noteType, nearestNoteGameScript.noteDir));
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
    public void Missed(bool hitByBomb, Note noteScript, string name)
    {
        if (nearestAnyNote == null)
        {
            return;
        }

        if (gm.comboMulti >= 2)
        {
            animator.SetTrigger("ComboReset");
            am.PlaySound("PlayerHurt");
        }

        int index = noteScript.gameObject.transform.GetSiblingIndex();

        if (!hitByBomb && noteScript.noteType != "slider")
        {
            //Debug.Log("b");
            noteScript.canGetNote = false;
            noteScript.missed = true;
            //Debug.Log(noteScript.missed + " " + noteScript.gameObject.name);
            //Debug.Break();
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

            if (diminishAccuracyUI != null)
            {
                StopCoroutine(diminishAccuracyUI);
            }

            // Began the cooldown till the acuracy ui text vanishes
            diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

            gm.comboMulti = 1;
            gm.updateGameUI();

            if (gm.reposition)
            {
                if (tc.notesObj.transform.childCount >= 2)
                {
                    if (tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>().noteDir != "down")
                    {
                        StartCoroutine("RepositionMod", tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>());
                    }
                }

            }

            return;
        }

        if (!hitByBomb && noteScript.noteType == "slider")
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
                    noteScript.missed = true;
                    nearestSliderStartEnd.GetComponent<Note>().sliderLr.gameObject.GetComponent<Slider>().Missed();
                    gm.UpdateHealth(gm.regenSlider);    
                    gm.scoreIncreased = true;

                    gm.misses++;

                    gm.score += gm.missScore;

                    //Update player accuracy UI
                    accuracyUI.text = gm.missScoreName;

                    // Update total accuracy UI
                    gm.UpdateTotalAccuracy();

                    if (diminishAccuracyUI != null)
                    {
                        StopCoroutine(diminishAccuracyUI);
                    }

                    // Began the cooldown till the acuracy ui text vanishes
                    diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

                    gm.comboMulti = 1;
                    gm.updateGameUI();

                    if (gm.reposition)
                    {
                        if (tc.notesObj.transform.childCount >= 2)
                        {
                            if (tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>().noteDir != "down")
                            {
                                StartCoroutine("RepositionMod", tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>());
                            }
                        }
                    }

                    return;
                }
            }
            else if (nearestNoteGame)
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

                if (diminishAccuracyUI != null)
                {
                    StopCoroutine(diminishAccuracyUI);
                }

                // Began the cooldown till the acuracy ui text vanishes
                diminishAccuracyUI = StartCoroutine("DiminishAccuracyUI");

                gm.comboMulti = 1;
                gm.updateGameUI();

                if (gm.reposition)
                {
                    if (tc.notesObj.transform.childCount >= 2)
                    {
                        if (tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>().noteDir != "down")
                        {
                            StartCoroutine("RepositionMod", tc.notesObj.transform.GetChild(index + 1).GetComponent<Note>());
                        }
                    }
                }

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
            playerPos.x = 1.5f * (nearestNoteGameScript.laneNumber - 2);
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }

        else if (hitRLaunch)
        {
            hitRLaunch = false;
            playerPos.x = 1.5f * (nearestNoteGameScript.laneNumber);
            transform.position = playerPos;

            pm.FindNearestPath(false);
        }
    }

    IEnumerator DiminishAccuracyUI()
    {
        yield return new WaitForSeconds(.35f);
        accuracyUI.text = "";   
    }
}