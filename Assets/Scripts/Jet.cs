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
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AssignVariables()
    {
        player = gm.player;
        pScript = gm.playerScript;
        tcScript = gm.tc;
    }
    private void Update()
    {
        CheckToAim();
    }
    public void EnableJet()
    {
        gameObject.SetActive(true);
    }

    public void DisableJet()
    {
        gameObject.SetActive(false);
    }

    public void CheckToAim()
    {
        // If a note is close enough to the player, play Jet's aim animation
        if (pScript.nearestBlast)
        {
            float difference = Mathf.Abs(pScript.nearestBlast.transform.position.z - player.transform.position.z);
            Note nearestBlast = pScript.nearestBlastScript;
            float bigMath = (gm.noteDistForAim * (1 + (1 - ((tcScript.selectedMap.noteTimeTaken - ((1 - (tcScript.selectedMap.noteTimeTaken / 7))) * 6) / 7))) * (1 + ((1 * (tcScript.selectedMap.bpm / 128)))));
            if (difference <= bigMath && !nearestBlast.usedForJetAim)
            {
                nearestBlast.usedForJetAim = true;
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        gm.am.PlaySound("Jet_Aiming");

        animator.SetBool("Idle", false);
        animator.SetBool("Aim", true);

        yield return new WaitForSeconds(1.05f);

        animator.SetBool("Idle", false);    
        animator.SetBool("Aim", false);
        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(.5f);

        animator.SetBool("Idle", true);
    }
}

