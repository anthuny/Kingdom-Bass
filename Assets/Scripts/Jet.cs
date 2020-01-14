using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jet : MonoBehaviour
{
    [HideInInspector]
    public Transform aimHolder;
    [HideInInspector]
    public Transform shootHolder;
    private GameObject player;
    private float shootCD;
    private Vector3 aimPos;
    private Vector3 shootPos;

    public GameObject aimLaser;
    public GameObject shootLaser;
    public GameObject hit;

    public float laserAimSpeed;
    public float laserShootSpeed;
    public float laserForwardInf;
    public float shootCDMin;
    public float shootCDMax;
    public bool shooting;
    public ParticleSystem hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        aimHolder = transform.Find("AimHolder");
        shootHolder = transform.Find("ShootHolder");

        // Get shoot cooldown before shooting once, so laser
        // doesn't spawn instantly when game starts  

        //StartCoroutine(GetShootCooldown());
    }

    void Aim()
    {
        //Rotate gunholder to look at aim position
        aimPos = player.transform.position + (player.GetComponent<Player>().rb.velocity) * laserForwardInf;

        // Look at player
        aimHolder.transform.LookAt(aimPos);

        // Spawn a laser
        GameObject go = Instantiate(aimLaser, aimHolder.position, Quaternion.identity);
        go.GetComponent<Laser>().player = player.transform;
        go.GetComponent<Laser>().jet = GetComponent<Jet>();
        go.transform.rotation = aimHolder.rotation;

        // Get new shot cooldown
        StartCoroutine(GetShootCooldown());
    }

    public void Shoot()
    {
        //Rotate gunholder to look at aim position
        shootPos = player.transform.position;

        // Look at player
        shootHolder.transform.LookAt(shootPos);

        // Spawn a laser
        GameObject go2 = Instantiate(shootLaser, shootHolder.position, Quaternion.identity);
        go2.GetComponent<Laser>().player = player.transform;
        go2.GetComponent<Laser>().jet = GetComponent<Jet>();
        go2.transform.rotation = shootHolder.rotation;
        go2.GetComponent<Laser>().isLaser = true;
    }


    IEnumerator GetShootCooldown()
    {
        shootCD = Random.Range(shootCDMin, shootCDMax);

        yield return new WaitForSeconds(shootCD);

        Aim();
    }
}
