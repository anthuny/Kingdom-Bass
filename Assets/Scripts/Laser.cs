using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [HideInInspector]
    public Transform player;
    private Rigidbody rb;

    // Aim bot
    private Vector3 aimPos;
    private Vector3 dir;
    private Vector3 startingPos;
    public bool isLaser;

    public Jet jet;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        startingPos = transform.position;
        LookAtPlayer();
    }

    private void Update()
    {
        Launch();
    }
    void LookAtPlayer()
    {

    }

    void Launch()
    {
        // if not the real laser, (just the aim) aim forward of the player
        if (!isLaser)
        {
            // Move towards player 
            aimPos = player.position + (player.GetComponent<Player>().rb.velocity * jet.laserForwardInf);
            Vector3 dir = aimPos - startingPos;
            dir.Normalize();
            Vector3 pos;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            pos.z = transform.position.z;

            pos.x += dir.x * jet.laserAimSpeed * Time.deltaTime;
            pos.y += dir.y * jet.laserAimSpeed * Time.deltaTime;
            pos.z += dir.z * jet.laserAimSpeed * Time.deltaTime;

            transform.position = pos;
        }

        // If is real laser, move directly towards the player
        if (isLaser)
        {
            // Move towards player 
            aimPos = player.position;
            Vector3 dir = aimPos - transform.position;
            dir.Normalize();
            Vector3 pos;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            pos.z = transform.position.z;

            pos.x += dir.x * jet.laserShootSpeed * Time.deltaTime;
            pos.y += dir.y * jet.laserShootSpeed * Time.deltaTime;
            pos.z += dir.z * jet.laserShootSpeed * Time.deltaTime;

            transform.position = pos;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Path")
        {
            GameObject go = Instantiate(jet.hit, transform.position, Quaternion.identity);
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            Vector3 pos;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            pos.z = transform.position.z;

            pos.x = player.gameObject.transform.position.x;
            pos.y = player.gameObject.GetComponent<Player>().nearestPath.transform.position.y + 0.01f;
            go.transform.position = pos;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If laser hits the player, destroy the laser
        if (other.transform.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

}
