using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    private PathManager pm;
    private Gamemode gm;

    [HideInInspector]
    public bool isNote, isLaunch;

    [HideInInspector]
    public Vector3 startingPos;
    [HideInInspector]
    public float pathWidth;
    [HideInInspector]
    private Player player;
    [HideInInspector]
    public string arrowDir;
    //[HideInInspector]
    public int eighthWait;

    private bool doneOnce;
    private bool doneOnce2;
    private bool doneOnce3;


    public int laneNumber;
    [HideInInspector]
    public float percDistance;

    bool hitEnd;

    private float wallLengths;
    private Renderer rend;

    public Vector3 playerPos;

    private float startTime;
    private float curTime = 0;

    public bool canMove;

    public GameObject meshRendererRef;
    private Renderer noteRend;
    private float noteWidth;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();
        noteRend = meshRendererRef.GetComponent<Renderer>();
        noteWidth = noteRend.bounds.size.z;

        //Determine the direction of the arrow on the note
        switch (arrowDir)
        {
            case "left":
                if (isNote)
                {
                    Sprite leftArrow = Resources.Load<Sprite>("Sprites/T_LeftArrow") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;
                   
                }
                else if (isLaunch)
                {
                    Sprite leftArrowLaunch = Resources.Load<Sprite>("Sprites/T_LeftArrowLaunch") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;
                }

                break;

            case "right":
                if (isNote)
                {
                    Sprite rightArrow = Resources.Load<Sprite>("Sprites/T_RightArrow") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrow;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalNoteArrowC;
                }
                else if (isLaunch)
                {
                    Sprite rightArrowLaunch = Resources.Load<Sprite>("Sprites/T_RightArrowLaunch") as Sprite;

                    transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrowLaunch;
                    transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalLaunchArrowC;
                }


                break;

            case "up":
                Sprite upArrow = Resources.Load<Sprite>("Sprites/T_UpArrow") as Sprite;
                    
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = upArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.upArrowC;

                break;

            default:
                Debug.Log(this.gameObject.name + " does not have an arrow direction");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (canMove && !doneOnce)
        {
            doneOnce = true;
            startTime = tc.trackPos;
        }
        // Calculate the percantage of completion of the note on the lane
        percDistance = Mathf.Abs((transform.position.z - player.transform.position.z) / pm.pathLength) * 100;

        // Determine the speed the note needs to use to get to the player on the beat
        //gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);
        gm.noteSpeed = pm.pathLength / (tc.secPerBeat * tc.noteTimeTaken);

        if (curTime <= 1 && !doneOnce2)
        {
            curTime = Mathf.Clamp01((tc.trackPos - startTime) / (tc.noteTimeTaken * tc.secPerBeat));

            Vector3 pos;
            // Interpolate the note between the edge of the note, to the edge of the player (closest edges from eachother, based on curTime)

            pos = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, startingPos.z),
                            new Vector3(transform.position.x, transform.position.y, player.transform.position.z), curTime);
            transform.position = pos;
        }

        //newGoalTime = goalTime - startTime;


        if (curTime >= 1)
        {

            doneOnce2 = true;
            transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
            hitEnd = true;
        }

        if (doneOnce2 && !doneOnce3)
        {
            doneOnce3 = true;
            //Debug.Log("Note Landing " + tc.trackPos);
        }

        if (hitEnd)
        {
            hitEnd = false;

            Invoke("DestroyNote", (tc.secPerBeat * tc.noteTimeTaken) / 3);
        }
    }
    void DestroyNote()
    {
        Destroy(gameObject);
    }
}


