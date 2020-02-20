using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteAccuracy : MonoBehaviour
{
    public GameObject player;
    public GameObject perfect;
    public GameObject great;
    public GameObject good;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        perfect = GameObject.FindGameObjectWithTag("PERFECT"); 
        great = GameObject.FindGameObjectWithTag("GREAT");
        good = GameObject.FindGameObjectWithTag("GOOD");
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
