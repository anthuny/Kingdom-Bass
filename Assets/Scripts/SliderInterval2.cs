using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderInterval2 : MonoBehaviour
{
    private float distFromPlayer;
    public Player player;
    public Gamemode gm;
    public Slider slider;

    private bool doneOnce;
    bool doneOnce2;
    public Transform parent;
    public float sliderStartCount;
    // For bomb
    public GameObject note;
    private Note noteScript;
    public bool fromBomb;
    public bool fromNote;

    private void FixedUpdate()
    {
        if (fromBomb)
        {
            // If the nearest bomb reaches the player...
            if (parent.position.z - 2.5f <= player.gameObject.transform.position.z && !doneOnce)
            {
                doneOnce = true;

                noteScript = note.GetComponent<Note>();
                CheckIfPlayerHit();
            }
        }
    }
    void Update()
    {
        if (!fromBomb && !fromNote)
        {
            // If this note reaches the player...
            if (parent.position.z - gm.sliderOffset <= player.gameObject.transform.position.z && !doneOnce)
            {
                doneOnce = true;
                CheckIfPlayerHit();
            }

            // reason - strange error was happening.
            if (!slider)
            {
                slider.sliderIntervalsInFront.Remove(parent);
                slider.allSliderIntervals.Remove(parent);
                Destroy(parent.gameObject);
            }
            else if ((int)sliderStartCount == slider.gameObject.GetComponent<LineRenderer>().positionCount - 1)
            {
                //Debug.Log("end " + parent.name);
                slider.sliderIntervalsInFront.Remove(parent);
                slider.allSliderIntervals.Remove(parent);
                Destroy(parent.gameObject);
            }
        }
    }

    void CheckIfPlayerHit()
    {
        distFromPlayer = Mathf.Abs(parent.position.x - player.transform.position.x);

        if (fromBomb)
        {
            if (noteScript.laneNumber == player.nearestLaneNumber)
            {              
                noteScript.bombHitPlayer = true;
                player.Missed(true);
            }
        }
        else
        {
            // If the player is not close enough to this interval
            if (distFromPlayer > gm.maxDistInterval && !slider.missedOn && !doneOnce2 || player.isShielding && !slider.missedOn && !doneOnce2)
            {
                doneOnce2 = true;
                player.Missed(false);
                slider.missedOn = true;
                slider.Missed();

                if (player.isShielding)
                {
                    //Debug.Log("not shielding when you should be");
                }
                else
                {
                    //Debug.Log("too far from interval " + distFromPlayer + " " + parent.name);
                    //Debug.Break();
                }

                Destroy();
                return;
            }

            else if (gm.maxDistInterval > distFromPlayer && !player.isShielding && !slider.missedOn && !doneOnce2 && slider.sliderIntervalsInFront.Count == 1)
            {
                player.HitPerfect();
                //Debug.Log("hit perfect");
            }
            Destroy();
        }
 
    }


    void Destroy()
    {
        // Remove the original interval from a list
        if (!parent.gameObject.GetComponent<Note>())
        {
            slider.sliderIntervalsInFront.Remove(parent);
        }

        Invoke("Kill", 1.75f);
    }

    void Kill()
    {
        // Remove the original interval from a list
        if (!parent.gameObject.GetComponent<Note>())
        {
            slider.allSliderIntervals.Remove(parent);
        }

        if (slider.allSliderIntervals.Count == 1)
        {
            gm.sliders.Remove(slider.gameObject.transform);

            // Destroy all temporary game objects in the scene that were used for their transform for the temp slider locations
            foreach (Transform child in gm.sliderTransformPar.transform)
            {
                if (child != transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            // Clear the set of slider notes list
            slider.setOfSliderNotes.Clear();

            Destroy(slider.gameObject);
        }

        Destroy(parent.gameObject);
    }
}
