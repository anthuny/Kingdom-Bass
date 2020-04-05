using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gamemode : MonoBehaviour
{
    private TrackCreator tc;
    public AudioManager am;
    public GameObject player;
    public Player playerScript;
    public GameObject jet;
    public float jetZ;
    public float jetY;
    public GameObject eventSystem;
    private EventSystem es;

    [Header("Scene Manager")]
    public string activeScene;

    [Header("Slider")]
    public GameObject sliderTransformPar;
    public GameObject sliderIntervalPar;
    public GameObject sliderIntervalRef;
    public bool checkForSliderIntervals;
    public float maxDistInterval;
    public Gradient notMissedGrad;
    public Gradient missedGrad;
    public List<Transform> sliders = new List<Transform>();
    public Color sliderEdgeColorMiss;
    public float sliderOffset;
    public int sliderIntervalCountGo;

    public int sliderIntervalCount;

    [Header("Note Management")]
    public int notesPassedPlayer = 0;

    [Header("Main Menu")]
    public GameObject mainMenuUI;
    public GameObject exitPromptUI;

    [Header("Game Pause")]
    public bool gamePaused;
    public GameObject pausedUI;
    public Text countdownText;
    public bool countingDown;
    public bool cantPause = true;

    [Header("Post Game Statistics")]
    public Sprite[] ranks;
    public Image rankImage;
    public GameObject postMapStatsUI;
    public Text endScoreText;
    public Text endTotalAccuracyText;
    public Text indivAccAmountText;
    public Text finalComboText;
    public GameObject postMapUI;

    [Header("Menu")]
    public bool tutPaused;

    [Header("Buttons")]
    public Button playBtn;
    public Button yesBtn;
    public Button retryBtn;
    public Button map1Btn;
    public Button menuBtn;
    public Button continueBtn;

    [Header("Map Buttons")]
    public Button[] mapButtons;
    public GameObject mapSelectionUI;

    [Header("Game UI")]
    public float jetDistance;

    public float playerEvadeStr;

    public float startTime;

    [HideInInspector]
    public float noteSpeed;

    public int score = 0;
    private int oldScore;

    public int targetFps;

    public Text scoreText;
    public Text totalAccuracyText;
    public Text fpsCounterText;
    public Text accuracyText;
    public Text beatsText;
    public Text comboText;
    public Text movingLeftText;
    public Text movingRightText;
    public Text blastInputText;
    public Text aboutToBlastText;

    public Text pressKeyToContinueEnd;

    [Header("Map Selection")]
    public Text mapSelectText;
    [TextArea(1, 2)]
    public string selectAMapText;
    public GameObject startBtn;

    [Header("Tutorial")]
    public Text tutAreaText;
    //[HideInInspector]
    public string tutAreaInfo;
    [TextArea(1, 9)]
    public string[] tutTexts;
    //[HideInInspector]
    public int tutorialStage = 0;
    public int maxTutorialStages;
    public int[] nextStageThreshholdBeats;
    public float timeForMoveBack;
    public GameObject tutorialUI;
    public GameObject[] allVisuals;
    public Text[] keyTexts;
    public string[] keyText;
    public Image[] arrowNotes;
    public Image[] arrow;
    public Text[] supportingTexts;
    public Text[] spaceSupportingTexts;
    public Image[] spaceBar;
    public Image plusSymbol;
    public Transform firstUI;
    public Transform secondUI;
    public Image tutAreaTextBG;
    public Text tutUnPauseText;
    [HideInInspector]
    public Vector3 originalFirstUIPos;
    [HideInInspector]
    public Vector3 originalSecondUIPos;

    public Sprite leftArrowNote;
    public Sprite RightArrowNote;
    public Sprite leftLaunchNote;
    public Sprite rightLaunchNote;
    public Sprite blastNote;
    public Sprite upArrowNote;
    public Sprite bombIcon;
    public Sprite playerShield;
    public Sprite playerNoShield;
    //public Sprite unpressedKey;
    //public Sprite pressedKey;

    [Header("Other")]
    public int debugUICounter = 1;
    bool displayDebugUI;
    public float currentFps;
    bool doneOnce;
    public float accuracy = 3;

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

    [HideInInspector]
    public float greatMax, goodMax, missMax;

    [HideInInspector]
    public int perfects, greats, goods, misses;

    [Header("Accuracy")]
    public int perfectScore;
    public int goodScore;
    public int badScore;
    public int missScore;

    public float perfectMin;
    public float greatMin;
    public float goodMin;

    //[HideInInspector]
    public float totalAccuracy;
    //[HideInInspector]
    public float curAccuracy;
    //[HideInInspector]
    public float totalAccuracyMax;

    public string perfectScoreName;
    public string greatScoreName;
    public string goodScoreName;
    public string missScoreName;

    public int comboMulti = 1;

    public bool scoreIncreased;

    [Tooltip("Max amount of time in seconds for how long it takes for movements to NOT give score")]
    public float maxTimeBetweenInputs;

    [Header("Shield")]
    public float shieldOpacityIncSpeed = 1;
    public float shieldEmissionIncSpeed = 3;
    //[HideInInspector]
    public float shieldOpacity;
    public float maxOpacity;
    //[HideInInspector]
    public float shieldEmissionInc;
    [ColorUsageAttribute(true, true)]
    public Color shieldColor;

    private bool allowIncOpacity;
    private bool allowIncEmission;
    public float shieldMaxEmission;
    public float shieldPulseSpeed = 5;
    public float shieldMinScale;
    public float shieldMaxScale;
    public float shieldScaleSpeed;

    [Header("Path")]
    public Color lane1Color;
    public Color lane2Color;
    public Color lane3Color;

    [Header("Player")]
    public float shieldOffSpeed;

    [Header("Health")]
    public GameObject gameUI;
    public float lerpSpeed;
    //public Text healthText;
    public Image healthBar;
    public float healthMax;
    public float regenPerfect;
    public float regenGreat;
    public float regenGood;
    public float regenMiss;
    public float regenBomb;
    public float regenSlider;
    public float healthRegen;
    public float healthRegenInterval;
    public float audioSpeedDec;
    public float minPitchDec;
    public float minPitchVolDecStart;
    public GameObject gameOverUI;
    [HideInInspector]
    public AudioSource activeTrack;
    private float regenPerfectOri;
    private float regenGreatOri;
    private float regenGoodOri;
    private float regenMissOri;
    private float regenBombOri;
    private float regenSliderOri;
    float t = 1;
    float y = 1;

    //public Map retryingMap;

    public float health;
    public bool playerDead;
    public bool killingPlayer;

    [Header("Note Sprites")]
    public Sprite leftArrow;
    public Sprite rightArrow;
    public Sprite upArrow;
    public Sprite leftLaunchArrow;
    public Sprite rightArrowLaunch;
    public Sprite blast;
    public Sprite blastAim;
    public Sprite bomb;
    public LineRenderer sliderRef;

    [Header("UI Camera")]
    public GameObject UICam;

    [Header("Notes")]
    public float distPercArrowLock;
    public int totalNotes;
    public int notesLeftInfront;

    [Header("Electricity")]
    //public LineRenderer lr;
    public List<LineRenderer> lrs = new List<LineRenderer>();
    public GameObject lrPrefab;
    public Transform electricityStart;
    public ParticleSystemRenderer ElectrictyBallPS;
    public Color missedColour;
    public Color readyColour;
    public Color stealthColor;
    public Gradient stealthGradient;
    public Gradient readyGradient;
    public Gradient readyNextGradient;
    public Gradient stealthNextGradient;
    public Transform laserParent;

    [Header("Bombs")]
    public float bombHitRange;

    [Header("Controller")]
    public float movthreshHold = 0;
    public bool controllerConnected;
    [HideInInspector]
    public Controller controls;
    [HideInInspector]
    public Vector2 move, noShieldMove;
    [HideInInspector]
    public float shieldingVal;
    [HideInInspector]
    public float blastLVal, blastRVal;
    public float maxSpeed, lowSpeed;
    public bool firstStart;
    public GameObject lastSelectedBtn;

    //public Vector3 playerPos;

    private void Awake()
    {
        controls = new Controller();

        // Moving left / right with shield
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;

        // Moving left / right without shield
        controls.Gameplay.Move.performed += ctx => noShieldMove = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => noShieldMove = Vector2.zero;

        // Toggling Shield
        controls.Gameplay.Shield.started += ctx => shieldingVal = ctx.ReadValue<float>();
        controls.Gameplay.Shield.canceled += ctx => shieldingVal = ctx.ReadValue<float>();

        // Triggering blast
        controls.Gameplay.BlastL.started += ctx => blastLVal = ctx.ReadValue<float>();
        controls.Gameplay.BlastR.started += ctx => blastRVal = ctx.ReadValue<float>();
        controls.Gameplay.BlastL.canceled += ctx => blastLVal = ctx.ReadValue<float>();
        controls.Gameplay.BlastR.canceled += ctx => blastRVal = ctx.ReadValue<float>();


        es = eventSystem.gameObject.GetComponent<EventSystem>();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        UICam.SetActive(true);

        player = FindObjectOfType<Player>().gameObject;
        playerScript = player.GetComponent<Player>();
        tc = FindObjectOfType<TrackCreator>();
        am = FindObjectOfType<AudioManager>();

        updateGameUI();

        Application.targetFrameRate = targetFps;

        health = healthMax;

        mainMenuUI.SetActive(true);
        exitPromptUI.SetActive(false);

        regenPerfectOri = regenPerfect;
        regenGreatOri = regenGreat;
        regenGoodOri = regenGood;
        regenMissOri = regenMiss;
        regenBombOri = regenBomb;
        regenSliderOri = regenSlider;

        
        StartGame();
        InvokeRepeating("FindControllers", 0, 2f);
    }

    // Check if a controller is connected 
    void FindControllers()
    {
        //Get Joystick Names
        string[] temp = Input.GetJoystickNames();

        //Check whether a controller is connected
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    controllerConnected = true;
                }
                else
                {
                    controllerConnected = false;
                }
            }
        }

        if (!firstStart)
        {
            firstStart = true;
            MainMenu();
        }
    }
    
    public void StartGame()
    {
        totalAccuracy = 100;
        totalAccuracyText.text = totalAccuracy.ToString() + "%";

        //lr.gameObject.SetActive(true);
        //lr.SetPosition(0, electricityStart.transform.position);
        //lr.gameObject.SetActive(false);
        gameOverUI.SetActive(false);
        postMapUI.SetActive(false);
        pausedUI.SetActive(false);
        countdownText.gameObject.SetActive(false);
        countingDown = false;

        mapSelectText.text = selectAMapText;

        playerScript.nearestLaneNumber = 3;
        playerScript.oldNearestLaneNumber = 2;

        originalFirstUIPos = firstUI.gameObject.GetComponent<RectTransform>().localPosition;
        originalSecondUIPos = secondUI.gameObject.GetComponent<RectTransform>().localPosition;

        //pressKeyToContinueEnd.gameObject.SetActive(false);
        postMapStatsUI.SetActive(false);

        regenPerfect = regenPerfectOri;
        regenGreat = regenGreatOri;
        regenGood = regenGreatOri;
        regenMiss = regenMissOri;
        regenBomb = regenBombOri;
        regenSlider = regenSliderOri;

        if (tc.selectedMap)
        {
            // Update health altered values based on the health drain value of the map
            regenGood *= tc.selectedMap.healthDrain;
            regenGreat *= tc.selectedMap.healthDrain;
            regenPerfect *= tc.selectedMap.healthDrain;
            regenMiss *= tc.selectedMap.healthDrain;
            regenBomb *= tc.selectedMap.healthDrain;
            regenSlider *= tc.selectedMap.healthDrain;

            // begin the loop for health regeneration
            InvokeRepeating("UpdateHealthRegen", 0, healthRegenInterval);
        }
    }

    void UpdateHealthRegen()
    {
        UpdateHealth(healthRegen);
    }

    void SceneManager()
    {
        if (activeScene == "MapSelection")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                MainMenu();
            }
            return;
        }

        else if (activeScene == "MainMenu")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                ExitGamePrompt();
            }
            return;
        }

        else if (activeScene == "ExitGamePrompt")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                MainMenu();
            }
            return;
        }
    }

    void CheckForNoUISelection()
    {
        if (controllerConnected)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedBtn.gameObject);
            }
            else
            {
                lastSelectedBtn = EventSystem.current.currentSelectedGameObject;
            }

            if (activeScene == "MainMenu" && lastSelectedBtn == null)
            {
                lastSelectedBtn = playBtn.gameObject;
            }

            else if (activeScene == "MapSelection" && lastSelectedBtn == null)
            {
                lastSelectedBtn = map1Btn.gameObject;
            }

            else if (activeScene == "ExitGamePrompt" && lastSelectedBtn == null)
            {
                lastSelectedBtn = yesBtn.gameObject;
            }

            else if (activeScene == "PostMap" && lastSelectedBtn == null)
            {
                lastSelectedBtn = menuBtn.gameObject;
            }

            else if (activeScene == "PostMap" && lastSelectedBtn == null)
            {
                lastSelectedBtn = menuBtn.gameObject;
            }

            else if (activeScene == "Paused" && lastSelectedBtn == null)
            {
                lastSelectedBtn = continueBtn.gameObject;
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    void Update()
    {
        UpdateShield();
        UpdateElectricity();
        TutorialUnpause();
        PlayerDeath();
        PauseInput();
        UpdateHealthBar();
        SceneManager();
        CheckForNoUISelection();

        if (jet.activeSelf)
        {
            jetZ = jetDistance + player.transform.position.z;
            jet.transform.position = new Vector3(0, jetY, jetZ);
        }

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            //comboMulti += 1;
            updateGameUI();
        }

        currentFps = 1.0f / Time.deltaTime;

        fpsCounterText.text = Mathf.FloorToInt(currentFps).ToString();
    }

    void UpdateElectricity()
    {
        // Ensure that there is electricity for every horizontal note
        for (int i = 0; i < playerScript.electricNotes.Count; i++)
        {
            if (!playerScript.electricNotes[i].GetComponent<Note>().assignedElectricity && playerScript.electricNotes[0] != null)
            {
                playerScript.electricNotes[i].GetComponent<Note>().assignedElectricity = true;
                GameObject go = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(laserParent);
                go.transform.name = "Electricty Clone " + i;
                playerScript.electricNotes[i].GetComponent<Note>().lr = go.GetComponent<LineRenderer>();
                playerScript.electricNotes[i].GetComponent<Note>().lrObj = go;
                lrs.Add(go.GetComponent<LineRenderer>());
            }
        }

        if (playerScript.electricNotes.Count >= 1)
        {
            // Update the end position for each electricity
            for (int i = lrs.Count - 1; i >= 0; i--)
            {
                lrs[i].SetPosition(1, playerScript.electricNotes[i].GetComponent<Note>().ElectrictyEnd.position);
                lrs[i].SetPosition(0, electricityStart.position);
            }   
        }

        // Set the colour of the laser ball to the ready colour when it should be
        if (playerScript.isShielding && playerScript.notesInfront.Count >= 1)
        {
            // Update the end position for each electricity
            for (int i = lrs.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    if (playerScript.closestNoteInFrontScript)
                    {
                        if (playerScript.closestNoteInFrontScript.noteDir == "up")
                        {
                            lrs[i].colorGradient = readyNextGradient;
                            ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
                        }
                        else
                        {
                            lrs[i].colorGradient = readyGradient;
                            ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
                        }
                    }
                }
                else
                {
                    lrs[i].colorGradient = readyNextGradient;
                    ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
                }
            }

        }

        // Set the colour of the laser ball to the stealth colour when it should be
        else if (!playerScript.isShielding || playerScript.notesInfront.Count < 1)
        {
            if (playerScript.closestNoteInFrontScript)
            {
                playerScript.closestNoteInFrontScript.doneElecrictyEffect = 0;
            }

            for (int i = lrs.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    ElectrictyBallPS.material.SetColor("Color_CFB35B33", stealthColor * 15);
                    lrs[i].colorGradient = stealthGradient;
                }

                else
                {
                    ElectrictyBallPS.material.SetColor("Color_CFB35B33", stealthColor * 15);
                    lrs[i].colorGradient = stealthNextGradient;
                }
            }
        }

        if (playerScript.closestNoteInFrontScript)
        {
            // If the player got a miss for the nearest note infront, set the colour of the laser ball to the missed colour
            // And disable the electricity
            if (playerScript.closestNoteInFrontScript.missed)
            {
                //playerScript.closestNoteInFrontScript.doneElecrictyEffect = 2;
                ElectrictyBallPS.material.SetColor("Color_CFB35B33", missedColour * 15);

                for (int i = lrs.Count - 1; i >= 0; i--)
                {
                    lrs[i].startWidth = 0;
                    lrs[i].endWidth = 0;
                }
            }
        }

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
            if (shieldOpacity >= maxOpacity || playerScript.shield.transform.localScale.x >= shieldMaxScale)
            {
                shieldOpacity = maxOpacity;

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
    public void updateGameUI()
    {
        if (score != oldScore && tc.notesSpawned > 0)
        {
            comboMulti += 1;
        }
        oldScore = score;
        scoreIncreased = true;

        /*
        accuracyText.text = "Perfect " + perfects.ToString() +
            "\nGreat " + greats.ToString() +
            "\nGood " + goods.ToString() +
            "\nMiss " + misses.ToString();
        */

        comboText.text = comboMulti.ToString();

        scoreText.text = score.ToString();
    }

    void UpdateHealthBar()
    {
        if (health >= 0)
        {
            //healthText.text = "Health " + health.ToString();
            //healthBar.fillAmount = health / 10;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / 10, Time.deltaTime * lerpSpeed);
        }
        else
        {
            //healthText.text = "Health 0";
            healthBar.fillAmount = 0;
        }
    }

    public void UpdateHealth(float amount)
    {
        if (health <= 0 && !playerDead)
        {
            killingPlayer = true;
            health = 0f;
            am.PlaySound("TrackDeath");
            return;
        }
        else if (health > healthMax)
        {
            health = healthMax;
            return;
        }
        else if (health != healthMax && !playerDead || amount != healthRegen && !playerDead)
        {
            health += amount;
            updateGameUI();          
        }
    }

    void PlayerDeath()
    {
        if (killingPlayer)
        {
            cantPause = true;
            playerDead = true;

            // If the pitch of the track has not yet reached it's minimum:
            if (t > minPitchDec)
            {
                // Decrease the pitch of the audio track over time
                t -= Time.deltaTime * audioSpeedDec;
                activeTrack.pitch = t;
            }
            // Ensure that the pitch of the playing track is left at a specific pitch
            else 
            {
                t = minPitchDec;
            }

            // If the pitch has reached a threshhold AND the volume of the track is still above 0
            if (t <= minPitchVolDecStart && y > 0)
            {
                // Decrease the volume of the audio track over time
                y -= Time.deltaTime * (audioSpeedDec * 2.5f);
                activeTrack.volume = y;
            }
            // If the volume of the played track reaches 0 or less, queue the 'death' screen
            else if (y <= 0)
            {
                if (killingPlayer)
                {
                    killingPlayer = false;
                    GameOver();
                    y = 1;
                }
            }
        }
    }

    // Happens when player dies
    void GameOver()
    {
        // Disable game UI
        gameUI.SetActive(false);

        DestroyAllRemainingNotes();

        gameOverUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("GameOverContr");
        }

        AudioListener.pause = true;
    }

    IEnumerator GameOverContr()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(retryBtn.gameObject);
        retryBtn.OnSelect(null);
    }
    // Replay button
    public void Replay()
    {
        DestroyAllRemainingNotes();
        pausedUI.SetActive(false);
        gamePaused = false;
        EndTrack(true);
    }

    // This function only happens when the game needs to restart from either DYING or RESTARTING or GOING TO MAP SELECTION
    void DestroyAllRemainingNotes()
    {
        DestroyAllNotes(true);
        tc.notes.Clear();
        playerScript.furthestBehindNote = null;
        playerScript.activeNotes.Clear();
        playerScript.notesInfront.Clear();
        playerScript.electricNotes.Clear();
        // remove this note from the 'furthestbehindnote' variable
        playerScript.furthestBehindNote = null;
    }
    void PostMapStatistics()
    {
        // Disable game UI
        gameUI.SetActive(false);

        // Calculate the post map stat UI
        endScoreText.text = score.ToString();
        endTotalAccuracyText.text = totalAccuracy.ToString("F2") + "%";
        indivAccAmountText.text =  "Perfect " + perfects.ToString() +
            "\nGreat " + greats.ToString() +
            "\nGood " + goods.ToString() +
            "\nMiss " + misses.ToString();
        finalComboText.text = "Final Combo Multiplier " + comboMulti.ToString();

        #region Calculation for Rank Letter


        float perfectPerc = ((float)perfects / (float)tc.notesSpawned) * 100f;
        float goodPerc = ((float)goods / (float)tc.notesSpawned) * 100f;

        // If player got 100% accuracy, give SS rank
        if (totalAccuracy == 100)
        {
            rankImage.sprite = ranks[0];
            EndingMap();
            return;
        }
        // If player got over 90% perfects compared to all notes,
        // AND less then 1% goods compared to all notes,
        // AND no misses
        // Give S rank
        else if (perfectPerc > 90 && goodPerc < 1 && misses == 0)
        {
            rankImage.sprite = ranks[1];
            EndingMap();
            return;
        }
        // If player got over 80% perfects compared to all notes, AND no misses
        // OR over 90% perfects compared to all notes
        // Give A rank
        else if ((perfectPerc > 80 && misses == 0) || perfectPerc > 90)
        {
            rankImage.sprite = ranks[2];
            EndingMap();
            return;
        }
        // If player got over 70% perfects compared to all notes, AND no misses
        // OR over 80% perfects compared to all notes
        // Give B rank
        else if ((perfectPerc > 70 && misses == 0) || perfectPerc > 80)
        {
            rankImage.sprite = ranks[3];
            EndingMap();
            return;
        }
        // If player got over 60^ perfects compared to all notes.
        // Give C rank
        else if (perfectPerc > 60)
        {
            rankImage.sprite = ranks[4];
            EndingMap();
            return;
        }
        // If the player did not get a rank from any condition above, give rank D
        // Give D rank
        else
        {
            rankImage.sprite = ranks[5];
            EndingMap();
            return;
        }
        #endregion
    }

    void EndingMap()
    {
        // Enable the post map stats UI to see
        postMapStatsUI.SetActive(true);

        // Enable post map buttons to see
        postMapUI.SetActive(true);

        activeScene = "PostMap";

        if (controllerConnected)
        {
            StartCoroutine("EndingMapContr");
        }
    }

    IEnumerator EndingMapContr()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(menuBtn.gameObject);
        menuBtn.OnSelect(null);
    }

    // Happens when the player presses quit
    void ExitGamePrompt()
    {
        mainMenuUI.SetActive(false);
        exitPromptUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("ExitGamePromptContr");
        }

        activeScene = "ExitGamePrompt";
    }

    IEnumerator ExitGamePromptContr()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(yesBtn.gameObject);
        yesBtn.OnSelect(null);
    }

    // Happens when the player presses YES when asked are you sure you want to quit
    public void ExitGame()
    {
        Application.Quit();
    }

    // Happens when the player presses NO when asked are you sure you want to quit
    public void CancelGamePrompt()
    {
        exitPromptUI.SetActive(false);
        mainMenuUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("MainMenuContr");
        }
    }

    public void PlayBtn()
    {
        mapSelectionUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("MapSelectionContr");
        }

        activeScene = "MapSelection";
    }

    // map selection button
    public void MapSelection()
    {
        if (mapSelectionUI.activeSelf)
        {
            mapSelectionUI.SetActive(false);
        }

        if (mainMenuUI.activeSelf)
        {
            mainMenuUI.SetActive(false);
        }

        if (tutorialUI.activeSelf)
        {
            tutorialUI.SetActive(false);
        }

        if (controllerConnected)
        {
            StartCoroutine("MapSelectionContr");
        }

        activeScene = "MapSelection";

        DestroyAllRemainingNotes();
        pausedUI.SetActive(false);
        gamePaused = false;
        cantPause = true;
        EndTrack(false);
    }

    IEnumerator MapSelectionContr()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(map1Btn.gameObject);
        map1Btn.OnSelect(null);
    }
    public void MainMenu()
    {
        if (mapSelectionUI.activeSelf)
        {
            mapSelectionUI.SetActive(false);
        }

        if (exitPromptUI.activeSelf)
        {
            exitPromptUI.SetActive(false);
        }

        if (mapSelectionUI.activeSelf)
        {
            mapSelectionUI.SetActive(false);
        }

        mainMenuUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("MainMenuContr");
        }

        activeScene = "MainMenu";

        cantPause = true;
        tc.selectedMap = null;
        startBtn.SetActive(false);
    }

    IEnumerator MainMenuContr()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(playBtn.gameObject);
        playBtn.OnSelect(null);
    }

    public void EndTrack(bool retry)
    {
        // Reset the tutorial stage if it had already proceeded into the first stage
        if (tutorialStage > 0)
        {
            tutorialStage = 0;
        }

        gameOverUI.SetActive(false);
        postMapUI.SetActive(false);

        // Disable the post map stats UI
        if (postMapStatsUI.activeSelf)
        {
            postMapStatsUI.SetActive(false);
        }

        tc.mapHasBeenSelected = false;
        tc.trackInProgress = false;

        // Disable game UI
        gameUI.SetActive(false);

        t = 1;
        y = 1;

        // Reset the track's values, and stop it  for the next map
        if (activeTrack)
        {
            activeTrack.pitch = 1;
            activeTrack.volume = 1;
            activeTrack.Stop();
        }

        tc.loadingTrack = false;
        tc.notesSpawned = 0;
        tc.allNotes.Clear();
        tc.allNoteTypes.Clear();
        tc.beatWaitCountAccum.Clear();
        playerScript.electricNotes.Clear();
        notesPassedPlayer = 0;

        tc.noteLanes.Clear();
        tc.beatWaitCount.Clear();
        tc.trackPosIntervalsList2.Clear();
        tc.trackPosIntervalsList3.Clear();
        tc.trackPosNumber = 0;
        tc.trackPosNumber2 = 0;
        tc.secPerBeat = 0;
        tc.trackPos = 0;
        tc.trackPosInBeats = 0;
        tc.trackPosInBeatsGame = 0;
        tc.lastBeat = 0;
        tc.previousNoteBeatTime = 0;
        tc.previousNoteBeatTime2 = 0;
        tc.previousNoteBeatTime3 = 0;
        tc.nextNoteInBeats = 0;
        tc.curNoteCount = 0;
        tc.nextIndex3 = 0;
        tc.newStartingNoteAccum = 0;
        tc.oldNewStartingNoteAccum = 0;
        tc.beatWaitAccum = 0;
        tc.newStartingInt = 0;

        score = 0;
        perfects = 0;
        greats = 0;
        goods = 0;
        misses = 0;
        curAccuracy = 0;
        totalAccuracy = 0;
        totalAccuracyMax = 0;
        totalNotes = 0;
        totalAccuracyText.text = 0.ToString() + "%";
        comboMulti = 1;
        comboText.text = "1";

        playerScript.activeNotes.Clear();
        playerScript.notesInfront.Clear();
        playerScript.newPerfect = 0;
        playerScript.newGood = 0;
        playerScript.newGood = 0;
        playerScript.newGoodMiss = 0;

        playerScript.closestNoteInFrontScript = null;
        playerScript.nearestAnyNote = null;
        playerScript.nearestAnyNoteScript = null;


        if (!retry)
        {
            mapSelectText.text = selectAMapText;
            mapSelectText.gameObject.SetActive(true);

            tutorialUI.SetActive(false);

            // If the player reached the end of the map or exited to map selection
            if (!playerDead && health <= 0)
            {
                DestroyAllNotes(false);
            }

            mapSelectionUI.SetActive(true);

            if (controllerConnected)
            {
                StartCoroutine("MapSelectionContr");
            }

            activeScene = "MapSelection";

            tc.selectedMap = null;
        }
        else
        {
            //DestroyAllRemainingNotes();
            StartCoroutine(tc.LoadTrackCo());
        }

        playerScript.RepositionPlayer();

        playerDead = false;
        health = healthMax;
    }

    public void EndTrackNote()
    {
        Invoke("PostMapStatistics", tc.trackEndWait);
    }
   
    public void UpdateTotalAccuracy()
    {
        // Update the total accuracy.
        totalAccuracy = (curAccuracy / totalAccuracyMax) * 100;

        if (totalAccuracy != 100)
        {
            // Display the total accuracy UI only in 2 decimal places
            totalAccuracyText.text = totalAccuracy.ToString("F2") + "%";
        }
        else if (totalAccuracy >= 100)
        {
            // Display the total accuracy UI as 100% if it is, this is so 100% doesnt have 2 dp.
            totalAccuracyText.text = "100%";
        }
    }

    void DestroyAllNotes(bool playerDied)
    {
        // Destroy all notes that are still alive if the playe ended the map
        if (!playerDied)
        {
            for (int i = 0; i < tc.notesObj.transform.childCount; i++)
            {
                GameObject go = tc.notesObj.transform.GetChild(i).gameObject;
                StartCoroutine(go.GetComponent<Note>().DestroyNote());
            }
        }

        // Destroy all notes differently if the player dies during the map
        else
        {
            for (int i = 0; i < tc.notesObj.transform.childCount; i++)
            {
                Destroy(tc.notesObj.transform.GetChild(i).gameObject);
            }
        }
    }
    void PauseInput()
    {
        if (!cantPause)
        {
            // Input to pause the game
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9") && !mapSelectionUI.activeSelf && !countingDown && !gamePaused && !tc.allNotes[tc.allNotes.Count - 1].GetComponent<Note>().behindPlayer)
            {
                PauseGame(false);
            }
        }

        // Input to resume the game
        else if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown("joystick button 2")) && gamePaused && !countingDown)
        {
            StartCoroutine(UnpauseGame());
        }

        // If player presses escape in the main menu screen, send to exit game prompt
        if (Input.GetKeyDown(KeyCode.Escape) && mainMenuUI.activeSelf)
        {
            ExitGamePrompt();
        }

        // If player presses escape in the map selection screen, send to main menu
        if (Input.GetKeyDown(KeyCode.Escape) && mapSelectionUI.activeSelf)
        {
            MainMenu();
        }


    }
    // If fate is true, the pause is for the end of a map
    // if fate is false, it's for the tutorial stage pauses
    public void PauseGame(bool tutPause)
    {
        AudioListener.pause = true;

        activeScene = "Paused";

        if (tutPause)
        {
            tutUnPauseText.gameObject.SetActive(true);
            tutPaused = true;
        }
        else
        {
            gamePaused = true;
            cantPause = true;
            pausedUI.SetActive(true);

            if (controllerConnected)
            {
                StartCoroutine("PauseGameControllerSelection");
            }
        }
    }

    IEnumerator PauseGameControllerSelection()
    {
        // Have the controller select a button
        yield return null; // Wait 1 frame for UI to recalculate.
        if (es.currentSelectedGameObject != null)
        {
            var previous = es.currentSelectedGameObject.GetComponent<Selectable>();
            if (previous != null)
            {
                previous.OnDeselect(null);
                es.SetSelectedGameObject(null);
            }
        }
        es.SetSelectedGameObject(continueBtn.gameObject);
        continueBtn.OnSelect(null);
    }

    // This exists so the continue button in the pause menu can be linked to this
    public void UnPauseGameBut()
    {
        StartCoroutine(UnpauseGame());
    }

    public IEnumerator UnpauseGame()
    {
        countingDown = true;
        pausedUI.SetActive(false);
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";

        yield return new WaitForSeconds(1f);

        countdownText.text = "2";

        yield return new WaitForSeconds(1f);

        countdownText.text = "1";

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        gamePaused = false;
        countingDown = false;
        cantPause = false;
        AudioListener.pause = false;

        activeScene = "Game";
    }

    public void TutorialUnpause()
    {
        if (Input.GetKeyDown(KeyCode.Return) && tutPaused && tc.selectedMap.title == "Tutorial")
        {
            tutPaused = false;
            AudioListener.pause = false;

            if (tutUnPauseText.enabled)
            {
                tutUnPauseText.gameObject.SetActive(false);
            }
        }
    }
}
