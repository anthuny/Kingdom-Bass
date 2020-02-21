using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Renderer rend;

    [HideInInspector]
    public float pathLength;

    public float pathWidth;

    public int segment;
    public int laneNumber;

    public GameObject leftDivider;
    public GameObject rightDivider;
    public GameObject[] leftDividers;
    public GameObject[] rightDividers;
    public GameObject leftBeam;
    public GameObject rightBeam;

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
