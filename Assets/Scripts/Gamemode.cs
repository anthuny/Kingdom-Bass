using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    private TrackCreator tc;
    private GameObject player;
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

    [Range(0, 10)]
    [Header("What percentage of accuracy is required for a PERFECT hit?")] 
    public float prPercent;
    public GameObject perfectCOL;
    [Range(0, 10)]
    [Header("What percentage of accuracy is required for a GREAT hit?")]
    public float grPercent;
    public GameObject greatCOL;
    [Range(0, 10)]
    [Header("What percentage of accuracy is required for a GOOD hit?")]
    public float goPercent;
    public GameObject goodCOL;
    //anything outside of these percentages will be treated as a miss

    [HideInInspector]
    public float greatMax, goodMax, missMax;

    [HideInInspector]
    public int perfects, greats, goods, misses;

    [Header("Accuracy Score Amounts")]
    public int perfectScore;
    public int goodScore;
    public int badScore;
    public int missScore;

    public bool scoreIncreased;

    [Tooltip("Max amount of time in seconds for how long it takes for movements to NOT give score")]
    public float maxTimeBetweenInputs;

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        player = FindObjectOfType<Player>().gameObject;
        jet = FindObjectOfType<Jet>().gameObject;
        tc = FindObjectOfType<TrackCreator>();

        UpdateUI();

        Application.targetFrameRate = targetFps;

        //find all the colliders and set them to gameObject Vars
        perfectCOL = GameObject.FindGameObjectWithTag("PERFECT");
        greatCOL = GameObject.FindGameObjectWithTag("GREAT");
        goodCOL = GameObject.FindGameObjectWithTag("GOOD");
        Vector3 playerT = player.transform.localScale;
        playerT.y = 1;

        //set the Z scale of each to the value of the percentage set in inspector this will only affect the width of the collider relative to the centre of the object
        playerT.z = prPercent;
        perfectCOL.transform.localScale = playerT;
        playerT.z = grPercent;
        greatCOL.transform.localScale = playerT;
        playerT.z = goPercent;
        goodCOL.transform.localScale = playerT;
    }

    void Update()
    {
        jetZ = jetDistance + player.transform.position.z;
        jet.transform.position = new Vector3(0, jetY, jetZ);

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            UpdateUI();
        }

        currentFps = 1.0f / Time.deltaTime;
        fpsCounterText.text = "FPS | " + (int)currentFps;

        noteCalculationOverText.text = "noteCalculationOver = " + player.GetComponent<Player>().noteCalculationOver.ToString();
        scoreAllowedText.text = "scoreAllowed = " + player.GetComponent<Player>().scoreAllowed.ToString();
        canGetNoteText.text = "canGetNote = " + tc.canGetNote.ToString();
        timeFromLastMoveText.text = "Time From Last Movement " + player.GetComponent<Player>().elapsedTimeSinceMove.ToString();
    }

    public void UpdateUI()
    {
        oldScore = score;
        scoreIncreased = true;

        scoreText.text = "Score | " + score.ToString();
        perfectsText.text = "Perfect: " + perfects.ToString()+
            "\nGreat: " + greats.ToString() +
            "\nGood: " + goods.ToString()+
            "\nMiss: " + misses.ToString();
    }
}
