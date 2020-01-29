using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
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

    public float currentFps;
    public Text fpsCounterText;
    public float launchRotAmount;
    public float launchRotTime;

    public Color horizontalNoteArrowC;
    public Color horizontalLaunchArrowC;
    public Color upArrowC;

    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a PERFECT. 0 - 1")] 
    public float perfectMin;
    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a GOOD. 0 - 1")]
    public float goodMin;

    [Range(0, 1)]
    [Header("The MINIMUM point of how accurate the note must be hit for a BAD. 0 - 1")]
    public float badMin;

    [HideInInspector]
    public float goodMax, badMax, missMax;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;

        player = FindObjectOfType<Player>().gameObject;
        jet = FindObjectOfType<Jet>().gameObject;

        UpdateUI();

        Application.targetFrameRate = targetFps;

        goodMax = perfectMin - 0.01f;
        badMax = goodMax - 0.01f;
        missMax = badMax - 0.01f;
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
    }

    void UpdateUI()
    {
        scoreText.text = "Score | " + score.ToString();
    }
}
