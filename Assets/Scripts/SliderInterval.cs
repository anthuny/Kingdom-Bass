using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderInterval : MonoBehaviour
{
    public Gamemode gm;
    public Player player;
    public Note note;

    public int sliderStartCount;
    public int sliderEndCount;
    public Vector3 sliderStartPos;
    public Vector3 sliderEndPos;
    public float sliderInterval;

    private bool queuedToDie;

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



        if (transform.position.z < player.transform.position.z && !queuedToDie)
        {
            StartCoroutine("CheckIfPassedPlayer");
        }

    }

    IEnumerator CheckIfPassedPlayer()
    {
        queuedToDie = true;

        yield return new WaitForSeconds(1.75f);

        Destroy(gameObject);
    }
}
