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
    private Gamemode gm;

    [HideInInspector]
    public GameObject initialPath;
    [HideInInspector]
    public GameObject nearestPath;
    private GameObject oldNearestPath;
    private GameObject segmentHolder;

    public int totalPaths;
    public int maxLanes;
    public int totalSegments;


        [HideInInspector]
    public List<int> laneNumbers = new List<int>();
    [HideInInspector]
    public int maxPathNumber;

    public int pathDestroySegment;

    [Range(1, 100)]
    public int spawnPathDist;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        //Assign this variable as an EGO
        segmentHolder = new GameObject();

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

        //Spawn a segment holder
        GameObject segmentHolderGO = Instantiate(segmentHolder, pathStartSpawn);
        segmentHolderGO.name = "Segment" + totalSegments.ToString();

        for (int i = 1; i <= maxLanes; i++)
        {
            // Spawn the first path
            GameObject go = Instantiate(path, new Vector3(width, 0, pathStartSpawn.transform.position.z), Quaternion.identity);

            // Set the path's parent to SegmentHolder Go
            go.transform.SetParent(segmentHolderGO.transform);

            // Add the spawned path into a list
            paths.Add(go);

            // Increase total path count 
            totalPaths = paths.Count - 1;

            // Declare the segment the spawned path belongs to
            go.GetComponent<Path>().segment = totalSegments;

            // Declare what lane number each path is
            go.GetComponent<Path>().laneNumber = i;

            // Set the name of the path
            go.name = "Path " + i;

            // Increase width
            width += initialPath.GetComponent<Path>().pathWidth;
        }

        if (nearestPath)
        {
            // Determine the largest path number in each segment
            foreach (Transform i in nearestPath.transform.parent)
            {
                laneNumbers.Add(i.GetComponent<Path>().laneNumber);
            }

            laneNumbers.Sort();
            maxPathNumber = laneNumbers[laneNumbers.Count - 1];
        }

        //Spawn note
        gm.SpawnNote();

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

            //Spawn a segment holder
            GameObject segmentHolderGO = Instantiate(segmentHolder, pathStartSpawn);
            segmentHolderGO.name = "Segment" + totalSegments.ToString();

            // Do this for the amount of lanes
            for (int i = 1; i <= maxLanes; i++)
            {
                // Spawn the path
                GameObject go = Instantiate(path, new Vector3(width, 0, pathLength * (totalSegments - 1)), Quaternion.identity);

                // Set the path's parent to SegmentHolder Go
                go.transform.SetParent(segmentHolderGO.transform);

                //Add the spawned path into a list
                paths.Add(go);

                // Increase total path count 
                totalPaths = paths.Count - 1;

                // Declare the segment the spawned path belongs to
                go.GetComponent<Path>().segment = totalSegments;

                // Declare what lane number each path is
                go.GetComponent<Path>().laneNumber = i;

                // Set the name of the path
                go.name = "Path " + i;

                // Increase the width so the lanes spawn next to the last spawned one
                width += initialPath.GetComponent<Path>().pathWidth;
            }

            // Determine the largest path number in each segment
            foreach (Transform i in nearestPath.transform.parent)
            {
                laneNumbers.Add(i.GetComponent<Path>().laneNumber);
            }

            laneNumbers.Sort();
            maxPathNumber = laneNumbers[laneNumbers.Count - 1];
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

        //float percentTravelled = (playerCurrentDist / (pathLength * totalSegments)) * 100;
        float percentTravelled = Mathf.InverseLerp(pathLength * (totalSegments - 1), (pathLength * totalSegments), playerCurrentDist)  * 100;

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

            //Remove all elements inside the pathNumbers list for the current segment
            laneNumbers.Clear();
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
            Destroy(pathToDestroy.transform.parent.gameObject);
        }
        else
        {
            return;
        }
    }
}


    

