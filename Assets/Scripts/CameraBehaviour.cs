﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Gamemode gm;
    bool doneOnce;
    bool retracting;
    string direction;
    Vector3 rot;
    float t;
    float t2;

    public iTween.EaseType easeType;
    public iTween.LoopType loopType;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        rot.x = transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator RollCamera(string dir)
    {
        doneOnce = true;
        direction = dir;
        if (dir == "left")
        {
            iTween.RotateTo(this.gameObject, iTween.Hash("z", -gm.launchRotAmount, "time", gm.launchRotTime, "easytype", easeType, "looptype", loopType));
            yield return new WaitForSeconds(gm.launchRotTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("z", 0, "time", gm.launchRotTime * 3, "easytype", easeType, "looptype", loopType));
        }

        else if (dir == "right")
        {
            iTween.RotateTo(this.gameObject, iTween.Hash("z", gm.launchRotAmount, "time", gm.launchRotTime, "easytype", easeType, "looptype", loopType));
            yield return new WaitForSeconds(gm.launchRotTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("z", 0, "time", gm.launchRotTime * 3, "easytype", easeType, "looptype", loopType));
        }

    }
}