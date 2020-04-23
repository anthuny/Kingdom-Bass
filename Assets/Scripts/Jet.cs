using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jet : MonoBehaviour
{
    public Gamemode gm;
    private GameObject player;
    private Player pScript;
    private AudioManager amScript;
    private TrackCreator tcScript;
    public Animator animator;

    [Header("Laser")]
    public Transform laserHolder;
    public LineRenderer laser;
    public float laserDecSizeSpeed;

    [Header("Laser Point")]
    public ParticleSystem laserPoint;

    private bool shooting;
    private bool decLaserSize;

    public void AssignVariables()
    {
        player = gm.player;
        pScript = gm.playerScript;
        tcScript = gm.tc;
    }
    private void Update()
    {
        CheckToAim();
        ShootVisuals();
        DecreaseLaserSize();
    }
    public void EnableJet()
    {
        gameObject.SetActive(true);

        animator = GetComponent<Animator>();

        StartCoroutine(SpawnJet());
    }

    public void DisableJet()
    {
        gameObject.SetActive(false);
    }

    public void CheckToAim()
    {
        // If a note is close enough to the player, play Jet's aim animation
        if (!pScript)
        {
            return;
        }
        if (pScript.nearestBlast)
        {
            float difference = Mathf.Abs(pScript.nearestBlast.transform.position.z - player.transform.position.z);
            Note nearestBlast = pScript.nearestBlastScript;
  
            if (difference <= tcScript.selectedMap.jetAimRange && !nearestBlast.usedForJetAim)
            {
                nearestBlast.usedForJetAim = true;
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        shooting = true;
        laser.gameObject.SetActive(true);

        // Trigger the laser point for jet
        laserPoint.gameObject.SetActive(true);

        laser.startWidth = 0;
        laser.endWidth = 0;

        gm.am.PlaySound("Jet_Aiming");

        animator.SetBool("Idle", false);
        animator.SetBool("Aim", true);

        yield return new WaitForSeconds(.25f);
        laser.startWidth = 1;
        laser.endWidth = 1;
        yield return new WaitForSeconds(.1f);
        laser.startWidth = 0;
        laser.endWidth = 0;
        yield return new WaitForSeconds(.1f);
        laser.startWidth = 1;
        laser.endWidth = 1;
        yield return new WaitForSeconds(.1f);
        laser.startWidth = 0;
        laser.endWidth = 0;
        yield return new WaitForSeconds(.5f);

        animator.SetBool("Idle", false);    
        animator.SetBool("Aim", false);
        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(.25f);

        laser.startWidth = 10;
        laser.endWidth = 10;

        decLaserSize = true;

        yield return new WaitForSeconds(.25f);

        animator.SetBool("Idle", true);

        yield return new WaitForSeconds(.7f);

        // Trigger the laser point for jet
        laserPoint.gameObject.SetActive(false);

        laser.gameObject.SetActive(false);


        decLaserSize = false;
        shooting = false;
    }
    
    void ShootVisuals()
    {
        if (shooting)
        {
            laser.SetPosition(0, laserHolder.position);
            laser.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z));
        }
    }

    void DecreaseLaserSize()
    {
        if (decLaserSize && laser.startWidth > 0)
        {
            laser.startWidth -= laserDecSizeSpeed * Time.deltaTime;
            laser.endWidth -= laserDecSizeSpeed * Time.deltaTime;
        }
    }

    public IEnumerator SpawnJet()
    {
        animator.SetTrigger("Activate");
        gm.am.PlaySound("Jet_Activate");
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(gm.lm.TurnOnLights());
    }
}

