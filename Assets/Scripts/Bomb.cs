using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Gamemode gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BombHit()
    {
        Debug.Log("bomb hit");
        Debug.Break();
    }
}
