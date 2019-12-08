using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    [SerializeField]
    public GameObject nearestPath;
    private GameObject oldNearestPath;
    private Gamemode gm;
    private PathManager pm;
    private Vector3 dir = new Vector3(0, -1, 0);
    private float dist = 10;
    public bool spawnedPath;

    public float speed;
    [Range(0f, 100f)]
    public float pathPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = FindObjectOfType<PathManager>();
        gm = FindObjectOfType<Gamemode>();

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        DetectPathTrigger();
        DetectNearestPath();
        AllowPathToSpawn();
    }

    void AllowPathToSpawn()
    {
        if (nearestPath == oldNearestPath)
        {
            spawnedPath = true;
        }
        else
        {
            spawnedPath = false;
        }
    }

    void Movement()
    {
        // Adding movement in the forward direction of the player
        //rb.AddForce(Vector3.forward * speed);
        rb.velocity = new Vector3(0, 0, 1) * speed;
    }

    void DetectNearestPath()
    {
        RaycastHit hit;
        //Detect the nearest path and store it in variable
        Physics.Raycast(transform.position, dir, out hit, dist);
        
        if (hit.collider.gameObject.tag == "Path")
        {
            nearestPath = hit.collider.gameObject;
        }
    }

    void DetectPathTrigger()
    {
        if (nearestPath && !spawnedPath)
        {
            if (nearestPath != oldNearestPath)
            {
                float totalPaths = pm.totalPaths + 1;
                //Detect percentage of how far player has gone on the nearestpath
                float pathStartPos = totalPaths * nearestPath.GetComponent<Path>().pathLength - nearestPath.GetComponent<Path>().pathLength;
                float pathEndPos = nearestPath.GetComponent<Path>().pathLength;

                float pathPercentage = (((transform.position.z - pathStartPos) / pathEndPos) * 100);
                //Debug.Log(pathPercentage);

                // Check if the player reaches a far enough distance
                // on a path, for another one to spawn
                if (pathPercentage >= pathPoint)
                {
                    spawnedPath = true;

                    // Spawn a path 
                    pm.SpawnPath();

                    oldNearestPath = nearestPath;
                }
            }
            
        }

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