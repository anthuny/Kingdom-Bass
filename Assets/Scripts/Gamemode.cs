using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    private TrackCreator tc;
    private GameObject player;
    private Player playerScript;
    [HideInInspector]
    public GameObject jet;
    public float jetZ;
    public float jetY;

    public float jetDistance;

    public float playerEvadeStr;

    public float startTime;

    [HideInInspector]
    public float noteSpeed;

    public int score = 0;
    private int oldScore;

    public float distPercArrowLock;

    public int targetFps;

    public Text scoreText;
    public Text fpsCounterText;
    public Text perfectsText;
    public Text greatsText;
    public Text goodsText;
    public Text missesText;
    public Text beatsText;
    public Text comboText;
    public Text healthText

    public Text timeFromLastMoveText;

    public Text canGetNoteText;
    public Text scoreAllowedText;
    public Text noteCalculationOverText;

    public float currentFps;

    public float launchRotAmount;
    public float launchRotTime;

    public Color horizontalNoteArrowC;
    public Color horizontalLaunchArrowC;
    public Color upArrowC;
    public Color blastNoteC;
    [Tooltip("The higher this value is, the lighter the spotlight will be, compared to the colour of the note")]
    public float noteSpotLightDiff;
    [Tooltip("The higher this value is, the larger the spotlight will be (in scale) for a launch note")]
    public float launchSpotLightInc;

    [Tooltip("The higher this value is, the more intense the spotlight will be for a note note")]
    public float noteSpotLightIntensity;
    [Tooltip("The higher this value is, the more intense the spotlight will be for a launch note")]
    public float launchSpotLightIntensity;

    [Tooltip("This is the maximum opacity the material can reach")]
    public float maxPathOpacity = 15;
    [Tooltip("This is the maximum distance that the path will be visible. The higher this number, the further the player can see of the path")]
    public float maxPathViewDist = 1.1f;
    [Tooltip("The higher this value, the faster the increase of opacity will elapse at the start of the game")]
    public float pathOpacityIncSpeed = 3;
    [Tooltip("The higher this value, the faster the increase in player sight for the path will be at the start of the game")]
    public float pathViewDistIncSpeed = .2f;

    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a PERFECT. 0 - 1")] 
    public float perfectMin;
    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a GREAT. 0 - 1")]
    public float greatMin;
    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a GOOD. 0 - 1")]
    public float goodMin;
    //recomend change from checking distance between notes to a set of colliders on the player and check that way as to free up processing from constantly checking distance of notes
    //probably not that big of a drain for most PCs to notice but need to optimise where we can for slower ones

    [HideInInspector]
    public float greatMax, goodMax, missMax;

    [HideInInspector]
    public int perfects, greats, goods, misses;

    [Header("Accuracy Score Amounts")]
    public int perfectScore;
    public int goodScore;
    public int badScore;
    public int missScore;

    public int comboMulti = 1;


    public bool scoreIncreased;

    [Tooltip("Max amount of time in seconds for how long it takes for movements to NOT give score")]
    public float maxTimeBetweenInputs;

    [Header("Shield")]
    public float shieldOpacityIncSpeed = 1;
    public float shieldEmissionIncSpeed = 3;
    //[HideInInspector]
    public float shieldOpacity;
    //[HideInInspector]
    public float shieldEmissionInc;
    [ColorUsageAttribute(true,true)]
    public Color shieldColor;

    private bool allowIncOpacity;
    private bool allowIncEmission;
    private bool completed1 = true;
    private bool completed2 = true;
    public float shieldMaxEmission;
    public float shieldPulseSpeed = 5;
    public float shieldMinScale;
    public float shieldMaxScale;
    public float shieldScaleSpeed;

    [Header("Path")]
    public Color lane1Color;
    public Color lane2Color;
    public Color lane3Color;


    [Header("Health Bar")]
    private float health;

    public float healthMax;
    public float regenPerfect;
    public float regenGreat;
    public float regenGood;
    public float lossMiss;
    public float healthRegenPerSec;

    void Start()
    { 
        QualitySettings.vSyncCount = 0;

        player = FindObjectOfType<Player>().gameObject;
        playerScript = player.GetComponent<Player>();
        jet = FindObjectOfType<Jet>().gameObject;
        tc = FindObjectOfType<TrackCreator>();

        UpdateUI();

        Application.targetFrameRate = targetFps;

        greatMax = perfectMin + 0.01f;
        goodMax = greatMax + 0.01f;
        missMax = goodMax + 0.01f;

        perfectMin = 1 - perfectMin;
        greatMin = 1 - greatMin;
        goodMin = 1 - goodMin;

        health = healthMax;
    }

    void Update()
    {
        UpdateShield();

        jetZ = jetDistance + player.transform.position.z;
        jet.transform.position = new Vector3(0, jetY, jetZ);

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            comboMulti += 1;
            UpdateUI();
        }

        currentFps = 1.0f / Time.deltaTime;
        fpsCounterText.text = "FPS | " + (int)currentFps;

        noteCalculationOverText.text = "noteCalculationOver = " + playerScript.noteCalculationOver.ToString();
        scoreAllowedText.text = "scoreAllowed = " + playerScript.scoreAllowed.ToString();
        canGetNoteText.text = "canGetNote = " + tc.canGetNote.ToString();
        timeFromLastMoveText.text = "Time From Last Movement " + playerScript.elapsedTimeSinceMove.ToString();
        beatsText.text = tc.trackPosInBeats.ToString();

        UpdateHealth(healthRegenPerSec);
    }

    void UpdateShield()
    {
        // Turning the visuals ON for the shield
        if (playerScript.isShielding)
        {
            //completed1 = false;
            allowIncOpacity = true;
            allowIncEmission = true;
        }

        if (allowIncOpacity)
        {
            // Increase the opacity of the shield
            shieldOpacity += Time.deltaTime * shieldOpacityIncSpeed;

            // If shield is active, set pulse speed to it's actual value
            playerScript.shieldMat.SetFloat("Vector1_60F525E0", shieldPulseSpeed);

            // Increase the scale of the shield
            //playerScript.shield.transform.localScale = Vector3.Lerp(new Vector3(shieldMinScale, shieldMinScale, shieldMinScale),
            //    new Vector3(shieldMaxScale, shieldMaxScale, shieldMaxScale), shieldScaleSpeed);

            playerScript.shield.transform.localScale += Vector3.one * Time.deltaTime * shieldScaleSpeed;

            // if the shield opacity reaches 1, stop it from continuing
            if (shieldOpacity >= 1 || playerScript.shield.transform.localScale.x >= shieldMaxScale)
            {
                shieldOpacity = 1;

                // Set the scale of the shield to the max scale
                playerScript.shield.transform.localScale = new Vector3(shieldMaxScale, shieldMaxScale, shieldMaxScale);
            }
        }

        if (allowIncEmission)
        {
            // Increase the emission over time
            shieldEmissionInc += Time.deltaTime * shieldEmissionIncSpeed;

            // If the shield emmision reaches or passes the max, stop it from continuing
            if (shieldEmissionInc >= shieldMaxEmission)
            {
                shieldEmissionInc = shieldMaxEmission;
            }
        }

        // Turning the visuals OFF for the shield
        if (!playerScript.isShielding)
        {
            allowIncOpacity = false;
            allowIncEmission = false;
        }

        if (!allowIncOpacity)
        {
            // Increase the opacity of the shield
            shieldOpacity -= Time.deltaTime * shieldOpacityIncSpeed;

            // If shield is active, set pulse speed to 0
            playerScript.shieldMat.SetFloat("Vector1_60F525E0", 0);

            // Decrease the scale of the shield
            //playerScript.shield.transform.localScale = Vector3.Lerp(new Vector3(shieldMaxScale, shieldMaxScale, shieldMaxScale),
             //   new Vector3(shieldMinScale, shieldMinScale, shieldMinScale), shieldScaleSpeed);

            playerScript.shield.transform.localScale -= Vector3.one * Time.deltaTime * shieldScaleSpeed;

            // if the shield opacity reaches 0, stop it from continuing
            if (shieldOpacity <= 0 || playerScript.shield.transform.localScale.x <= shieldMinScale)
            {
                shieldOpacity = 0;
                shieldEmissionInc = 0;
                // Set the scale of the shield to the min scale
                playerScript.shield.transform.localScale = new Vector3(shieldMinScale, shieldMinScale, shieldMinScale);
            }
        }

        if (!allowIncEmission)
        {
            // Increase the emission over time
            shieldEmissionInc -= Time.deltaTime * shieldEmissionIncSpeed;

            // If the shield emmision reaches or passes the max, stop it from continuing
            if (shieldEmissionInc <= 0)
            {
                shieldEmissionInc = 0;
            }
        }
    }
    public void UpdateUI()
    {
        if (score != oldScore)
        {
            comboMulti += 1;
        }
        oldScore = score;
        scoreIncreased = true;

        scoreText.text = "Score | " + score.ToString();
        perfectsText.text = "Perfect: " + perfects.ToString()+
            "\nGreat: " + greats.ToString() +
            "\nGood: " + goods.ToString()+
            "\nMiss: " + misses.ToString();
        comboText.text = ("Combo Multiplier x" + comboMulti.ToString());
        healthText.text = (health.ToString());
    }

    public void UpdateHealth(float amount)
    {
        health += amount;

        if (health<=0)
        {
            health = 0f;
        }
        else if (health>=healthMax)
        {
            health = healthMax;
        }
    }
}
