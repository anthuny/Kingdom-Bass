using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightManager : MonoBehaviour
{
    public Gamemode gm;
    public List<GameObject> allLights = new List<GameObject>();

    public GameObject fanLight;
    [HideInInspector]
    public Animator fanLightAnimator;
    public List<GameObject> jetBackLights = new List<GameObject>();

    [Header("Jet Back Lights")]
    public float elapsedTimeChange;
    [HideInInspector]
    private Animator jetBackLightAAnimator;
    [HideInInspector]
    private Animator jetBackLightBAnimator;
    ShowLaserEffect jetBackLightA;
    ShowLaserEffect jetBackLightB;

    [Header("Side Lights")]
    public List<GameObject> sideLights = new List<GameObject>();
    private Animator sideLightAAnimator;
    private Animator sideLightBAnimator;
    ShowLaserEffect sideLightAScript;
    ShowLaserEffect sideLightBScript;

    [Header("Jet Back Fan Light")]
    public float panSpeed;
    public float fanFadeOutSpeed = .03f;
    [HideInInspector]
    public Coroutine activeCoroutine;
    ShowLaserEffect fanScript;

    public bool turningOffFanLight;
    public bool turningOffJetBackLight;

    [Header("BG Light")]
    public GameObject bgLight;
    ShowLaserEffect bgLightScript;
    private Animator bgLightAnimator;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allLights.Count; i++)
        {
            allLights[i].SetActive(false);
        }

        fanLightAnimator = fanLight.GetComponent<Animator>();
        fanScript = fanLight.GetComponent<ShowLaserEffect>();

        jetBackLightA = jetBackLights[0].GetComponent<ShowLaserEffect>();
        jetBackLightAAnimator = jetBackLights[0].GetComponent<Animator>();

        jetBackLightB = jetBackLights[1].GetComponent<ShowLaserEffect>();
        jetBackLightBAnimator = jetBackLights[1].GetComponent<Animator>();

        sideLightAScript = sideLights[0].GetComponent<ShowLaserEffect>();
        sideLightAAnimator = sideLights[0].GetComponent<Animator>();

        sideLightBScript = sideLights[1].GetComponent<ShowLaserEffect>();
        sideLightBAnimator = sideLights[1].GetComponent<Animator>();

        bgLightAnimator = bgLight.GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateFan();
        TurnOffLight();
    }

    public void DisableAllLights()
    {
        for (int i = 0; i < allLights.Count; i++)
        {
            allLights[i].SetActive(false);
        }
    }
    public void TriggerFan(bool power, string dir)
    {
        for (int i = 0; i < sideLights.Count; i++)
        {
            sideLights[i].SetActive(false);
        }

        for (int i = 0; i < jetBackLights.Count; i++)
        {
            jetBackLights[i].SetActive(false);
        }

        fanLight.SetActive(true);
        
        if (dir == "left")
        {
            panSpeed = 20;
        }

        else if (dir == "right")
        {
            panSpeed = -20;
        }

        else if (dir == "straight")
        {
            panSpeed = 0;
        }
    }

    void UpdateFan()
    {
        if (fanLight.activeSelf)
        {
            fanLight.transform.parent.transform.Rotate(new Vector3(0, 0, panSpeed) * Time.deltaTime);
        }
    }

    public IEnumerator TurnOnLights()
    {
        bgLight.SetActive(true);
        bgLightAnimator.SetTrigger("TurnOn");
        gm.am.PlaySound("Light_BG_On");

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < jetBackLights.Count; i++)
        {
            jetBackLights[i].SetActive(true);
            gm.am.PlaySound("Light_Def_On");
        }

        yield return new WaitForSeconds(.8f);

        for (int i = 0; i < sideLights.Count; i++)
        {
            sideLights[i].SetActive(true);
            gm.am.PlaySound("Light_Def_On");

            yield return new WaitForSeconds(.2f);
        }
    }

    public void TurnOffLight()
    {
        if (turningOffFanLight)
        {
            fanScript.fadeOutValue += fanFadeOutSpeed * Time.deltaTime;
            if (fanScript.fadeOutValue >= 1)
            {
                fanScript.fadeOutValue = 1;
                fanLight.SetActive(false);
                fanScript.fadeOutValue = 0;
                turningOffFanLight = false;
            }
        }
    }

    public IEnumerator JetLights(string noteType, string noteDir)
    {
        if (noteDir == "up" && noteType == "note")
        {
            jetBackLightAAnimator.SetTrigger("HitUpNote");
            jetBackLightBAnimator.SetTrigger("HitUpNote");

            sideLightAAnimator.SetTrigger("HitUpNote");
            sideLightBAnimator.SetTrigger("HitUpNote");

            yield return new WaitForSeconds(elapsedTimeChange * 1.5f);

            jetBackLightAAnimator.SetTrigger("FadeOutUpNote");
            jetBackLightBAnimator.SetTrigger("FadeOutUpNote");

            sideLightAAnimator.SetTrigger("FadeOutUpNote");
            sideLightBAnimator.SetTrigger("FadeOutUpNote");

            yield return new WaitForSeconds(elapsedTimeChange * 1.5f);

            jetBackLightAAnimator.SetTrigger("Idle");
            jetBackLightBAnimator.SetTrigger("Idle");

            sideLightAAnimator.SetTrigger("Idle");
            sideLightBAnimator.SetTrigger("Idle");
            yield break;
        }
        switch (noteType)
        {
            case "launch":
                jetBackLightAAnimator.SetTrigger("HitLaunch");
                jetBackLightBAnimator.SetTrigger("HitLaunch");

                sideLightAAnimator.SetTrigger("HitLaunch");
                sideLightBAnimator.SetTrigger("HitLaunch");

                yield return new WaitForSeconds(elapsedTimeChange * 1.5f);

                jetBackLightAAnimator.SetTrigger("FadeOutLaunch");
                jetBackLightBAnimator.SetTrigger("FadeOutLaunch");

                sideLightAAnimator.SetTrigger("FadeOutLaunch");
                sideLightBAnimator.SetTrigger("FadeOutLaunch");

                yield return new WaitForSeconds(elapsedTimeChange * 1.5f);

                jetBackLightAAnimator.SetTrigger("Idle");
                jetBackLightBAnimator.SetTrigger("Idle");

                sideLightAAnimator.SetTrigger("Idle");
                sideLightBAnimator.SetTrigger("Idle");
                break;

            case "note":
                jetBackLightAAnimator.SetTrigger("HitNote");
                jetBackLightBAnimator.SetTrigger("HitNote");

                sideLightAAnimator.SetTrigger("HitNote");
                sideLightBAnimator.SetTrigger("HitNote");

                yield return new WaitForSeconds(elapsedTimeChange);

                jetBackLightAAnimator.SetTrigger("FadeOutNote");
                jetBackLightBAnimator.SetTrigger("FadeOutNote");

                sideLightAAnimator.SetTrigger("FadeOutNote");
                sideLightBAnimator.SetTrigger("FadeOutNote");

                yield return new WaitForSeconds(elapsedTimeChange);

                jetBackLightAAnimator.SetTrigger("Idle");
                jetBackLightBAnimator.SetTrigger("Idle");

                sideLightAAnimator.SetTrigger("Idle");
                sideLightBAnimator.SetTrigger("Idle");
                break;

            case "blast":
                jetBackLightAAnimator.SetTrigger("HitBlast");
                jetBackLightBAnimator.SetTrigger("HitBlast");

                sideLightAAnimator.SetTrigger("HitBlast");
                sideLightBAnimator.SetTrigger("HitBlast");

                yield return new WaitForSeconds(elapsedTimeChange * 2);

                jetBackLightAAnimator.SetTrigger("FadeOutBlast");
                jetBackLightBAnimator.SetTrigger("FadeOutBlast");

                sideLightAAnimator.SetTrigger("FadeOutBlast");
                sideLightBAnimator.SetTrigger("FadeOutBlast");

                yield return new WaitForSeconds(elapsedTimeChange * 2);

                jetBackLightAAnimator.SetTrigger("Idle");
                jetBackLightBAnimator.SetTrigger("Idle");

                sideLightAAnimator.SetTrigger("Idle");
                sideLightBAnimator.SetTrigger("Idle");
                break;
        }

        activeCoroutine = null;
    }
}
