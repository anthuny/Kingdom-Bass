using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    private Gamemode gm;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<Gamemode>();

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Adding movement in the forward direction of the player
        //rb.AddForce(Vector3.forward * speed);
        rb.velocity = new Vector3(0, 0, 1) * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Hit")
        {
            //Jet Shoots at player
            gm.jet.GetComponent<Jet>().Shoot();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Hit")
        {
            //Player hitting the hit
            if (Input.GetKey(KeyCode.Space))
            {
                // Spawn hit particle effect
                Instantiate(gm.jet.GetComponent<Jet>().hitParticle, other.transform.position, other.transform.rotation);
            }
        }
    }
}