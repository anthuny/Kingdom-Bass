using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Player player;
    public List<GameObject> paths = new List<GameObject>();
    public GameObject path;
    public Transform pathStartSpawn;
    public Vector3 initialPathSpawnLoc;
    float pathLength;
    bool justSpawnedPaths;
    private GameObject pathToDestroy;

    private GameObject initialPath;
    private GameObject nearestPath;
    private GameObject oldNearestPath;

    public int totalPaths;
    public int maxLanes;
    public int totalSegments;

    public int pathDestroySegment;

    [Range(1, 100)]
    public int spawnPathDist;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        initialPath = Instantiate(path, initialPathSpawnLoc, Quaternion.identity);
        initialPath.name = "initialPath DEBUG";

        SpawnFirstPath();

        //Set old nearest path to the first path that spawns
        oldNearestPath = paths[0];
    }

    private void Update()
    {
        SpawnPaths();
        FindNearestPath();
        DestroyPathsBehind();
    }

    // Spawn the first path
    void SpawnFirstPath()
    {
        float width = 0;

        // Increase the amount of segments
        totalSegments++;

        for (int i = 1; i <= maxLanes; i++)
        {
            // Spawn the first path
            GameObject go = Instantiate(path, new Vector3(width, 0, pathStartSpawn.transform.position.z), Quaternion.identity);

            // Add the spawned path into a list
            paths.Add(go);

            // Declare the segment the spawned path belongs to
            go.GetComponent<Path>().segment = totalSegments;

            // Increase total path count 
            totalPaths = paths.Count - 1;

            // Increase width
            width += initialPath.GetComponent<Path>().pathWidth;
        }


    }

    void SpawnPaths()
    {
        // If the player passes a certain distance on a segment
        // Spawn more segments in front of the segement they are on.
        if (TrackPlayerDistance() > spawnPathDist && !justSpawnedPaths)
        {
            justSpawnedPaths = true;
            float width = 0;
            pathLength = paths[totalPaths].GetComponent<Path>().pathLength;

            // Increase the amount of segments
            totalSegments++;

            // Do this for the amount of lanes
            for (int i = 1; i <= maxLanes; i++)
            {
                // Spawn the path
                GameObject go = Instantiate(path, new Vector3(width, 0, pathLength * (totalSegments - 1)), Quaternion.identity);

                //Add the spawned path into a list
                paths.Add(go);

                // Declare the segment the spawned path belongs to
                go.GetComponent<Path>().segment = totalSegments;

                // Increase total path count 
                totalPaths = paths.Count - 1;

                // Increase the width so the lanes spawn next to the last spawned one
                width += initialPath.GetComponent<Path>().pathWidth;
            }


        }
    }

    // Determine the distance travelled on the current segment of path
    // as a percentage
    float TrackPlayerDistance()
    {

        if (!nearestPath)
        {
            return 0;
        }

        float pathLength = nearestPath.GetComponent<Path>().pathLength;
        float playerCurrentDist = player.transform.position.z;
        float percentTravelled = (playerCurrentDist / (pathLength * totalSegments)) * 100;

        return percentTravelled;
    }

    void FindNearestPath()
    {
        //Find the path the player is on
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            //Debug.DrawRay(player.transform.position, Vector3.down, Color.green);
            nearestPath = hit.collider.gameObject;
        }
        else
        {
            //Debug.DrawRay(player.transform.position, Vector3.down, Color.red);
            return;
        }

        // When the player goes onto a different segment from the last
        // Reset the percentage tracker for distance
        if (oldNearestPath != nearestPath)
        {
            oldNearestPath = nearestPath;
            justSpawnedPaths = false;
        }
    }

    void DestroyPathsBehind()
    {
        // Send a raycast x amounts of segments behind where the player currently is.
        // Find the segment number of that path
        // If the difference between that path's segment, and the player's nearest path
        // segment is larger then x. Destroy all paths in that behind segment
        //Find the path the player is on

        pathLength = paths[totalPaths].GetComponent<Path>().pathLength;

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - (pathLength * pathDestroySegment)), Vector3.down, out hit))
        {
            pathToDestroy = hit.collider.gameObject;

            // Send a RayCast Right of this hit path
            RaycastHit hit2;
            if (Physics.Raycast(pathToDestroy.transform.position, Vector3.right, out hit2, 10))
            {
                Destroy(hit2.collider.gameObject);
            }

            // Send a RayCast Left of this hit path
            RaycastHit hit3;
            if (Physics.Raycast(pathToDestroy.transform.position, Vector3.left, out hit3, 10))
            {
                Destroy(hit3.collider.gameObject);
            }

            Invoke("DestroyImpossiblePath", 0.05f);
        }
        else
        {
            return;
        }
    }

    void DestroyImpossiblePath()
    {
        Destroy(pathToDestroy);
    }
}


    

