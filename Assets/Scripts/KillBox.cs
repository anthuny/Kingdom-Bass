using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter(Collider other)
    {
        //player.GetComponent<Player>().Missed();
        other.gameObject.GetComponent<Note>().DestroyNote();
    }
}
