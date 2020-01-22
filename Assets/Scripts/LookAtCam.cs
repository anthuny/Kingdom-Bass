using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    Player player;
    Gamemode gm;
    PathManager pm;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<Gamemode>();
        pm = FindObjectOfType<PathManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only if the note is far enough from the player, will the arrow look at the camera's position.
        if ((gm.distPercArrowLock / pm.pathLength) * 100 < transform.parent.GetComponent<Note>().percDistance)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }

    }
}
