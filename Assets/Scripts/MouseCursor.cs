using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    public Gamemode gm;
    public Vector3 vector;
    public Vector3 oldVector;
    public bool doneOnce;
    public bool doneOnce2;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!doneOnce2)
        {
            StartCoroutine("UpdateMousePos");
        }


        vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        transform.position = vector;
    }

    IEnumerator UpdateMousePos()
    {
        doneOnce2 = true;
        yield return new WaitForSeconds(.2f);

        if (gm.controllerConnected)
        {
            if (oldVector != vector)
            {
                image.enabled = true;
                if (!doneOnce)
                {
                    StartCoroutine("mousePos");
                }
            }
        }
        else
        {
            image.enabled = true;
        }
        oldVector = vector;


        doneOnce2 = false;
    }

    IEnumerator mousePos()
    {
        doneOnce = true;

        yield return new WaitForSeconds(1f);

        if (oldVector == vector)
        {
            image.enabled = false;
        }
        doneOnce = false;
    }
}
