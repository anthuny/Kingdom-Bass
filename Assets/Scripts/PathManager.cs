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
    [HideInInspector]
    public float pathLength;
    private GameObject pathToDestroy;
    private Gamemode gm;

    [HideInInspector]
    public GameObject initialPath;
    public GameObject nearestPath;
    private GameObject oldNearestPath;
    private GameObject segmentHolder;
    public GameObject currentSegment;

    public int totalPaths;
    public int maxLanes;
    public GameObject middleLane;
    public int totalSegments;


    [HideInInspector]
    public List<int> laneNumbers = new List<int>();
    [HideInInspector]
    public int maxPathNumber;

    public int pathDestroySegment;

    [Range(1, 100)]
    public int spawnPathDist;

    private float t;
    private float n;
    private bool spawningPaths;

    public GameObject underPathsBlock;

    // Start is called before the first frame update

    private void Awake()
    {
        initialPath = Instantiate(path, initialPathSpawnLoc, Quaternion.identity);
        initialPath.name = "initialPath DEBUG";
    }

    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        //Assign this variable as an EGO
        segmentHolder = new GameObject();

        player = FindObjectOfType<Player>();


        SpawnFirstPath();

        underPathsBlock.SetActive(true);
    }

    void BeginPathSpawn()
    {
        // If all lanes have been spawned
        if (spawningPaths)
        {
            // Itterate through each one
            for (int i = 0; i < paths.Count; i++)
            {
                // Increase the opacity and the player's sight distance on it over time
                paths[i].transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Vector1_7E903828", t);
                paths[i].transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Vector1_58090123", n);
            }
        }

    }

    void ElapsePathSpawn()
    {
        if (!spawningPaths)
        {
            return;
        }

        // If the lane's opacity has not reached it's maximum value, continue until it does.
        if (t <= gm.maxPathOpacity)
        {
            t += Time.deltaTime * gm.pathOpacityIncSpeed;
        }

        // If the player's lane view distance has not reached it's maximum value, continue until it does.
        if (n <= gm.maxPathViewDist)
        {
            n += Time.deltaTime * gm.pathViewDistIncSpeed;
        }
    }

    private void Update()
    {
        if (gm.tutPaused)
        {
            return;
        }
        //FindNearestPath();
        ElapsePathSpawn();
        BeginPathSpawn();
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

            if (i == Mathf.Floor(maxLanes / 2))
            {
                middleLane = go;
                player.GetComponent<Player>().RepositionPlayer();

            }

            //Set the colour of the spotlight
            //Color.RGBToHSV(gm.lane1Color, out H, out S, out V);

            if (i == 1)
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("Color_D4874C26", gm.lane1Color);
            }

            if (i == 2)
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("Color_D4874C26", gm.lane2Color);
            }

            if (i == 3)
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("Color_D4874C26", gm.lane3Color);
            }

            if (i == 4)
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("Color_D4874C26", gm.lane2Color);
            }

            if (i == 5)
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("Color_D4874C26", gm.lane1Color);
            }

            // After all lanes have spawned (all at 0 opacity) allow them to begin to increase in opacity
            GameObject allPaths = FindObjectOfType<Path>().gameObject;
            allPaths.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Vector1_7E903828", 0);

            spawningPaths = true;

            // If this path is NOT the last path
            if (i != maxLanes)
            {
                // Disable the right divider of the path
                go.GetComponent<Path>().rightDivider.SetActive(false);
            }

            // If this path is the LAST path
            else
            {
                // enable the right divider of the path
                go.GetComponent<Path>().rightDivider.SetActive(true);

                go.GetComponent<Path>().rightBeam.transform.localPosition = new Vector3(0.75f, 2.082f, 5.135f);
                go.GetComponent<Path>().rightBeam.transform.localScale = new Vector3(0.13f, 5f, 0.13f);

                // Iterate through all right divider visuals and make them all larger in scale + position them differently
                for (int x = 0; x < go.GetComponent<Path>().rightDividers.Length; x++)
                {
                    go.GetComponent<Path>().rightDividers[x].transform.localScale = new Vector3(2.2f, 1, 1);
                    go.GetComponent<Path>().rightDividers[x].GetComponent<RectTransform>().localPosition = new Vector3(-12, -1.73f, 0);
                    go.GetComponent<Path>().rightDividers[x].GetComponent<RectTransform>().localScale = new Vector3(2.2f, 1.2f, 1);
                }
            }

            // If this path is the FIRST path
            if (i == 1)
            {
                // enable the right divider of the path
                go.GetComponent<Path>().leftDivider.SetActive(true);

                go.GetComponent<Path>().leftBeam.transform.localPosition = new Vector3(2.326f, 2.082f, 5.135f);
                go.GetComponent<Path>().leftBeam.transform.localScale = new Vector3(0.13f, 5f, 0.13f);

                // Iterate through all left divider visuals and make them all larger in scale + position them differently + scale them differently
                for (int x = 0; x < go.GetComponent<Path>().leftDividers.Length; x++)
                {
                    go.GetComponent<Path>().leftDividers[x].transform.localScale = new Vector3(2.2f, 1, 1);
                    go.GetComponent<Path>().leftDividers[x].GetComponent<RectTransform>().localPosition = new Vector3(-12, -1.73f, 0);
                    go.GetComponent<Path>().leftDividers[x].GetComponent<RectTransform>().localScale = new Vector3(2.2f, 1.2f, 1);
                }
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

        //float percentTravelled = (playerCurrentDist / (pathLength * totalSegments)) * 100;
        float percentTravelled = Mathf.InverseLerp(pathLength * (totalSegments - 1), (pathLength * totalSegments), playerCurrentDist)  * 100;

        return percentTravelled;
    }

    public void FindNearestPath(bool forScore)
    {
        //Debug.Log("findnearestpath function started");
        //Debug.Break();
        //Find the path the player is on
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            if (hit.transform.CompareTag("Path"))
            {
                //Debug.DrawRay(player.transform.position, Vector3.down, Color.green);
                nearestPath = hit.collider.gameObject;
                //Debug.Log("assigned nearestlane number");
                //Debug.Break();
                player.SetNearestLaneNumber(forScore, false, nearestPath);

            }
        }
        else
        {
            //Debug.DrawRay(player.transform.position, Vector3.down, Color.red);
            return;
        }

        // Search for the nearest path
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
    }
}


    

