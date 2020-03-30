using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    public Gamemode gm;
    public TrackCreator tc;
    public List<Transform> setOfSliderNotes = new List<Transform>();
    public List<Transform> allSliderIntervals = new List<Transform>();
    public List<Transform> sliderIntervalsInFront = new List<Transform>();
    public float indexOfSliderNoteSet = 1;
    public bool missed;
    public bool missedOn;
    public bool noteCalculatedAcc;

    private void Start()
    {
        gameObject.GetComponent<LineRenderer>().colorGradient = gm.notMissedGrad;
        tc.notesSpawned++;
    }

    public void Missed()
    {
        missed = true;
        //noteCalculatedAcc = true;
        gameObject.GetComponent<LineRenderer>().colorGradient = gm.missedGrad;
    }
}
