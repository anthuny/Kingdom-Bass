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
    public Vector3 startingPos;
    [HideInInspector]
    public float pathWidth;
    [HideInInspector]
    private Player player;

    public string arrowDir;


    public int laneNumber;
      

    public GameObject leftWall, rightWall, forwardWall;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();

        Sprite leftArrow = Resources.Load<Sprite>("Sprites/T_LeftArrow") as Sprite;
        Sprite rightArrow = Resources.Load<Sprite>("Sprites/T_RightArrow") as Sprite;
        Sprite upArrow = Resources.Load<Sprite>("Sprites/T_UpArrow") as Sprite;

        leftWall.SetActive(false);
        rightWall.SetActive(false);
        forwardWall.SetActive(false);

        //Determine the direction of the arrow on the note
        switch (arrowDir)
        {
            case "left":
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = leftArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalArrowC;
                rightWall.SetActive(true);
                break;

            case "right":
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = rightArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.horizontalArrowC;
                leftWall.SetActive(true);
                break;

            case "up":
                transform.GetChild(1).GetComponentInChildren<Image>().sprite = upArrow;
                transform.GetChild(1).GetComponentInChildren<Image>().color = gm.upArrowC;
                forwardWall.SetActive(true);
                break;

            default:
                Debug.Log(this.gameObject.name + "does not have an arrow direction");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Determine the speed the note needs to use to get to the player on the beat
        //gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);
        gm.noteSpeed = pm.pathLength / (tc.secPerBeat * tc.noteTimeToArriveMult);

        // Move the note toward the player
        transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
       
        //Destroy the note when it reaches the player
        if (transform.position.z <= player.gameObject.transform.position.z)
        {
            Destroy(gameObject);
        }
    }
   
}
