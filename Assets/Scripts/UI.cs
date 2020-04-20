using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Gamemode gm;
    public bool selected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Selected()
    {
        foreach (Button b in gm.mapButtons)
        {
            if (b.gameObject.GetComponent<UI>() != this)
            {
                b.GetComponent<UI>().selected = false;
            }
        }

        if (!selected)
        {
            selected = true;
        }
        else
        {
            gm.tc.LoadTrack();
        }
    }
}
