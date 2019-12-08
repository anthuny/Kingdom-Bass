using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Player player;
    public List<GameObject> paths = new List<GameObject>();
    public GameObject path;
    public Transform pathStartSpawn;

    public int totalPaths;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        SpawnFirstPath();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void SpawnFirstPath()
    {
        // Spawn the first path
        GameObject go = Instantiate(path, pathStartSpawn.position, Quaternion.identity);

        //Add the spawned path into a list
        paths.Add(go);

        // Increase total path count 
        totalPaths = paths.Count - 1;
        //Debug.Log(totalPaths);
    }

    // Spawn paths only when allowed to
    public void SpawnPath()
    {
        //Debug.Log(paths[totalPaths]);
        // Spawn path
        GameObject go = Instantiate(path, paths[totalPaths].transform.position + new Vector3 (0,0,paths[totalPaths].GetComponent<Path>().pathLength), Quaternion.identity);

        totalPaths = paths.Count;

        //Add the spawned path into a list
        paths.Add(go);
    }
}
