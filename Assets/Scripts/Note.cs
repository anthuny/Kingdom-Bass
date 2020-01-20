using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private TrackCreator tc;
    private PathManager pm;
    private Gamemode gm;
    public int beatOfThisNote;
    public bool online;
    public Vector3 startingPos;
    public float pathWidth;
    public bool canMove;
    private Player player;


    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PathManager>();
        tc = FindObjectOfType<TrackCreator>();
        gm = FindObjectOfType<Gamemode>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Determine the speed the note needs to use to get to the player on the beat
        gm.noteSpeed = pm.pathLength / (tc.timeToWait * tc.noteTimeToArriveMult);

        // Move the note toward the player
        transform.position += -transform.forward * Time.deltaTime * gm.noteSpeed;
       
        //Destroy the note when it reaches the player
        if (transform.position.z <= player.gameObject.transform.position.z)
        {
            Destroy(gameObject);
        }
    }
   
}
