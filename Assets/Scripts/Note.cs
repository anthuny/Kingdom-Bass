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
     
    public GameObject leftWall, rightWall, forwardWall;

    bool hitEnd;

    private float wallLengths;
    private Renderer rend;

    public Vector3 playerPos;

    private float t;


    private float startTime;
    private float goalTime;
    private float newGoalTime;
    private float curTime = 0;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();

        leftWall.SetActive(false);
        rightWall.SetActive(false);
        forwardWall.SetActive(false);

        // Declare wallLengths to be the z value of the leftWall gameobject
        rend = leftWall.GetComponent<Renderer>();
        wallLengths = rend.bounds.size.z;

        // Set the size of the wall to be based off the noteTimeToArriveMult (the smaller the value, the larger the walls).
        wallLengths /= tc.beatsShownInAdvance / 2;

        Vector3 scale = leftWall.transform.localScale;
        scale.z = wallLengths;
        leftWall.transform.localScale = scale;
        rightWall.transform.localScale = scale;


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

                rightWall.SetActive(true);
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

                leftWall.SetActive(true);
                break;

            case "up":
                Sprite upArrow = Resources.Load<Sprite>("Sprites/T_UpArrow") as Sprite;
                    
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = upArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.upArrowC;
                forwardWall.SetActive(true);
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
            //Debug.Log("Note spawned " + Time.frameCount);

            startTime = tc.trackPos;
            //Debug.Log("startTime is " + startTime);
            goalTime = (startTime + tc.secPerBeat) * tc.beatsShownInAdvance;
            //Debug.Log("goalTime is " + goalTime);
            //curTime = startTime;
        }
        // Calculate the percantage of completion of the note on the lane
        percDistance = Mathf.Abs((transform.position.z - player.transform.position.z) / pm.pathLength) * 100;

        // Determine the speed the note needs to use to get to the player on the beat
        //gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);
        gm.noteSpeed = pm.pathLength / (tc.secPerBeat * tc.beatsShownInAdvance);

        if (t <= 1)
        {
            Vector3 pos;
            pos = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, startingPos.z),
                            new Vector3(transform.position.x, transform.position.y, player.transform.position.z), t);
            transform.position = pos;
        }


        newGoalTime = goalTime - startTime;

        if (curTime <= newGoalTime)
        {
            curTime += Time.deltaTime;
            t = curTime / newGoalTime;
        }

        if (t >= 1)
        {

            doneOnce2 = true;
            transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
            hitEnd = true;
        }

        if (doneOnce2 && !doneOnce3)
        {
            Debug.Log(newGoalTime);
            doneOnce3 = true;
            Debug.Log("Note Landing frame count " + Time.frameCount);
        }

        if (hitEnd)
        {
            hitEnd = false;

            Invoke("DestroyNote", (tc.secPerBeat * tc.beatsShownInAdvance) / 3);
        }
    }
    void DestroyNote()
    {
        Destroy(gameObject);
    }
}


