using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    Player player;
    Gamemode gm;
    PathManager pm;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.parent.name != "Player")
        {
            // Only if the note is far enough from the player, will the arrow look at the camera's position.
            if ((gm.distPercArrowLock / pm.pathLength) * 100 < transform.parent.GetComponent<Note>().percDistance)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
            }
        }

        else
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }
    }
}
