using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Renderer rend;

    [HideInInspector]
    public float pathLength;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();

        pathLength = rend.bounds.size.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
