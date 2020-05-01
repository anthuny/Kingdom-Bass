using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
    private PathManager pm;
    private AudioManager am;
    private Jet jScript;

    [Header("Map Selection")]
    public bool mapHasBeenSelected;
    //[HideInInspector]
    public Map selectedMap;

    [Header("Map Names")]
    public string map1;
    public string map2;

    [Header("Tutorial")]
    public bool increasedStage;
    public Coroutine activeCoroutine;


    public string[] laneCodes = new string[] { "lane1Code", "lane2Code", "lane3Code", "lane4Code", "lane5Code", "lane6Code", "lane7Code", "lane8Code" };

    // Lane codes
   // public string lane1Code, lane2Code, lane3Code, lane4Code, lane5Code;

    // Note code
    public string noteCode;

    public float trackEndWait;

    public List<float> noteLanes = new List<float>();
    public List<GameObject> notes = new List<GameObject>();
    public List<GameObject> allNotes = new List<GameObject>();
    public List<string> allNoteTypes = new List<string>();
    public List<float> beatWaitCount = new List<float>();
    public List<float> beatWaitCountAccum = new List<float>();
    public List<float> trackPosIntervalsList2 = new List<float>();
    public List<float> trackPosIntervalsList3 = new List<float>();

    public float trackPosNumber;
    public float trackPosNumber2;

    public GameObject notesObj;
    public GameObject noteVisual;

    //public float trackBpm;
    public float noteOffSet;

    public string noteType1Code;
    public string noteType2Code;

    public float secPerBeat;
    public float trackPos;
    public float trackPosInBeats;
    public float trackPosInBeatsGame;
    public float dspTrackTime;
    //public AudioSource audioSource;
    public float previousNoteBeatTime;
    public float previousNoteBeatTime2;
    public float previousNoteBeatTime3;
    public float nextNoteInBeats;
    public int notesSpawned;

    public bool trackInProgress;

    [HideInInspector]
    public float lastBeat;

    //[HideInInspector]
    [Tooltip("This number determines what note the track has most recently spawned")]
    public int curNoteCount = 0;
    [HideInInspector]
    public float nextIndex2 = 0;
    //[HideInInspector]
    public int nextIndex3 = 0;
    public int beatWaitNextNote = 0;

    public float newStartingNoteAccum = 0;
    public float oldNewStartingNoteAccum = 0;
    public float noteTempNum = 0;

    public float beatWaitAccum = 0;
    public float newStartingInt = 0;

    [Tooltip("Amount of beats that must play before the first note spawns")]
    [Range(1, 25)]
    public int beatsBeforeStart;

    private float noteTimeTaken;

    Player player;

    XmlDocument mapLD;

    [HideInInspector]
    public float nextBeat;
    [HideInInspector]
    public float trackPosIntervals;
    [HideInInspector]
    public float trackPosIntervals2;
    [HideInInspector]
    public float trackPosIntervals3;
    [HideInInspector]
    public float pointToNextBeat, pointToNextBeat2;
    [HideInInspector]
    public float firstNote;

    [HideInInspector]
    public float pointFromLastBeatInstant, pointFromLastBeatWait;

    [HideInInspector]
    public float firstInterval;

    [HideInInspector]
    public bool deadNoteAssigned;

    [HideInInspector]
    public bool doneOnce = false;
    private bool doneOnce2;
    private Transform noteInfront;

    public bool searchingNotes;
    public bool doneOnce3 = false;

    private Vector3 pos;

    public Path path;

    public bool loadingTrack;

    public int index = 1;

    public bool movedUI;

    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        am = FindObjectOfType<AudioManager>();
        jScript = gm.jet.GetComponent<Jet>();
    }

    private void Update()
    {
        if (gm.tutPaused)
        {
            return;
        }

        QueueNoteSpawns();
        ChangeTutorialStage();
    }

    public void AssignNotes(string noteType, string laneNumber, string arrowD, string beatWait)
    {
        //Debug.Break();
        // Checks for notes in each lane, If it belongs in a lane, make it belong there 
        for (int i = 1; i <= pm.maxLanes; i++)
        {
            //Debug.Log("laneNumber " + laneNumber);
            //Debug.Log("beatWait " + beatWait);
            //Debug.Break();


            if (laneNumber.Substring(0, 1).Contains(i.ToString()))
            {
                // Spawn noteVisual, at notes position
                GameObject go = Instantiate(noteVisual, notesObj.transform.position, Quaternion.identity);
                Note noteScript = go.GetComponent<Note>();

                // If the note has another note in the same beat wait to spawn
                for (int y = 0; y < laneNumber.Length - 1; y++)
                {
                    if (laneNumber.Length > 1)
                    {
                        string secondChar = laneNumber.Substring(y + 1, 1);

                        noteScript.nextBombLane.Add(int.Parse(secondChar));
                    }
                    else
                    {
                        break;
                    }
                }

                // Set the name of the note to the lane it is in
                go.name = i.ToString();

                // Parent this note to the notes object 
                go.transform.SetParent(notesObj.transform);

                // Set the note type and arrow direction
                noteScript.noteType = noteType;
                noteScript.noteDir = arrowD;

                // Set the amount of beatWaits the note is to perform
                noteScript.beatWait = float.Parse(beatWait);

                // Set the lane number of the note 
                noteScript.laneNumber = i;

                // Disable the note
                go.SetActive(false);

                // Add the note to allNotes list | This is needed for the dynamic spawning times if using it.
                //if (noteType != "slider")
                //{
                noteLanes.Add(i);
                //Debug.Log(noteLanes.Count);

                // add notes to the all notes list
                allNotes.Add(go);
                //}

                if (noteType != "bomb")
                {
                    // Add the note to the notes non-bomb note list
                    notes.Add(go);
                    gm.notesLeftInfront++;
                }

                // Add the beatWait wait for each note to beatWaitcount list
                beatWaitCount.Add(noteScript.beatWait);
                beatWaitAccum += beatWaitCount[beatWaitCount.Count - 1];

                // Add the beatwaitcount to a list that accumilates for each element
                beatWaitCountAccum.Add(beatWaitAccum);

                noteScript.beatWaitCur = beatWaitCountAccum[beatWaitCountAccum.Count - 1];

                // Set this note's beatWaitCur to equal the beatWaitAccum when it spawns
                //go.GetComponent<Note>().beatWaitCur = beatWaitCountAccum[beatWaitCountAccum.Count - 1] - beatWaitCount[0];

                allNoteTypes.Add(noteType);

                // move the note to the correct lane
                go.transform.position = new Vector3(1.5f * (int.Parse(laneNumber) - 1), 0.02f, 70);
            }
        }

        // Ensure that the ui flip to in-game mode only happens once, rather then for every note in the song
        if (!mapHasBeenSelected)
        {

            UpdateMapSelectionUI();

            player.oldNearestLaneNumber = player.nearestLaneNumber;
        }

        if (selectedMap.title != "Tutorial" && !gm.jet.activeSelf)
        {
            gm.jScript.EnableJet();
        }
    }

    // After notes have been assigned, call this
    void UpdateMapSelectionUI()
    {
        // Now that all the notes have loaded, allow the player to start
        mapHasBeenSelected = true;

        trackInProgress = true;

        StartCoroutine("StartSong");
    }

    // This is called after UpdateMapSelectionUI is run
    IEnumerator StartSong()
    {
        yield return new WaitForSeconds(selectedMap.timeBeforeStart);

        // Incase the audio Listener was paused, activate it now
        AudioListener.pause = false;

        // Start the track timer?
        dspTrackTime = (float)AudioSettings.dspTime;

        if (selectedMap.title == "Tutorial")
        {
            UpdateTutorialSlides();        

            // Disable game UI
            gm.gameUI.SetActive(false);

            am.PlaySound(selectedMap.title);

            // Allow the play to pause
            gm.cantPause = false;
        }
        else
        {
            am.PlaySound(selectedMap.title);

            // Display all game UI
            gm.gameUI.SetActive(true);
            gm.updateGameUI();
        }

        gm.StartGame();

        // Get access to the active track playing
        foreach (AudioSource aSource in am.gameObject.GetComponents<AudioSource>())
        {
            if (aSource.clip.name == selectedMap.title)
            {
                if (aSource.isPlaying)
                {
                    gm.activeTrack = aSource;
                }
            }
        }
    }

    IEnumerator activateAnimators(Animator animatorKB, Animator animatorCT)
    {
        if (gm.tutorialStage == 1)
        {
            yield return new WaitForSeconds(8.5f);

            gm.blur.SetActive(true);

            gm.tutorialUI.SetActive(true);

            // Enable / Disable entire parents 
            gm.keyboardUI.SetActive(true);
            gm.controllerUI.SetActive(true);

            gm.key.GetComponent<RectTransform>().sizeDelta = new Vector2(606.8f, 606.8f);

            gm.controllerImageDouble.SetActive(false);
            gm.controllerImageAnimator.SetBool("Double", false);


            yield return new WaitForSeconds(1);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.leftArrowNote;
            gm.key.GetComponent<Image>().sprite = gm.aKeyImageSprite;

            // Change the colour of note
            gm.noteKB.GetComponent<Image>().color = gm.horizontalNoteArrowC;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.leftArrowNote;
            gm.controllerImage.GetComponent<Image>().sprite = gm.leftControllerImageSprite;

            // Change the colour of note
            gm.noteCT.GetComponent<Image>().color = gm.horizontalNoteArrowC;

            // Enable the correct arrow UI for controller UI
            gm.leftArrowCT.SetActive(true);
            gm.rightArrowCT.SetActive(false);

            gm.tiltText.GetComponent<Animator>().SetTrigger("GoLeft");

            // Reposition the UI
            animatorKB.SetBool("Side", false);
            animatorCT.SetBool("Side", false);

            gm.tutTextAnimator.SetBool("Side", false);

            gm.noteKBAnimator.SetBool("Double", false);
            if (gm.noteDoubleKB.activeSelf)
            {
                gm.noteDoubleKBAnimator.SetBool("Double", false);
            }

            gm.keyAnimator.SetBool("Double", false);

            if (gm.keyDouble.activeSelf)
            {
                gm.keyDoubleAnimator.SetBool("Double", false);
            }

            yield return new WaitForSeconds(4.5f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 2)
        {
            yield return new WaitForSeconds(2.2f);

            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.RightArrowNote;
            gm.key.GetComponent<Image>().sprite = gm.lKeyImageSprite;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.RightArrowNote;
            gm.controllerImage.GetComponent<Image>().sprite = gm.rightControllerImageSprite;

            // Enable the correct arrow UI for controller UI
            gm.leftArrowCT.SetActive(false);
            gm.rightArrowCT.SetActive(true);

            // Reposition the UI
            gm.tiltText.GetComponent<Animator>().SetTrigger("GoRight");

            yield return new WaitForSeconds(5.8f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 3)
        {
            yield return new WaitForSeconds(1.3f);

            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.leftArrowNote;
            gm.noteDoubleKB.GetComponent<Image>().sprite = gm.RightArrowNote;
            gm.noteDoubleCT.GetComponent<Image>().sprite = gm.RightArrowNote;
            gm.key.GetComponent<Image>().sprite = gm.aKeyImageSprite;

            gm.noteDoubleKB.GetComponent<Image>().color = gm.horizontalNoteArrowC;
            gm.noteDoubleCT.GetComponent<Image>().color = gm.horizontalNoteArrowC;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.leftArrowNote;
            gm.controllerImage.GetComponent<Image>().sprite = gm.middleControllerImageSprite;

            // Enable the correct arrow UI for keyboard UI
            gm.noteKB.SetActive(true);
            gm.key.SetActive(true);

            gm.noteDoubleKB.SetActive(true);
            gm.keyDouble.SetActive(true);

            // Enable the correct arrow UI for controller UI
            gm.leftArrowCT.SetActive(true);
            gm.rightArrowCT.SetActive(true);

            gm.noteDoubleCT.SetActive(true);

            // Reposition the keyboard UI
            gm.noteKBAnimator.SetBool("Double", true);
            gm.noteDoubleKBAnimator.SetBool("Double", true);

            gm.keyAnimator.SetBool("Double", true);
            gm.keyDoubleAnimator.SetBool("Double", true);

            gm.tiltText.GetComponent<Animator>().SetTrigger("GoMiddle");

            // Reposition the controller UI
            gm.noteCTAnimator.SetBool("Double", true);
            gm.noteDoubleCTAnimator.SetBool("Double", true);

            gm.tiltText.GetComponent<Animator>().SetTrigger("GoMiddle");

            yield return new WaitForSeconds(7.3f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 4)
        {
            yield return new WaitForSeconds(1.7f);

            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.leftLaunchNote;
            gm.noteDoubleKB.GetComponent<Image>().sprite = gm.rightLaunchNote;

            // Change the colour of note
            gm.noteKB.GetComponent<Image>().color = gm.horizontalLaunchArrowC;
            gm.noteDoubleKB.GetComponent<Image>().color = gm.horizontalLaunchArrowC;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.leftLaunchNote;
            gm.noteDoubleCT.GetComponent<Image>().sprite = gm.rightLaunchNote;

            // Change the colour of note
            gm.noteCT.GetComponent<Image>().color = gm.horizontalLaunchArrowC;
            gm.noteDoubleCT.GetComponent<Image>().color = gm.horizontalLaunchArrowC;

            // Reposition the UI
            gm.noteKBAnimator.SetBool("Double", true);
            gm.noteDoubleKBAnimator.SetBool("Double", true);

            gm.keyAnimator.SetBool("Double", true);
            gm.keyDoubleAnimator.SetBool("Double", true);

            gm.tiltText.GetComponent<Animator>().SetTrigger("GoMiddle");

            yield return new WaitForSeconds(15.3f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 5)
        {
            yield return new WaitForSeconds(1.8f);

            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.upArrow;
            gm.key.GetComponent<Image>().sprite = gm.tutStageTarget;

            // Change the colour of note
            gm.noteKB.GetComponent<Image>().color = gm.upArrowC;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.upArrow;
            gm.controllerImage.GetComponent<Image>().sprite = gm.tutStageTarget;

            // Change the colour of note
            gm.noteCT.GetComponent<Image>().color = gm.upArrowC;

            gm.noteDoubleKB.SetActive(false);

            gm.keyDouble.SetActive(false);

            gm.noteDoubleCT.SetActive(false);

            gm.leftArrowCT.SetActive(false);
            gm.rightArrowCT.SetActive(false);

            gm.key.transform.GetChild(0).gameObject.SetActive(false);

            gm.tiltText.gameObject.SetActive(false);

            // Reposition the UI
            gm.noteKBAnimator.SetBool("Double", false);
            gm.keyAnimator.SetBool("Double", false);

            gm.noteCTAnimator.SetBool("Double", false);
  

            yield return new WaitForSeconds(8.2f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 6)
        {
            yield return new WaitForSeconds(1.2f);

            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.tutStageBlastPlayer;
            gm.key.GetComponent<Image>().sprite = gm.spacePressed;

            // Change the colour of note
            gm.noteKB.GetComponent<Image>().color = Color.white;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.tutStageBlastPlayer;
            gm.controllerImage.GetComponent<Image>().sprite = gm.tutStageBlastContr;

            // Change the colour of note
            gm.noteCT.GetComponent<Image>().color = Color.white;

            // Enable / Disable the correct Keyboard UI

            gm.pressBothText.SetActive(true);       

            // Reposition the Keyboard UI
            gm.noteKBAnimator.SetBool("Double", false);
            gm.keyAnimator.SetBool("Double", false);

            // Reposition the Controller UI
            gm.noteCTAnimator.SetBool("Double", false);

            gm.key.GetComponent<RectTransform>().sizeDelta = new Vector2(606.8f, 290);

            yield return new WaitForSeconds(6.8f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 7)
        {
            gm.blur.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.bombIcon;
            gm.key.GetComponent<Image>().sprite = gm.cross;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.bombIcon;
            gm.controllerImage.GetComponent<Image>().sprite = gm.cross;

            gm.key.GetComponent<RectTransform>().sizeDelta = new Vector2(606.8f, 606.8f);

            // Enable / Disable entire parents 
            //gm.keyboardUI.SetActive(true);
            //gm.controllerUI.SetActive(true);
            //gm.tutAreaTextBG.gameObject.SetActive(true);

            gm.pressBothText.SetActive(false);

            animatorCT.SetBool("Side", false);
            animatorKB.SetBool("Side", false);
            gm.tutTextAnimator.SetBool("Side", false);

            yield return new WaitForSeconds(8.5f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 8)
        {
            yield return new WaitForSeconds(1.1f);

            gm.blur.SetActive(true);

            // Enable / Disable entire parents 
            gm.keyboardUI.SetActive(true);
            gm.controllerUI.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.slider;
            gm.key.GetComponent<Image>().sprite = gm.spaceHeld;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.slider;
            gm.controllerImage.GetComponent<Image>().sprite = gm.middleControllerImageSprite;
            gm.controllerImageDouble.GetComponent<Image>().sprite = gm.shieldControllerImageSprite;
            
            gm.controllerImageDouble.SetActive(true);
            gm.tiltText.gameObject.SetActive(true);
            gm.controllerImageDouble.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Hold";

            gm.key.GetComponent<RectTransform>().sizeDelta = new Vector2(606.8f, 290);

            gm.leftArrowCT.SetActive(true);
            gm.rightArrowCT.SetActive(true);

            gm.tutAreaTextBG.gameObject.SetActive(true);

            gm.pressBothText.SetActive(false);

            gm.tutTextAnimator.SetBool("Side", false);

            animatorCT.SetBool("Side", false);
            animatorKB.SetBool("Side", false);

            gm.controllerImageAnimator.SetBool("Double", true); 
            gm.keyAnimator.SetBool("Double", false);

            gm.key.transform.GetChild(1).GetComponent<Text>().text = "Hold";
            gm.keyDouble.transform.GetChild(1).GetComponent<Text>().text = "Hold";

            yield return new WaitForSeconds(23.9f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield break;
        }

        else if (gm.tutorialStage == 9)
        {
            yield return new WaitForSeconds(1.1f);

            gm.blur.SetActive(true);

            // Enable / Disable entire parents 
            gm.keyboardUI.SetActive(true);
            gm.controllerUI.SetActive(true);

            // Change the note / key sprite for keyboard ui
            gm.noteKB.GetComponent<Image>().sprite = gm.noShieldPlayer;
            gm.key.GetComponent<Image>().sprite = gm.spaceHeld;

            // Change the note / controller sprite for controller ui
            gm.noteCT.GetComponent<Image>().sprite = gm.noteKB.GetComponent<Image>().sprite = gm.noShieldPlayer;
            gm.controllerImage.GetComponent<Image>().sprite = gm.middleControllerImageSprite;
            gm.controllerImageDouble.GetComponent<Image>().sprite = gm.shieldControllerImageSprite;

            gm.controllerImageDouble.SetActive(true);
            gm.tiltText.gameObject.SetActive(true);
            gm.controllerImageDouble.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Hold";

            gm.key.GetComponent<RectTransform>().sizeDelta = new Vector2(606.8f, 290);

            gm.leftArrowCT.SetActive(true);
            gm.rightArrowCT.SetActive(true);

            gm.tutAreaTextBG.gameObject.SetActive(true);

            gm.pressBothText.SetActive(false);

            gm.tutTextAnimator.SetBool("Side", false);

            animatorCT.SetBool("Side", false);
            animatorKB.SetBool("Side", false);

            gm.controllerImageAnimator.SetBool("Double", true);
            gm.keyAnimator.SetBool("Double", false);

            gm.key.transform.GetChild(1).GetComponent<Text>().text = "Hold";
            gm.keyDouble.transform.GetChild(1).GetComponent<Text>().text = "Hold";

            yield return new WaitForSeconds(25.9f);

            gm.blur.SetActive(false);

            // Reposition the UI
            animatorKB.SetBool("Side", true);
            animatorCT.SetBool("Side", true);

            gm.tutTextAnimator.SetBool("Side", true);

            yield return new WaitForSeconds(20);

            gm.noFail = false;

            yield break;
        }
    }

    // This is called during StartSong if tutorial is on
    void UpdateTutorialSlides()
    {
        increasedStage = true;
        gm.tutorialStage++;

        //Debug.Log("increasing stage" + gm.tutorialStage + " " +  gm.doneTutStageCount);

        // Change the text on the tutorial text to be the first slide of information
        if (gm.tutorialStage <= gm.maxTutorialStages)
        {
            gm.tutAreaInfo = gm.tutTexts[gm.tutorialStage - 1];

            // Left regular arrow note
            #region Tutorial Stage 1
            if (gm.tutorialStage == 1 && gm.doneTutStageCount == 0)
            {
                gm.doneTutStageCount++;

                gm.reposition = true;

                gm.noteDoubleKB.SetActive(false);
                gm.keyDouble.SetActive(false);
                gm.noteDoubleCT.SetActive(false);
                gm.pressBothText.SetActive(false);

                am.PlaySound("Tut_Stage_1");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Right regular arrow note
            #region Tutorial Stage 2
            if (gm.tutorialStage == 2 && gm.doneTutStageCount == 1)
            {
                gm.doneTutStageCount++;

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_2");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Combine left/right regular arrow note
            #region Tutorial Stage 3   
            if (gm.tutorialStage == 3 & gm.doneTutStageCount == 2)
            {
                gm.doneTutStageCount++;

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_3");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Launch Notes
            #region Tutorial Stage 4
            if (gm.tutorialStage == 4 && gm.doneTutStageCount == 3)
            {
                gm.doneTutStageCount++;

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_4");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Up arrow notes
            #region Tutorial Stage 5
            if (gm.tutorialStage == 5 && gm.doneTutStageCount == 4)
            {
                gm.doneTutStageCount++;

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_5");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Blast notes
            #region Tutorial Stage 6
            {
                if (gm.tutorialStage == 6 && gm.doneTutStageCount == 5)
                {
                    gm.doneTutStageCount++;

                    // Reposition the UI
                    gm.keyboardUIAnimator.SetBool("Side", false);
                    gm.controllerUIAnimator.SetBool("Side", false);

                    gm.tutTextAnimator.SetBool("Side", false);

                    am.PlaySound("Tut_Stage_6");

                    activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
                }
            }
            
            #endregion
            // Bomb notes
            #region Tutorial Stage 7
            if (gm.tutorialStage == 7 && gm.doneTutStageCount == 6)
            {
                gm.doneTutStageCount++;

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                // Enable / Disable entire parents 
                //gm.keyboardUI.SetActive(false);
                //gm.controllerUI.SetActive(false);
                //gm.tutAreaTextBG.gameObject.SetActive(false);

                am.PlaySound("Tut_Stage_7");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Slider
            #region Tutorial Stage 8
            if (gm.tutorialStage == 8 && gm.doneTutStageCount == 7)
            {
                gm.doneTutStageCount++;

                // Enable / Disable entire parents 
                gm.keyboardUI.SetActive(true);
                gm.controllerUI.SetActive(true);
                gm.tutAreaTextBG.gameObject.SetActive(true);

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_8");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
            // Shield
            #region Tutorial Stage 9
            if (gm.tutorialStage == 9 && gm.doneTutStageCount == 8)
            {
                gm.doneTutStageCount++;

                // Enable / Disable entire parents 
                gm.keyboardUI.SetActive(true);
                gm.controllerUI.SetActive(true);
                gm.tutAreaTextBG.gameObject.SetActive(true);

                // Reposition the UI
                gm.keyboardUIAnimator.SetBool("Side", false);
                gm.controllerUIAnimator.SetBool("Side", false);

                gm.tutTextAnimator.SetBool("Side", false);

                am.PlaySound("Tut_Stage_9");

                activeCoroutine = StartCoroutine(activateAnimators(gm.keyboardUIAnimator, gm.controllerUIAnimator));
            }
            #endregion
        }

        // Assign the correct text for the text
        gm.tutAreaText.text = gm.tutAreaInfo;
    }

    // Called every update
    void ChangeTutorialStage()
    {
        if (gm.tutorialStage < 1)
        {
            return;
        }

        if (gm.tutorialStage > gm.maxTutorialStages)
        {
            gm.tutorialStage = 0;
            return;
        }

        // Only allow an increase in stage when the next stage happens
        if (gm.tutorialStage > 0)
        {
            if (trackPosInBeatsGame > gm.nextStageThreshholdBeats[gm.tutorialStage - 1])
            {
                increasedStage = false;
                //gm.PauseGame(true);
                player.RepositionPlayer();
            }
        }

        if (gm.tutorialStage >= 1 && gm.tutorialStage != gm.maxTutorialStages + 1 && trackPosInBeatsGame > gm.nextStageThreshholdBeats[gm.tutorialStage - 1] && !increasedStage)
        {
            UpdateTutorialSlides();
        }
    }

    void SpawnNotes()
    {
        // Declare what the current segment is, if it hasn't been done already.
        if (!pm.currentSegment)
        {
            //Find the path the player is on
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
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

        pm.currentSegment = pm.nearestPath.transform.parent.gameObject;
        path = pm.initialPath.GetComponent<Path>();

        //Assign the notes
        for (int i = 0; i < noteLanes.Count; i++)
        {
            // If the note is not active, set it active
            if (!notesObj.transform.GetChild(i).gameObject.activeSelf)
            {
                notesObj.transform.GetChild(i).gameObject.SetActive(true);
                for (int z = 0; z < pm.maxLanes; z++)
                {
                    // Find the note's name that equals the correct code
                    if (notesObj.transform.GetChild(i).name.Contains(laneCodes[z]))
                    {
                        // Loop through all paths in the current segment
                        for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                        {
                            // If the lane number equals the correct one
                            if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(laneCodes[z]))
                            {
                                // move the note to the correct lane
                                notesObj.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, path.pathLength);

                                // Set the starting position variable for the note to it's position after it has been moved to starting position
                                notesObj.transform.GetChild(i).GetComponent<Note>().startingPos.z = notesObj.transform.GetChild(i).GetComponent<Note>().transform.position.z;

                                // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                                notesObj.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                                // Allow the note to move when ready
                                notesObj.transform.GetChild(i).GetComponent<Note>().canMove = true;

                                // Set the accumulative beat wait of what each note is supposed to be at, for information
                                //beatWaitAccum += notesObj.transform.GetChild(i).GetComponent<Note>().beatWait;
                            }
                        }
                    }
                }
                return;
            }
        }
    }

    void QueueNoteSpawns()
    {
        if (selectedMap & !gm.mapSelectionUI.activeSelf)
        {
            foreach (AudioSource aSource in am.gameObject.GetComponents<AudioSource>())
            {
                if (aSource.clip.name == selectedMap.title)
                {
                    if (!aSource.isPlaying)
                    {
                        return;
                    }
                }
            }
        }

        // Keep track of the track's position in seconds from when it started
        trackPos = (float)(AudioSettings.dspTime - dspTrackTime);

        // Determine how many beats since the track started
        trackPosInBeats = (trackPos / secPerBeat);

        // Determine how many beats since the game started
        trackPosInBeatsGame = trackPosInBeats - beatsBeforeStart + 1;

        if (curNoteCount >= noteLanes.Count)
        {
            return;
        }

        // Spawn a note
        if (trackPos > (lastBeat + ((beatsBeforeStart - 1) * secPerBeat)) + (secPerBeat * beatWaitCount[curNoteCount]))
        {
            SpawnNotes();

            lastBeat += secPerBeat * beatWaitCount[curNoteCount];

            trackPosIntervals = beatWaitCount[curNoteCount];
            trackPosIntervals2 = beatWaitCount[curNoteCount];
            trackPosIntervals3 += beatWaitCount[curNoteCount];

            trackPosIntervalsList2.Add(trackPosIntervals2);
            trackPosIntervalsList3.Add(trackPosIntervals3);

            nextIndex2 = beatWaitCount[0];
            curNoteCount++;
        }

        // Index out of bounds check
        if (trackPosIntervalsList2.Count == 1)
        {
            firstInterval = trackPosIntervalsList2[0];

            // Determine what the first next note will be for score measuring
            pointToNextBeat = trackPosIntervalsList2[0] * (selectedMap.noteTimeTaken + 1);
            //firstNote = pointToNextBeat;
        }

        if (trackPosIntervalsList2.Count == 2)
        {
            pointToNextBeat2 = trackPosIntervalsList2[1] * (selectedMap.noteTimeTaken + 1);
        }
    }

    // Happens when the player selects a map
    public void SelectMap(Button button)
    {
        // Initialize what map was selected
        selectedMap = button.GetComponent<MapDetails>().map;

        gm.mapSelectText.text = "Select map again to Confirm!";

    }

    // Happens when the player presses the start button
    public void LoadTrack()
    {
        // Used to ensure the button is pressed once.
        if (!loadingTrack)
        {
            gm.cursor.SetActive(false);

            gm.blur.SetActive(false);

            loadingTrack = true;

            gm.activeScene = "Game";
            // Disable the ability to pause
            gm.cantPause = true;

            gm.shieldOffSpeed = selectedMap.sliderFastSpeed;

            // disable all buttons in map selection screen
            gm.mapSelectionUI.SetActive(false);

            gm.mapSelectText.gameObject.SetActive(false);

            // Assign map values (BPM)
            SetupTrack(selectedMap.bpm);

            mapLD = new XmlDocument();

            mapLD.LoadXml(selectedMap.mapXML.text);

            XmlNodeList notes = mapLD.SelectNodes("/Levels/Level/Notes/Note");

            foreach (XmlNode note in notes)
            {
                GetNote newGetNote = new GetNote(note);
            }
        }

    }

    // This is used because I can't link a button to call a coroutine
    public IEnumerator LoadTrackCo()
    {
        yield return new WaitForSeconds(0f);

        LoadTrack();
    }

    public void SetupTrack(int bpm)
    {
        // Calculate the number of seconds in each beat
        secPerBeat = 60f / bpm;

        StartMapSelectionMode();
    }

    void StartMapSelectionMode()
    {
        gm.mapSelectText.text = gm.selectAMapText;
    }

    // XML referencing for each note.
    class GetNote
    {
        public string noteType { get; private set; }
        public string noteLane { get; private set; }
        public string noteArrowD { get; private set; }
        public string beatWait { get; private set; }

        TrackCreator tc;

        public GetNote(XmlNode curNoteNode)
        {
            tc = FindObjectOfType<TrackCreator>();
            noteType = curNoteNode["Note_Type"].InnerText;
            noteLane = curNoteNode["Note_Lane"].InnerText;
            noteArrowD = curNoteNode["Note_ArrowD"].InnerText;
            beatWait = curNoteNode["Note_BeatWait"].InnerText;

            XmlNode NoteNode = curNoteNode.SelectSingleNode("Note_Type");
            XmlNode laneNode = curNoteNode.SelectSingleNode("Note_Lane");
            XmlNode arrowDNode = curNoteNode.SelectSingleNode("Note_ArrowD");
            XmlNode BeatWait = curNoteNode.SelectSingleNode("Note_BeatWait");

            //Debug.Break();
            tc.AssignNotes(NoteNode.InnerText, laneNode.InnerText, arrowDNode.InnerText, BeatWait.InnerText);
        }
    }
}
