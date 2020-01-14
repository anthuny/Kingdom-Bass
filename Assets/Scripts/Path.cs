using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Renderer rend;

    [HideInInspector]
    public float pathLength;
    [HideInInspector]
    public float pathWidth;

    public int segment;
    public int laneNumber;

    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();

        pathLength = rend.bounds.size.z;
        pathWidth = rend.bounds.size.x;
        //Debug.Log(pathWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
