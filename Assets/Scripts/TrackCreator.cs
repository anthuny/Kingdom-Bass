using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
    private PathManager pm;

    [Header("Maps")]
    public Map[] mapDetails;

    [Header("Map Selection")]
    public bool mapHasBeenSelected;
    [HideInInspector]
    public Map selectedMap;

    [Header("Map Names")]
    public string map1;
    public string map2;


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
    public AudioSource audioSource;
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

    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();

        // Load the audiosource atteched to this gameobject
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        QueueNoteSpawns();
    }

    public void AssignNotes(string noteType, string laneNumber, string arrowD, string beatWait)
    {
        //Debug.Break();
        // Checks for notes in each lane, If it belongs in a lane, make it belong there 
        for (int i = 1; i <= pm.maxLanes; i++)
        {
            if (int.Parse(laneNumber) == i)
            {              
                // Spawn noteVisual, at notes position
                GameObject go = Instantiate(noteVisual, notesObj.transform.position, Quaternion.identity);
                Note noteScript = go.GetComponent<Note>();

                // Set the name of the note to the lane it is in
                // TODO : Add the type of note aswell when I add the note in the game
                go.name = i.ToString();

                // Parent this note to the notes object 
                go.transform.SetParent(notesObj.transform);

                // Set the note type and arrow direction
                noteScript.noteType = noteType;
                noteScript.noteDir = arrowD;

                // Set the amount of beatWaits the note is to perfom
                noteScript.beatWait = float.Parse(beatWait);

                // Set the lane number of the note 
                noteScript.laneNumber = i;

                // Disable the note
                go.SetActive(false);

                // Add the note to allNotes list | This is needed for the dynamic spawning times if using it.
                noteLanes.Add(i);

                if (noteType != "bomb")
                {
                    // Add the note to the notes non-bomb note list
                    notes.Add(go);
                }

                // add notes to the all notes list
                allNotes.Add(go);

                // Add the beatWait wait for each note to beatWaitcount list
                beatWaitCount.Add(noteScript.beatWait);
                beatWaitAccum += beatWaitCount[beatWaitCount.Count - 1];

                // Add the beatwaitcount to a list that accumilates for each element
                beatWaitCountAccum.Add(beatWaitAccum);
                    
                // Set this note's beatWaitCur to equal the beatWaitAccum when it spawns
                go.GetComponent<Note>().beatWaitCur = beatWaitCountAccum[beatWaitCountAccum.Count - 1] - beatWaitCount[0];

                allNoteTypes.Add(noteType);
            }
        }

        // Ensure that the ui flip to in-game mode only happens once, rather then for every note in the song
        if (!mapHasBeenSelected)
        {
            UpdateMapSelectionUI();

            player.oldNearestLaneNumber = player.nearestLaneNumber;
        }
    }

    void UpdateMapSelectionUI()
    {
        // Now that all the notes have loaded, allow the player to start
        mapHasBeenSelected = true;

        trackInProgress = true;

        gm.startBtn.SetActive(false);

        // Display all debug UI
        gm.ToggleDebugUI();
        gm.UpdateUI();

        StartCoroutine("StartSong");
    }

    IEnumerator StartSong()
    {
        yield return new WaitForSeconds(selectedMap.timeBeforeStart);

        // assign the track in the audio source
        audioSource.clip = selectedMap.track;

        // Start the track timer?
        dspTrackTime = (float)AudioSettings.dspTime;

        audioSource.Play();
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
        Path path = pm.initialPath.GetComponent<Path>();

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
        if (!audioSource.isPlaying)
        {
            return;
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

    public void SelectMap(Button button)
    {
        // Initialize what map was selected
        selectedMap = button.GetComponent<MapInfo>().map;

        // Enable the start button if it isn't already
        if (!gm.startBtn.activeSelf)
        {
            gm.startBtn.SetActive(true);
        }
    }

    public void loadTrack()
    {
        // disable all buttons in map selection screen
        gm.ToggleMapSelectionButtons(false);

        // Assign map values (BPM)
        SetupTrack(selectedMap.bpm);

        mapLD = new XmlDocument();

        mapLD.LoadXml(selectedMap.mapXML.text);

        XmlNodeList notes = mapLD.SelectNodes("/Levels/Level/Notes/Note");

        foreach (XmlNode note in notes)
        {
            GetNote newGetNote = new GetNote(note);
        }

        return;
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
