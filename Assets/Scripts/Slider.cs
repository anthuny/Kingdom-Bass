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
    public Gradient nonMissedGrad;
    public Gradient missedGrad;
    public bool hasAtLeastTwo;
    bool doneOnce;

    private void Start()
    {
        nonMissedGrad = gm.notMissedGrad;
        missedGrad = gm.missedGrad;
        gameObject.GetComponent<LineRenderer>().colorGradient = nonMissedGrad;
        tc.notesSpawned++;
    }

    public void Missed()
    {
        missed = true;
        //noteCalculatedAcc = true;
        gameObject.GetComponent<LineRenderer>().colorGradient = missedGrad;

        // Change the colour of the slider edge for the entire slider to the miss colour
        foreach (Transform t in setOfSliderNotes)
        {
            if (!t.gameObject.GetComponent<Note>())
            {
                t.GetChild(0).GetComponent<Renderer>().material.SetColor("_UnlitColor", gm.sliderEdgeColorMiss);
            }
            else
            {
                t.gameObject.GetComponent<Note>().sliderEdge.GetComponent<Renderer>().material.SetColor("_UnlitColor", gm.sliderEdgeColorMiss);
            }
        }
    }
}
