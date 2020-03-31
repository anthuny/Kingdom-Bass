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

    // Update is called once per frame
    void Update()
    {
        // relocate this object to the sliderInterval (t) between two set slider points
        if (note.sliderLr)
        {
            // reason - strange error was happening.
            if ((int)sliderStartCount > slider.gameObject.GetComponent<LineRenderer>().positionCount - 1)
            {
                StartCoroutine("CheckIfPassedPlayer");
                return;
            }

            sliderStartPos = note.sliderLr.GetPosition((int)sliderStartCount);
            if (sliderEndCount < slider.gameObject.GetComponent<LineRenderer>().positionCount)
            {
                sliderEndPos = note.sliderLr.GetPosition((int)sliderEndCount);
            }
            else if (sliderEndCount == slider.gameObject.GetComponent<LineRenderer>().positionCount)
            {
                slider.allSliderIntervals.Remove(transform);
                slider.sliderIntervalsInFront.Remove(transform);
                Destroy(gameObject);
            }

            Vector3 dist = Vector3.Lerp(sliderStartPos, sliderEndPos, sliderInterval);

            transform.position = dist;
        }

        if (transform.position.z <= player.transform.position.z && !queuedToDie)
        {
            StartCoroutine("CheckIfPassedPlayer");
        }
    }

    IEnumerator CheckIfPassedPlayer()
    {
        queuedToDie = true;

        posWhenPassedPlayer = transform.position;

        distFromPlayer = Mathf.Abs(posWhenPassedPlayer.x - player.transform.position.x);

        if (!slider.missedOn)
        {
            if (distFromPlayer > gm.maxDistInterval * 2 || player.isShielding)
            { 
                player.Missed(false);
                slider.missedOn = true;
                Debug.Log("too far from interval " + distFromPlayer + " " + gameObject.name);
                //Debug.Break();
            }
        }

        if (slider.sliderIntervalsInFront.Count == 1 && !slider.missedOn)
        {
            player.HitPerfect();
        }

        slider.sliderIntervalsInFront.Remove(transform);

        yield return new WaitForSeconds(1.75f);

        if (slider.allSliderIntervals.Count == 1)
        {
            if (slider)
            {
                gm.sliders.Remove(slider.gameObject.transform);
                Destroy(slider.gameObject);

                // Destroy all temporary game objects in the scene that were used for their transform for the temp slider locations
                foreach (Transform child in gm.sliderTransformPar.transform)
                {
                    if (child != transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }

                // Clear the stet of slider notes list
                slider.setOfSliderNotes.Clear();
            }
        }

        slider.allSliderIntervals.Remove(transform);
        Destroy(gameObject);
    }
}
