using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Gamemode gm;
    Vector3 rot;

    public iTween.EaseType easeType;
    public iTween.LoopType loopType;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        rot.x = transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update() // Needed?
    {

    }

    public IEnumerator RollCamera(string dir) //what is using this enum?
    {
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

    public void Test() //UHHHHHHH??????? ok??
    {
        Debug.Log("wtf");
    }
}
