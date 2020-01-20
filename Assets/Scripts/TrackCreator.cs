using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
    private PathManager pm;
    public TextAsset song1;
    [TextArea(3, 10)]
    public string song1Text;

    // Lane codes
    public string lane1Code;
    public string lane2Code;
    public string lane3Code;
    public string lane4Code;
    public string lane5Code;

    // Note code
    public string noteCode;

    // Lines
    public string[] lines;


    public List<int> lane1Notes = new List<int>();
    public List<int> lane2Notes = new List<int>();
    public List<int> lane3Notes = new List<int>();
    public List<int> lane4Notes = new List<int>();
    public List<int> lane5Notes = new List<int>();

    public List<float> allNotes = new List<float>();

    public GameObject notes;
    public GameObject noteVisual;

    public float trackBpm;


    public float secPerBeat;
    public float trackPos;
    public float trackPosInBeats;
    public float dspTrackTime;
    public AudioSource audioSource;

    public float t = 0;
    //private int nextIndex = 0;

    [Tooltip("Amount of beats that must play before the first note spawns")]
    public int beatsBeforeStart;
    public int beatsShownInAdvance;

    private float timer;
    public float timeToWait;
    bool spawnedOnce;
    GameObject player;

    public float noteTimeToArriveMult;
    public bool tIsReady;

    // Start is called before the first frame update
    void Start()
    {
        // Load the audiosource atteched to this gameobject
        audioSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / trackBpm;

        // Assign what beatsshowninadvance is 
        //BeatsShownInAdvance *= secPerBeat;

        player = FindObjectOfType<Player>().gameObject;
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        
        song1Text = song1.text;
        
        // References each line
        lines = song1Text.Split("\n"[0]);

        for (int i = 0; i < lines.Length; i++)
        {
            //Checks for notes in lane 1, If there is one, add it to the lane note list 
            if (lines[i].Contains(lane1Code) && lines[i].Contains(noteCode))
            {
                lane1Notes.Add(i + 1);
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                go.name = "L1N";
                go.transform.SetParent(notes.transform);
                Note note = go.AddComponent<Note>();
                go.SetActive(false);

                allNotes.Add(i);
            }

            //Checks for notes in lane 2, If there is one, add it to the lane note list
            if (lines[i].Contains(lane2Code) && lines[i].Contains(noteCode))
            {
                lane2Notes.Add(i + 1);
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                go.name = "L2N";
                go.transform.SetParent(notes.transform);
                Note note = go.AddComponent<Note>();
                go.SetActive(false);

                allNotes.Add(i);
            }

            //Checks for notes in lane 3, If there is one, add it to the lane note list
            if (lines[i].Contains(lane3Code) && lines[i].Contains(noteCode))
            {
                lane3Notes.Add(i + 1);
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                go.name = "L3N";
                go.transform.SetParent(notes.transform);
                Note note = go.AddComponent<Note>();
                go.SetActive(false);

                allNotes.Add(i);
            }

            //Checks for notes in lane 4, If there is one, add it to the lane note list
            if (lines[i].Contains(lane4Code) && lines[i].Contains(noteCode))
            {
                lane4Notes.Add(i + 1);
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                go.name = "L4N";
                go.transform.SetParent(notes.transform);
                Note note = go.AddComponent<Note>();
                go.SetActive(false);

                allNotes.Add(i);
            }

            //Checks for notes in lane 5, If there is one, add it to the lane note list
            if (lines[i].Contains(lane5Code) && lines[i].Contains(noteCode))
            {
                lane5Notes.Add(i + 1);
                GameObject go = Instantiate(noteVisual, notes.transform.position, Quaternion.identity);
                go.name = "L5N";
                go.transform.SetParent(notes.transform);
                Note note = go.AddComponent<Note>();
                go.SetActive(false);

                allNotes.Add(i);
            }
        }

    }

    void SpawnNotes()
    {
        // Declare what the current segment is if it hasn't been done already.
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

                // Find the note's name that equals the correct code
                if (notes.transform.GetChild(i).name.Contains(lane1Code))
                {
                    //  Loop through all paths in the current segment
                    for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                    {
                        // If the lane number equals the correct one
                        if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(lane1Code))
                        {
                            // move the note to the correct lane
                            notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, path.pathLength);

                            // Set the starting position variable for the note to it's position after it has been moved to starting position
                            notes.transform.GetChild(i).GetComponent<Note>().startingPos.z = notes.transform.GetChild(i).GetComponent<Note>().transform.position.z;

                            // Turn the note online, so that it can be moved in the lane
                            notes.transform.GetChild(i).GetComponent<Note>().online = true;

                            // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                            notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;



                            notes.transform.GetChild(i).GetComponent<Note>().beatOfThisNote = (int)Mathf.Round(trackPosInBeats) + beatsShownInAdvance;
                        }
                    }
                }

                // Find the note's name that equals the correct code
                else if (notes.transform.GetChild(i).name.Contains(lane2Code))
                {
                    //  Loop through all paths in the current segment
                    for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                    {
                        // If the lane number equals the correct one
                        if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(lane2Code))
                        {
                            // move the note to the correct lane
                            notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, pm.currentSegment.transform.GetChild(x).transform.position.z + path.pathLength);

                            //Turn the note online, so that it can be moved in the lane
                            notes.transform.GetChild(i).GetComponent<Note>().online = true;

                            // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                            notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                            // Set the starting position variable for the note to it's position after it has been moved to starting position
                            notes.transform.GetChild(i).GetComponent<Note>().startingPos = notes.transform.GetChild(i).GetComponent<Note>().transform.position;

                            notes.transform.GetChild(i).GetComponent<Note>().beatOfThisNote = (int)Mathf.Round(trackPosInBeats) + beatsShownInAdvance;
                        }
                    }
                }

                // Find the note's name that equals the correct code
                else if (notes.transform.GetChild(i).name.Contains(lane3Code))
                {
                    //  Loop through all paths in the current segment
                    for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                    {
                        // If the lane number equals the correct one
                        if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(lane3Code))
                        {
                            // move the note to the correct lane
                            notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, pm.currentSegment.transform.GetChild(x).transform.position.z + path.pathLength);

                            //Turn the note online, so that it can be moved in the lane
                            notes.transform.GetChild(i).GetComponent<Note>().online = true;

                            // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                            notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                            // Set the starting position variable for the note to it's position after it has been moved to starting position
                            notes.transform.GetChild(i).GetComponent<Note>().startingPos = notes.transform.GetChild(i).GetComponent<Note>().transform.position;

                            notes.transform.GetChild(i).GetComponent<Note>().beatOfThisNote = (int)Mathf.Round(trackPosInBeats) + beatsShownInAdvance;
                        }
                    }
                }

                // Find the note's name that equals the correct code
                else if (notes.transform.GetChild(i).name.Contains(lane4Code))
                {
                    //  Loop through all paths in the current segment
                    for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                    {
                        // If the lane number equals the correct one
                        if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(lane4Code))
                        {
                            // move the note to the correct lane
                            notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, pm.currentSegment.transform.GetChild(x).transform.position.z + path.pathLength);

                            //Turn the note online, so that it can be moved in the lane
                            notes.transform.GetChild(i).GetComponent<Note>().online = true;

                            // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                            notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                            // Set the starting position variable for the note to it's position after it has been moved to starting position
                            notes.transform.GetChild(i).GetComponent<Note>().startingPos = notes.transform.GetChild(i).GetComponent<Note>().transform.position;

                            notes.transform.GetChild(i).GetComponent<Note>().beatOfThisNote = (int)Mathf.Round(trackPosInBeats) + beatsShownInAdvance;
                        }
                    }
                }

                // Find the note's name that equals the correct code
                else if (notes.transform.GetChild(i).name.Contains(lane5Code))
                {
                    //  Loop through all paths in the current segment
                    for (int x = 0; x < pm.currentSegment.transform.childCount; x++)
                    {
                        // If the lane number equals the correct one
                        if (pm.currentSegment.transform.GetChild(x).gameObject.GetComponent<Path>().laneNumber == int.Parse(lane5Code))
                        {
                            // move the note to the correct lane
                            notes.transform.GetChild(i).position = new Vector3(path.pathWidth * x, 0.02f, pm.currentSegment.transform.GetChild(x).transform.position.z + path.pathLength);

                            //Turn the note online, so that it can be moved in the lane
                            notes.transform.GetChild(i).GetComponent<Note>().online = true;

                            // Determine what the pathwidth is so that the notes are in the correct X axis for their destination
                            notes.transform.GetChild(i).GetComponent<Note>().pathWidth = path.pathWidth * x;

                            // Set the starting position variable for the note to it's position after it has been moved to starting position
                            notes.transform.GetChild(i).GetComponent<Note>().startingPos = notes.transform.GetChild(i).GetComponent<Note>().transform.position;

                            notes.transform.GetChild(i).GetComponent<Note>().beatOfThisNote = (int)Mathf.Round(trackPosInBeats) + beatsShownInAdvance;
                        }
                    }
                }

                // Ensure that the for loop stops after 1 note was activated.
                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

        // Wait for the track position in beats to reach a beat.
        // Then spawn a note
        /*
        if (nextIndex < allNotes.Count && (allNotes[nextIndex] + (beatsBeforeStart)) < trackPosInBeats)
        {
            nextIndex++;
            SpawnNotes();
        }
        */
        // SPAWNING NOTES

        
        if (audioSource.isPlaying && tIsReady)
        {
            // Spawn the first note.
            if (!spawnedOnce)
            {
                spawnedOnce = true;
                SpawnNotes();
            }

            // Wait for secPerBeat duration to spawn the second note. 
            // This code is where you determine how many notes you want to be able to see at a time.
            // To do this, decrease the amount of time secPerBeat is.
            if (timer > (secPerBeat))
            {
                timer -= (secPerBeat);
                SpawnNotes();
            }
            timer += Time.deltaTime;
        }

        // anthony - the value secPerBeat is divided by. use that to maybe times how fast the notes are.


        
    }
}
