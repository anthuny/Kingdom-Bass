using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("killbox works");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision + " kill box col");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "kill box trg");
    }
}
