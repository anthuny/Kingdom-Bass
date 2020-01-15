using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreator : MonoBehaviour
{
    private Gamemode gm;
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

    private string lane1NoteCode;

    public GameObject notes;
    public GameObject noteVisual;

    public float trackBpm;


    public float secPerBeat;
    public float trackPos;
    public float trackPosInBeats;
    public float dspTrackTime;
    public AudioSource audioSource;

    private int nextIndex = 0;

    [Tooltip("Amount of beats that must play before the first note spawns")]
    public float BeatsShownInAdvance;

    // Start is called before the first frame update
    void Start()
    {


        // Load the audiosource atteched to this gameobject
        audioSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / trackBpm;

        // Assign what beatsshowninadvance is 
        //BeatsShownInAdvance *= secPerBeat;

        // Record the time when the music starts
        dspTrackTime = (float)AudioSettings.dspTime;

        // Start the music
        audioSource.Play();

        gm = FindObjectOfType<Gamemode>();
        
        song1Text = song1.text;
        
        // References each line
        lines = song1Text.Split("\n"[0]);

        lane1NoteCode = lane1Code + noteCode;

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
        //Re-position the notes
        for (int i = 0; i < notes.transform.childCount; i++)
        {
            if (!notes.transform.GetChild(i).gameObject.activeSelf)
            {
                notes.transform.GetChild(i).gameObject.SetActive(true);

                // Re-position i into correct lane here
                // Continue reading off this source: https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
                return;
            }

        }
    }

    private void Update()
    {
        // Determine how many seconds since the track started
        trackPos = (float)(AudioSettings.dspTime - dspTrackTime);

        // Determine how many beats since the song started
        trackPosInBeats = trackPos / secPerBeat;

        // Wait for the track position in beats to reach a beat.
        // Then spawn a note
        if (nextIndex < allNotes.Count && (allNotes[nextIndex] + BeatsShownInAdvance) < trackPosInBeats)
        {
            Debug.Log("Spawning note");
            nextIndex++;

            SpawnNotes();
        }

    }
}
