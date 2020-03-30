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

    [Header("Map Selection")]
    public bool mapHasBeenSelected;
    //[HideInInspector]
    public Map selectedMap;

    [Header("Map Names")]
    public string map1;
    public string map2;

    [Header("Tutorial")]
    public bool increasedStage;


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

    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        am = FindObjectOfType<AudioManager>();
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
                if (noteType != "slider")
                {
                    noteLanes.Add(i);
                    // add notes to the all notes list
                    allNotes.Add(go);
                }

                if (noteType != "bomb")
                {
                    // Add the note to the notes non-bomb note list
                    notes.Add(go);
                }

                // Add the beatWait wait for each note to beatWaitcount list
                beatWaitCount.Add(noteScript.beatWait);
                beatWaitAccum += beatWaitCount[beatWaitCount.Count - 1];

                // Add the beatwaitcount to a list that accumilates for each element
                beatWaitCountAccum.Add(beatWaitAccum);
                    
                // Set this note's beatWaitCur to equal the beatWaitAccum when it spawns
                go.GetComponent<Note>().beatWaitCur = beatWaitCountAccum[beatWaitCountAccum.Count - 1] - beatWaitCount[0];

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
            //Debug.Log("?");
        }
    }

    // After notes have been assigned, call this
    void UpdateMapSelectionUI()
    {
        // Now that all the notes have loaded, allow the player to start
        mapHasBeenSelected = true;

        trackInProgress = true;

        gm.startBtn.SetActive(false);

        StartCoroutine("StartSong");
    }

    // This is called after UpdateMapSelectionUI is run
    IEnumerator StartSong()
    {
        yield return new WaitForSeconds(selectedMap.timeBeforeStart);

        // Incase the audio Listener was paused, activate it now
        AudioListener.pause = false;

        // Allow the play to pause
        gm.cantPause = false;

        // Start the track timer?
        dspTrackTime = (float)AudioSettings.dspTime;

        if (selectedMap.title == "Tutorial")
        {
            UpdateTutorialSlides();

            // Disable game UI
            gm.gameUI.SetActive(false);

            am.PlaySound(selectedMap.title);
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

    // This is called during StartSong if tutorial is on
    void UpdateTutorialSlides()
    {
        increasedStage = true;
        gm.tutorialStage++;

        // Change the text on the tutorial text to be the first slide of information
        if (gm.tutorialStage <= gm.maxTutorialStages - 1)
        {
            gm.tutAreaInfo = gm.tutTexts[gm.tutorialStage - 1];

            // Left regular arrow note
            #region Tutorial Stage 1
            if (gm.tutorialStage == 1)
            {
                gm.plusSymbol.gameObject.SetActive(false);

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    if (i == 0 || i == 1)
                    {
                        gm.arrowNotes[i].transform.localScale = Vector3.one * 2.1f;
                        gm.allVisuals[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.allVisuals[i].gameObject.SetActive(false);
                    }
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    if (i == 0)
                    {
                        gm.arrow[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrow[i].gameObject.SetActive(false);
                    }
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    if (i == 0)
                    {
                        //gm.allVisuals[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.leftArrowNote;
                        gm.arrowNotes[i].color = gm.horizontalNoteArrowC;
                        gm.arrowNotes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrowNotes[i].gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < gm.spaceSupportingTexts.Length; i++)
                {
                    gm.spaceSupportingTexts[i].gameObject.SetActive(false);
                }

                // Enables/Disables the 'Hold / Release' Texts
                for (int i = 0; i < gm.supportingTexts.Length; i++)
                {
                    gm.supportingTexts[i].gameObject.SetActive(true);
                }

                // Enable/Disable the space bar images
                for (int i = 0; i < gm.spaceBar.Length; i++)
                {
                    gm.spaceBar[i].gameObject.SetActive(false);
                }

                // Enables/Disables the supporting 'Hold / Release' Texts
                for (int i = 0; i < gm.spaceSupportingTexts.Length; i++)
                {
                    gm.spaceSupportingTexts[i].gameObject.SetActive(false);
                }

                gm.tutAreaTextBG.gameObject.SetActive(true);
            }
            #endregion
            // Right regular arrow note
            #region Tutorial Stage 2
            if (gm.tutorialStage == 2)
            {
                // Position the second group of UI to be in the same position the first UI was in
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = gm.firstUI.gameObject.GetComponent<RectTransform>().localPosition;

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    if (i == 2 || i == 3)
                    {
                        gm.allVisuals[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.allVisuals[i].gameObject.SetActive(false);
                    }
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    if (i == 1)
                    {
                        //gm.arrowNotes[i].gameObject.transform.position = new Vector3(gm.arrowNotes[i].gameObject.transform.localPosition.x, tempY - 100, gm.arrowNotes[i].gameObject.transform.position.z);
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.RightArrowNote;
                        gm.arrowNotes[i].color = gm.horizontalNoteArrowC;
                        gm.arrowNotes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrowNotes[i].gameObject.SetActive(false);
                    }
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    if (i == 1)
                    {
                        gm.arrow[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrow[i].gameObject.SetActive(false);
                    }
                }
            }
            #endregion
            // Combine left/right regular arrow note
            #region Tutorial Stage 3   
            if (gm.tutorialStage == 3)
            {
                // Lower the Y position of the secondUI UI area
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = gm.originalSecondUIPos;

                //pos.y = -50;
                //gm.secondUI.position = pos;
                //gm.secondUI.GetComponent<RectTransform>().localPosition = pos;

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(true);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    gm.arrowNotes[i].color = gm.horizontalNoteArrowC;
                    gm.arrowNotes[i].gameObject.SetActive(true);
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    gm.arrow[i].gameObject.SetActive(true);
                }
            }
            #endregion
            // Launch Notes
            #region Tutorial Stage 4
            if (gm.tutorialStage == 4)
            {
                // Lower the Y position of the secondUI UI area
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = gm.originalSecondUIPos;

                //pos.y = -50;
                //gm.secondUI.position = pos;
                //gm.secondUI.GetComponent<RectTransform>().localPosition = pos;

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(true);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    gm.arrowNotes[i].color = gm.horizontalLaunchArrowC;
                    gm.arrowNotes[0].GetComponent<Image>().sprite = gm.leftLaunchNote;
                    gm.arrowNotes[1].GetComponent<Image>().sprite = gm.rightLaunchNote;
                    gm.arrowNotes[i].gameObject.SetActive(true);
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    gm.arrow[i].gameObject.SetActive(true);
                }
            }
            #endregion
            // Up arrow notes
            #region Tutorial Stage 5
            if (gm.tutorialStage == 5)
            {
                // Lower the Y position of the secondUI UI area
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = gm.originalSecondUIPos;

                //pos.y = -50;
                //gm.secondUI.position = pos;
                //gm.secondUI.GetComponent<RectTransform>().localPosition = pos;

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(false);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    gm.arrowNotes[i].color = gm.upArrowC;
                    gm.arrowNotes[0].GetComponent<Image>().sprite = gm.upArrowNote;
                    if (i == 1)
                    {
                        gm.arrowNotes[i].gameObject.SetActive(false);
                    }
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    gm.arrow[i].gameObject.SetActive(false);
                }

                // Enables/Disables the 'Hold / Release' Texts
                for (int i = 0; i < gm.supportingTexts.Length; i++)
                {
                    gm.supportingTexts[i].gameObject.SetActive(false);
                }
            }
            #endregion
            // Blast notes
            #region Tutorial Stage 6
            if (gm.tutorialStage == 6)
            {
                // Lower the Y position of the secondUI UI area
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition.x, 230, gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition.z);

                gm.plusSymbol.gameObject.SetActive(true);

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(true);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    if (i == 0)
                    {
                        gm.arrowNotes[i].color = gm.blastNoteC;
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.blastNote;
                        gm.arrowNotes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrowNotes[i].gameObject.SetActive(false);
                    }
                }

                // Enables/Disables the 'Hold / Release' Texts
                for (int i = 0; i < gm.supportingTexts.Length; i++)
                {
                    if (i == 2 || i == 3)
                    {
                        gm.supportingTexts[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.supportingTexts[i].gameObject.SetActive(false);
                    }
                }
            }
            #endregion
            // Bomb notes
            #region Tutorial Stage 7
            if (gm.tutorialStage == 7)
            {
                gm.plusSymbol.gameObject.SetActive(false);

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(false);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    if (i == 0)
                    {
                        gm.arrowNotes[i].GetComponent<Image>().color = Color.white;
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.bombIcon;
                        gm.arrowNotes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        gm.arrowNotes[i].gameObject.SetActive(false);
                    }

                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    gm.arrow[i].gameObject.SetActive(false);
                }

                // Enables/Disables the 'Hold / Release' Texts
                for (int i = 0; i < gm.supportingTexts.Length; i++)
                {
                    gm.supportingTexts[i].gameObject.SetActive(false);
                }
            }
            #endregion
            // Shield
            #region Tutorial Stage 8
            if (gm.tutorialStage == 8)
            {
                // Lower the Y position of the secondUI UI area
                gm.secondUI.gameObject.GetComponent<RectTransform>().localPosition = gm.originalSecondUIPos;

                // Enable/disable the correct key images
                for (int i = 0; i < gm.allVisuals.Length; i++)
                {
                    gm.allVisuals[i].gameObject.SetActive(false);
                }

                // Enable/Disable the correct arrow notes
                for (int i = 0; i < gm.arrowNotes.Length; i++)
                {
                    if (i == 0)
                    {
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.playerShield;
                    }
                    else
                    {
                        gm.arrowNotes[i].GetComponent<Image>().sprite = gm.playerNoShield;
                    }

                    gm.arrowNotes[i].GetComponent<Image>().color = Color.white;
                    gm.arrowNotes[i].transform.localScale *= 1.5f;
                    gm.arrowNotes[i].gameObject.SetActive(true);
                }

                // enable/disable the correct arrow
                for (int i = 0; i < gm.arrow.Length; i++)
                {
                    gm.arrow[i].gameObject.SetActive(false);
                }

                // Enables/Disables the 'Hold / Release' Texts
                for (int i = 0; i < gm.supportingTexts.Length; i++)
                {
                    gm.supportingTexts[i].gameObject.SetActive(false);
                }

                // Enable/Disable the space bar images
                for (int i = 0; i < gm.spaceBar.Length; i++)
                {
                    gm.spaceBar[i].gameObject.SetActive(true);
                }

                for (int i = 0; i < gm.spaceSupportingTexts.Length; i++)
                {
                    gm.spaceSupportingTexts[i].gameObject.SetActive(true);
                }

                gm.tutAreaTextBG.gameObject.SetActive(true);
            }
            #endregion

            if (gm.tutorialStage < 3)
                {
                    foreach (Text t in gm.keyTexts)
                    {
                        t.text = gm.keyText[gm.tutorialStage - 1];
                    }
                }
                else
                {
                    for (int i = 0; i < gm.keyTexts.Length; i++)
                    {
                        gm.keyTexts[0].text = "A";
                        gm.keyTexts[1].text = "A";
                        gm.keyTexts[2].text = "L";
                        gm.keyTexts[3].text = "L";
                    }
                }
        }

        //Debug.Log(gm.tutTexts[gm.tutorialStage]);
        //Debug.Break();

        // Assign the correct text for the text
        gm.tutAreaText.text = gm.tutAreaInfo;

        // turn the tutorial text to visible
        gm.tutAreaText.gameObject.SetActive(true);

        // turn on the tutorial UI
        if (!gm.tutorialUI.activeSelf)
        {
            gm.tutorialUI.SetActive(true);
        }

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
                gm.PauseGame(true);
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
        if (selectedMap)
        {
            foreach (AudioSource aSource in am.gameObject.GetComponents<AudioSource>())
            {
                //Debug.Log(aSource);
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
            //Debug.Break();
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
        selectedMap = button.GetComponent<MapInfo>().map;

        gm.mapSelectText.text = "Title " + selectedMap.title +
            "\nBPM " + selectedMap.bpm + "\nDifficulty " + selectedMap.difficulty +
            "\nLength " + selectedMap.length + "\nGenre " + selectedMap.subGenre;

        // Enable the start button if it isn't already
        if (!gm.startBtn.activeSelf)
        {
            gm.startBtn.SetActive(true);
        }
    }

    // Happens when the player presses the start button
    public void LoadTrack()
    {
        // Disable the ability to pause
        gm.cantPause = true;

        gm.accuracy = selectedMap.averageBeatsBtwNotes;

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
