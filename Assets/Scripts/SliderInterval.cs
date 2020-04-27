using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderInterval : MonoBehaviour
{
    public Gamemode gm;
    public Player player;
    public Note note;
    public Slider slider;

    public float sliderStartCount;
    public float sliderEndCount;
    public Vector3 sliderStartPos;
    public Vector3 sliderEndPos;
    public float sliderInterval;

    private bool queuedToDie;

    private Vector3 posWhenPassedPlayer;
    private float distFromPlayer;

    private void Start()
    {
        GameObject go = Instantiate(note.sliderPredict, transform.position, Quaternion.identity);
        go.transform.SetParent(gameObject.transform);
        go.transform.localPosition += new Vector3(0, 0, -gm.sliderOffset);
        go.name = name;


        SliderInterval2 sliderPredictScript = go.GetComponent<SliderInterval2>();
        sliderPredictScript.slider = slider;
        sliderPredictScript.player = player;
        sliderPredictScript.gm = gm;
        sliderPredictScript.parent = gameObject.transform;
        sliderPredictScript.sliderStartCount = sliderStartCount;
        sliderPredictScript.note = note.gameObject;
        sliderPredictScript.noteScript = note;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLocation();
    }

    void UpdateLocation()
    {
        if (note.sliderLr)
        {
            sliderStartPos = note.sliderLr.GetPosition((int)sliderStartCount);
            if (sliderEndCount < slider.gameObject.GetComponent<LineRenderer>().positionCount)
            {
                sliderEndPos = note.sliderLr.GetPosition((int)sliderEndCount);
            }

            Vector3 dist = Vector3.Lerp(sliderStartPos, sliderEndPos, sliderInterval);

            transform.position = dist;
        }
    }
}
