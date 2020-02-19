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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "kill box trg");
        other.gameObject.GetComponent<Note>().DestroyNote();
    }
}
