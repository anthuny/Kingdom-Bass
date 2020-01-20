using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
    private PathManager pm;
    [TextArea(3, 10)]
    public string song1Text;

    public string[] laneCodes = new string[] {"lane1Code", "lane2Code", "lane2Code", "lane2Code", "lane2Code", "lane2Code", "lane2Code", "lane2Code"};

    // Lane codes
    public string lane1Code, lane2Code, lane3Code, lane4Code, lane5Code, lane6Code, lane7Code, lane8Code;

    // Note code
    public string noteCode;

    public List<float> allNotes = new List<float>();

    public GameObject notes;
    public GameObject noteVisual;

    public float trackBpm;

    public float secPerBeat;
    public float trackPos;
    public float trackPosInBeats;
    public float dspTrackTime;
    public AudioSource audioSource;

    private float t = 0;

    [Tooltip("Amount of beats that must play before the first note spawns")]
    public int beatsBeforeStart;
    public int beatsShownInAdvance;

    private float timer;
    public float timeToWait;
    bool spawnedOnce;
    GameObject player;
    
    public float noteTimeToArriveMult;
    public bool tIsReady;

    XmlDocument levelDataXml;

    private void Awake()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("LevelData");
        levelDataXml = new XmlDocument();
        levelDataXml.LoadXml(xmlTextAsset.text);
    }

    void Start()
    {
        // Load the audiosource atteched to this gameobject
        audioSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / trackBpm;

        player = FindObjectOfType<Player>().gameObject;
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();

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



    public void SpawnNotes(string noteType, string laneNumber, string arrowD)
    {
        // Checks for notes in each lane, If it belongs in a lane, make it belong there 
        // This is currently only going to work for lanes 
        // TODO : Make it work for type of note aswell, when I create that type of note.

        for (int i = 1; i <= pm.maxLanes; i++)
        {
            if (int.Parse(laneNumber) == i)
            {              
                // Spawn noteVisual, at notes position
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);

                // Set the name of the note to the lane it is in
                // TODO : Add the type of note aswell when I add the note in the game
                go.name = i.ToString();

                // Parent this note to the notes object 
                go.transform.SetParent(notes.transform);

                // Set the arrow direction of the note
                go.GetComponent<Note>().arrowDir = arrowD;

                // Set the lane number of the note 
                go.GetComponent<Note>().laneNumber = i;

                // Disable the note
                go.SetActive(false);

                // Add the note to allNotes list | TODO: Do I need this?
                allNotes.Add(i);

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
                            }
                        }
                    }
                }

                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        trackPos = audioSource.time;

        // Determine the time in seconds (timeToWait) it takes for 1 note to reach the player
        // find the average of this number over many itterations to get a more accurate number - TODO:
        if (t <= 1 && audioSource.isPlaying && trackPosInBeats < 1)
        {
            tIsReady = false;
            t = trackPosInBeats - Mathf.FloorToInt(trackPosInBeats);    
            Mathf.Clamp01(t);
            timeToWait += Time.deltaTime;

        }
        else
        {
            tIsReady = true;
        }

        // Determine how many beats since the song started
        trackPosInBeats = (trackPos / secPerBeat);

        if (audioSource.isPlaying && tIsReady)
        {
            // Spawn the first note.
            if (!spawnedOnce)
            {
                spawnedOnce = true;
                AssignNotes();
            }

            // Wait for secPerBeat duration to spawn the second note. 
            // This code is where you determine how many notes you want to be able to see at a time.
            // To do this, decrease the amount of time secPerBeat is.
            if (timer > (secPerBeat))
            {
                timer -= (secPerBeat);
                AssignNotes();
            }
            timer += Time.deltaTime;
        }       
    }

    class GetNote
    {
        public string noteType { get; private set; }
        public string noteLane { get; private set; }
        public string noteArrowD { get; private set; }

        TrackCreator tc;

        public GetNote(XmlNode curNoteNode)
        {
            tc = FindObjectOfType<TrackCreator>();
            noteType = curNoteNode["Note_Type"].InnerText;
            noteLane = curNoteNode["Note_Lane"].InnerText;
            noteArrowD = curNoteNode["Note_ArrowD"].InnerText;

            XmlNode NoteNode = curNoteNode.SelectSingleNode("Note_Type");
            XmlNode laneNode = curNoteNode.SelectSingleNode("Note_Lane");
            XmlNode arrowDNode = curNoteNode.SelectSingleNode("Note_ArrowD");

            tc.SpawnNotes(NoteNode.InnerText, laneNode.InnerText, arrowDNode.InnerText);
        }
    }
}
