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

    public float currentFps;

    public float launchRotAmount;
    public float launchRotTime;

    public Color horizontalNoteArrowC;
    public Color horizontalLaunchArrowC;
    public Color upArrowC;

    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a PERFECT. 0 - 1")] 
    public float perfectMin;
    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a GREAT. 0 - 1")]
    public float greatMin;
    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a GOOD. 0 - 1")]
    public float goodMin;

    [HideInInspector]
    public float greatMax, goodMax, missMax;

    [HideInInspector]
    public int perfects, greats, goods, misses;

    [Header("Accuracy Score Amounts")]
    public int perfectScore;
    public int goodScore;
    public int badScore;
    public int missScore;

    [Tooltip("Max amount of time in seconds for how long it takes for movements to NOT give score")]
    public float maxTimeBetweenInputs;


    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;

        player = FindObjectOfType<Player>().gameObject;
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
    }

    // Update is called once per frame
    void Update()
    {
        jetZ = jetDistance + player.transform.position.z;

        jet.transform.position = new Vector3(0, jetY, jetZ);

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            oldScore = score;
            UpdateUI();
        }

        currentFps = 1.0f / Time.deltaTime;
        fpsCounterText.text = "FPS | " + (int)currentFps;

        timeFromLastMoveText.text = "Time From Last Movement " + player.GetComponent<Player>().timeFromLastMovement.ToString();
    }

    public void UpdateUI()
    {
        scoreText.text = "Score | " + score.ToString();

        perfectsText.text = "Perfect: " + perfects.ToString();
        greatsText.text = "Great: " + greats.ToString();
        goodsText.text = "Good: " + goods.ToString();
        missesText.text = "Miss: " + misses.ToString();
    }
}
