using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Collections;

public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
    private PathManager pm;
    [TextArea(3, 10)]
    public string song1Text;

    public string[] laneCodes = new string[] {"lane1Code", "lane2Code", "lane3Code", "lane4Code", "lane5Code", "lane6Code", "lane7Code", "lane8Code"};

    // Lane codes
    public string lane1Code, lane2Code, lane3Code, lane4Code, lane5Code, lane6Code, lane7Code, lane8Code;

    // Note code
    public string noteCode;

    public List<float> allNotes = new List<float>();
    public List<int> noteEighthCount = new List<int>();
    public List<float> trackPosIntervalsList = new List<float>();
    public List<float> trackPosIntervalsList2 = new List<float>();
    public List<float> trackPosIntervalsList3 = new List<float>();

    public float trackPosNumber;
    public float trackPosNumber2;

    public GameObject notes;
    public GameObject noteVisual;
    public float maxNoteIntervalsEachBeat;

    public float trackBpm;

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
    public float nextNoteInBeats3;

    public float curNoteDiff, nextNoteDiff;
    private float oldNextNoteDiff;
    float lastBeat;

    [HideInInspector]
    public int nextIndex = 0;
    [HideInInspector]
    public float nextIndex2 = 0;
    [HideInInspector]
    public int nextIndex3 = 0;

    [Tooltip("Amount of beats that must play before the first note spawns")]
    [Range(1, 25)]
    public int beatsBeforeStart;
    [Tooltip("The amount of beats that must happen for the note to get to the end")]
    public float noteTimeTaken;
    [Tooltip("The MAX amount of beats that must happen for the note to get to the end")]
    public float noteTimeTakenMax;
    [Tooltip("As this number is lowered, the window of opportunity for hitting notes is smaller")]
    public float noteHitBoxDifficult;

    Player player;

    XmlDocument levelDataXml;

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
    public float nextNoteInBeats, nextNoteInBeats2;

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
    private void Awake()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("LevelData");
        levelDataXml = new XmlDocument();
        levelDataXml.LoadXml(xmlTextAsset.text);
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();

        // Load the audiosource atteched to this gameobject
        audioSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / trackBpm;

        // Assign the laneCode array with the actual codes for each lane
        laneCodes[0] = lane1Code;
        laneCodes[1] = lane2Code;
        laneCodes[2] = lane3Code;
        laneCodes[3] = lane4Code;
        laneCodes[4] = lane5Code;
        laneCodes[5] = lane6Code;
        laneCodes[6] = lane7Code;
        laneCodes[7] = lane8Code;

        FindNotes(); 
    }

    public void FindNotes()
    {
        XmlNodeList notes = levelDataXml.SelectNodes("/Levels/Level/Notes/Note");

        foreach (XmlNode note in notes)
        {
            GetNote newGetNote = new GetNote(note);
        }
    }
    public void SpawnNotes(string noteType, string laneNumber, string arrowD, string EighthWait)
    {
        // Checks for notes in each lane, If it belongs in a lane, make it belong there 
        for (int i = 1; i <= pm.maxLanes; i++)
        {
            if (int.Parse(laneNumber) == i)
            {              
                // Spawn noteVisual, at notes position
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                Note noteScript = go.GetComponent<Note>();

                // Set the name of the note to the lane it is in
                // TODO : Add the type of note aswell when I add the note in the game
                go.name = i.ToString();

                // Parent this note to the notes object 
                go.transform.SetParent(notes.transform);

                // Set the note type and arrow direction
                noteScript.noteType = noteType;
                noteScript.noteDir = arrowD;

                // Set the amount of eighthwaits the note is to perfom
                noteScript.eighthWait = int.Parse(EighthWait);

                // Set the lane number of the note 
                noteScript.laneNumber = i;

                // Disable the note
                go.SetActive(false);

                // Add the note to allNotes list | This is needed for the dynamic spawning times if using it.
                allNotes.Add(i);

                // Add the eightth wait for each note to noteEightCount list
                noteEighthCount.Add(int.Parse(EighthWait));

                // Exit the function immediately
                return;
            }
        }
    }

    void AssignNotes()
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

        //Re-position the notes
        for (int i = 0; i < notes.transform.childCount; i++)
        {
            // If the note is not active, set it active
            if (!notes.transform.GetChild(i).gameObject.activeSelf)
            {
                notes.transform.GetChild(i).gameObject.SetActive(true);

                for (int z = 0; z < pm.maxLanes; z++)
                {
                    // Find the note's name that equals the correct code
                    if (notes.transform.GetChild(i).name.Contains(laneCodes[z]))
                    {
                        // Loop through all paths in the current segment
                        for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                        {
                            // If the lane number equals the correct one
                            if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(laneCodes[z]))
                            {
                                // move the note to the correct lane
                                notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, path.pathLength);

                                // Set the starting position variable for the note to it's position after it has been moved to starting position
                                notes.transform.GetChild(i).GetComponent<Note>().startingPos.z = notes.transform.GetChild(i).GetComponent<Note>().transform.position.z;

                                // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                                notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                                // Allow the note to move when ready
                                notes.transform.GetChild(i).GetComponent<Note>().canMove = true;
                            }
                        }
                    }
                }

                return;
            }
        }
    }

    void UpdateMissCondition()
    {
        // Ensure that the following if statement only happens when there is a new 'nextNoteDiff'
        if (oldNextNoteDiff != nextNoteDiff)
        {
            oldNextNoteDiff = nextNoteDiff;
            doneOnce2 = false;
        }

        /*
        // Ensure through a bool, if the player recieves a miss or not for not inputing anything
        if (doneOnce && trackPosInBeatsGame >= nextNoteDiff && !doneOnce2)
        {
            Debug.Log("asd");
            doneOnce2 = true;
            

            if (!gm.scoreIncreased)
            {
                //Debug.Break();
                player.Missed();
            }

            gm.scoreIncreased = false;

            //Currently inbetween beats
           // nextIndex3++;

            trackPosNumber = trackPosIntervalsList3[nextIndex3 - 1];
            trackPosNumber2 = trackPosIntervalsList3[nextIndex3];

            previousNoteBeatTime2 = previousNoteBeatTime + (noteEighthCount[nextIndex3 - 1]);


            nextNoteInBeats = previousNoteBeatTime2;
            nextNoteInBeats2 = nextNoteInBeats + (noteEighthCount[nextIndex3]);

            float a = (nextNoteInBeats + previousNoteBeatTime) / 2;
            float b = (previousNoteBeatTime2 + nextNoteInBeats2) / 2;
            curNoteDiff = a;
            nextNoteDiff = b;
        }
        */
    }
    private void Update()
    {
        //UpdateMissCondition();

        // Index out of bounds check
        if (trackPosIntervalsList.Count >= 1)
        {
            // Set the max amount of time the player has to input another movement before their last 
            // input is what is score they will get for the current note
            gm.maxTimeBetweenInputs = ((secPerBeat * trackPosIntervalsList[0]) / 2);
        }

        // Start the song 
        if (Input.GetKeyDown(KeyCode.Space) && !audioSource.isPlaying && Time.realtimeSinceStartup > 3)
        {
            dspTrackTime = (float)AudioSettings.dspTime;

            audioSource.Play();
        }

        // Keep track of the track's position in seconds from when it started
        trackPos = (float)(AudioSettings.dspTime - dspTrackTime);

        // Determine how many beats since the track started
        trackPosInBeats = (trackPos / secPerBeat);

        // Determine how many beats since the game started
        trackPosInBeatsGame = trackPosInBeats - beatsBeforeStart + 1;

        if (audioSource.isPlaying)
        {
            // Spawn a note
            // Ensures intro is over before starting
            if (trackPos > (lastBeat + ((beatsBeforeStart - 1) * secPerBeat)) + (secPerBeat * noteEighthCount[nextIndex]))
            {
                AssignNotes();

                lastBeat += secPerBeat * noteEighthCount[nextIndex];

                trackPosIntervals = noteEighthCount[nextIndex];
                trackPosIntervals2 = noteEighthCount[nextIndex];
                trackPosIntervals3 += noteEighthCount[nextIndex];

                trackPosIntervalsList.Add(trackPosIntervals);
                trackPosIntervalsList2.Add(trackPosIntervals2);
                trackPosIntervalsList3.Add(trackPosIntervals3);

                nextIndex2 = noteEighthCount[0];
                nextIndex++;
            }

            // Index out of bounds check
            if (trackPosIntervalsList2.Count == 1)
            {
                firstInterval = trackPosIntervalsList2[0];

                // Determine what the first next note will be for score measuring
                pointToNextBeat = trackPosIntervalsList2[0] * (noteTimeTaken + 1);
                //firstNote = pointToNextBeat;
            }

            if (trackPosIntervalsList2.Count == 2)
            {
                pointToNextBeat2 = trackPosIntervalsList2[1] * (noteTimeTaken + 1);
            }

        }
    }

    // XML referencing for each note.
    class GetNote
    {
        public string noteType { get; private set; }
        public string noteLane { get; private set; }
        public string noteArrowD { get; private set; }
        public string noteEighthWait { get; private set; }

        TrackCreator tc;

        public GetNote(XmlNode curNoteNode)
        {
            tc = FindObjectOfType<TrackCreator>();
            noteType = curNoteNode["Note_Type"].InnerText;
            noteLane = curNoteNode["Note_Lane"].InnerText;
            noteArrowD = curNoteNode["Note_ArrowD"].InnerText;
            noteEighthWait = curNoteNode["Note_EighthWait"].InnerText;

            XmlNode NoteNode = curNoteNode.SelectSingleNode("Note_Type");
            XmlNode laneNode = curNoteNode.SelectSingleNode("Note_Lane");
            XmlNode arrowDNode = curNoteNode.SelectSingleNode("Note_ArrowD");
            XmlNode EighthWaitNode = curNoteNode.SelectSingleNode("Note_EighthWait");

            tc.SpawnNotes(NoteNode.InnerText, laneNode.InnerText, arrowDNode.InnerText, EighthWaitNode.InnerText);
        }
    }
}
