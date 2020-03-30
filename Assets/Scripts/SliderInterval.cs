using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderInterval : MonoBehaviour
{
    public Gamemode gm;
    public Player player;
    public Note note;
    public Slider slider;

    public int sliderStartCount;
    public int sliderEndCount;
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
            sliderStartPos = note.sliderLr.GetPosition(sliderStartCount);
            sliderEndPos = note.sliderLr.GetPosition(sliderEndCount);

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

        distFromPlayer = Vector3.Distance(posWhenPassedPlayer, player.transform.position);

        if (!slider.missedOn)
        {
            if (distFromPlayer > gm.maxDistInterval || player.isShielding)
            { 
                player.Missed(false);
                slider.missedOn = true;
                //Debug.Log("too far from interval " + distFromPlayer);
            }
            else
            {
               //Debug.Log("in range " + distFromPlayer);
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
            }
        }

        slider.allSliderIntervals.Remove(transform);
        Destroy(gameObject);
    }
}
