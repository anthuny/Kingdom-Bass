using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Gamemode : MonoBehaviour
{
    public TrackCreator tc;
    public AudioManager am;
    public GameObject player;
    public Player playerScript;
    public GameObject jet;
    public Jet jScript;
    public float jetZ;
    public float jetY;
    public GameObject eventSystem;
    private EventSystem es;

    [Header("Scene Manager")]
    public string activeScene;

    [Header("Jet")]
    public float noteDistForAim;

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
    public float slowSpeedMultCur;

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
    public Text postScoreText;
    public Text postTotalAccuracyText;
    public Text perfectAmountText;
    public Text greatAmountText;
    public Text goodAmountText;
    public Text missAmountText;
    public Text postComboText;
    public Text trackName;
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
    public GameObject screenCanvas;

    [Header("Map Selection")]
    public Text mapSelectText;
    [TextArea(1, 2)]
    public string selectAMapText;
    public GameObject startBtn;

    [Header("Tutorial")]
    public Animator tutTextAnimator;

    [Header("Parent UI")]
    public GameObject keyboardUI;
    public Animator keyboardUIAnimator;
    public GameObject controllerUI;
    public Animator controllerUIAnimator;
    [Header("Keyboard UI")]
    public GameObject noteKB;
    public Animator noteKBAnimator;
    public GameObject noteDoubleKB;
    public Animator noteDoubleKBAnimator;
    public GameObject key;
    public Animator keyAnimator;
    public GameObject keyDouble;
    public Animator keyDoubleAnimator;
    [Header("Controller UI")]
    public GameObject noteCT;
    public Animator noteCTAnimator;
    public GameObject noteDoubleCT;
    public Animator noteDoubleCTAnimator;
    public GameObject controllerImage;
    public Animator controllerImageAnimator;
    public GameObject controllerImageDouble;
    public Animator controllerImageDoubleAnimator;
    public GameObject leftArrowCT;
    public GameObject rightArrowCT;
    public GameObject tiltText;
    public GameObject pressBothText;

    [Header("Sprites")]
    public Sprite leftArrowNote;
    public Sprite RightArrowNote;
    public Sprite leftLaunchNote;
    public Sprite rightLaunchNote;
    public Sprite blastNote;
    public Sprite upArrowNote;
    public Sprite bombIcon;
    public Sprite playerShield;
    public Sprite playerNoShield;

    public Sprite aKeyImageSprite;
    public Sprite lKeyImageSprite;
    public Sprite leftControllerImageSprite;
    public Sprite rightControllerImageSprite;
    public Sprite middleControllerImageSprite;
    public Sprite tutStageTarget;
    public Sprite tutStageBlastPlayer;
    public Sprite tutStageBlastContr;
    public Sprite spacePressed;
    public Sprite spaceHeld;
    public Sprite cross;
    public Sprite slider;
    public Sprite shieldControllerImageSprite;
    public Sprite noShieldPlayer;


    [Header("Other")]
    public GameObject blur;
    public int doneTutStageCount;

    public float tutPosResetTime;
    public bool resetPosition;
    public GameObject laneSwitching;
    public Text tutAreaText;
    //[HideInInspector]
    public string tutAreaInfo;
    [TextArea(1, 9)]
    public string[] tutTexts;
    //[HideInInspector]
    public int tutorialStage = 0;
    public int oldTutorialStage;
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

    [Header("Other")]
    public int debugUICounter = 1;
    bool displayDebugUI;
    public float currentFps;
    bool doneOnce;
    public float accuracy = 3;

    public float launchRotAmount;
    public float launchRotTime;

    public Color missedNoteC;
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

    [Header("Path")]
    public Color lane1Color;
    public Color lane2Color;
    public Color lane3Color;

    [Header("Player")]
    public float shieldOffSpeed;
    public float slowSpeedMult = .5f;

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
    public int totalAllNotes;
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
    public bool electricityHasTurnedOff;
    public bool noNotesInFront;
    public bool doneOnce2;

    [Header("Bombs")]
    public float bombHitRange;
    //public float minListenerDist;
    //public float maxListenerDist;

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

    [Header("Controller Connected UI")]
    public GameObject controllerConnectedGo;
    public GameObject controllerConnectedBG;
    public Sprite controllerConnectedImage;
    public Sprite controllerDisconnectedImage;

    public GameObject cursor;

    public bool canReturn;
    //public Vector3 playerPos;

    [Header("UI")]
    public Color defColor;
    public Color defMapColor;
    public Color highlightedColor;
    public Color selectedColor;
    public GameObject lastSelectedButton;

    [Header("SceneChanging")]
    public string oldScene;

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
        //Debug.Log("yes");
        //Cursor.visible = false;
        //screenCanvas.GetComponent<GraphicRaycaster>().enabled = false;
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
        //Debug.Log("yes");
        //Cursor.visible = true;
        //screenCanvas.GetComponent<GraphicRaycaster>().enabled = true;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        UICam.SetActive(true);

        player = FindObjectOfType<Player>().gameObject;
        playerScript = player.GetComponent<Player>();
        tc = FindObjectOfType<TrackCreator>();
        am = FindObjectOfType<AudioManager>();
        jScript = FindObjectOfType<Jet>();

        jScript.AssignVariables();

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

        jScript.DisableJet();

        StartGame();
        InvokeRepeating("FindControllers", 0, 2f);
    }


    // Check if a controller is connected 
    void FindControllers()
    {
        //Get Joystick Names
        string[] temp = Input.GetJoystickNames();
        //Debug.Log(temp[1]);

        if (temp.Length > 0)
        {
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    //Not empty, controller temp[i] is connected
                    //Debug.Log("Controller " + i + " is connected using: " + temp[i]);
                    controllerConnected = true;
                    cursor.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    //If it is empty, controller i is disconnected
                    //where i indicates the controller number
                    //Debug.Log("Controller: " + i + " is disconnected.");
                    controllerConnected = false;
                    cursor.GetComponent<Image>().color = Color.green;
                }
            }
        }
    }
    
    public void StartGame()
    {
        if (!firstStart)
        {
            firstStart = true;

            tutorialUI.SetActive(false);
            keyboardUI.SetActive(false);
            controllerUI.SetActive(false);

            MainMenu();
        }
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

        //originalFirstUIPos = firstUI.gameObject.GetComponent<RectTransform>().localPosition;
        //originalSecondUIPos = secondUI.gameObject.GetComponent<RectTransform>().localPosition;

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
        if (!gamePaused)
        {
            UpdateHealth(healthRegen);
        }
    }

    void SceneManager()
    {
        if (activeScene == "MapSelection")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                es.SetSelectedGameObject(null);

                if (lastSelectedButton)
                {
                    lastSelectedButton.GetComponent<MapInfo>().selected = false;
                    lastSelectedButton = null;
                }

                MainMenu();
            }
            return;
        }

        else if (activeScene == "MainMenu")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                es.SetSelectedGameObject(null);

                if (lastSelectedButton)
                {
                    lastSelectedButton.GetComponent<MapInfo>().selected = false;
                    lastSelectedButton = null;
                }

                ExitGamePrompt();
            }
            return;
        }

        else if (activeScene == "ExitGamePrompt")
        {
            if (Input.GetKeyDown("joystick button 2"))
            {
                es.SetSelectedGameObject(null);
                MainMenu();
            }
            return;
        }

        else if (activeScene == "Paused")
            if (canReturn)
            {
                if (Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.Escape))
                {
                    es.SetSelectedGameObject(null);
                    UnPauseGameBut();
                }
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
        UpdateElectricity();
        PlayerDeath();
        PauseInput();
        UpdateHealthBar();
        SceneManager();
        CheckForNoUISelection();
        PlaySceneChangeSFX();

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            //comboMulti += 1;
            updateGameUI();
        }

        currentFps = 1.0f / Time.deltaTime;

        fpsCounterText.text = Mathf.FloorToInt(currentFps).ToString();
    }

    void PlaySceneChangeSFX()
    {
        if (oldScene != activeScene)
        {
            oldScene = activeScene;
            am.PlaySound("SceneSwitch");
        }
    }

    void UpdateElectricity()
    {
        if (!tc.selectedMap)
        {
            return;
        }

        if (tc.selectedMap.title == "Tutorial" && tutorialStage < 9 || playerDead)
        {
            ElectrictyBallPS.gameObject.SetActive(false);
            return;
        }

        ElectrictyBallPS.gameObject.SetActive(true);

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
            for (int i = lrs.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    if (playerScript.closestElectricNoteInFrontScript)
                    {
                        if (playerScript.closestElectricNoteInFrontScript.noteDir != "up" && playerScript.closestElectricNoteInFrontScript.noteDir != "down" && !playerScript.closestElectricNoteInFrontScript.missed)
                        {
                            //Debug.Log("4");
                            //playerScript.closestNoteInFrontScript.electricityHasTurnedOff = true;
                            lrs[i].colorGradient = readyGradient;
                            ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
                        }
                    }
                }
                else
                {
                    lrs[i].colorGradient = readyNextGradient;
                    //ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
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
            if (playerScript.closestNoteInFrontScript.noteDir != "up" && playerScript.closestNoteInFrontScript.noteDir != "down")
            {
                // If the player got a miss for the nearest note infront, set the colour of the laser ball to the missed colour
                // And disable the electricity
                if (playerScript.closestNoteInFrontScript.missed)// && playerScript.closestNoteInFrontScript.electricityHasTurnedOff)
                {
                    //Debug.Log("3");
                    playerScript.closestNoteInFrontScript.electricityHasTurnedOff = true;
                    electricityHasTurnedOff = true;
                    //playerScript.closestNoteInFrontScript.doneElecrictyEffect = 2;
                    ElectrictyBallPS.material.SetColor("Color_CFB35B33", missedColour * 15);

                    lrs[0].startWidth = 0;
                    lrs[0].endWidth = 0;
                }

                // Trigger SFX for electricty re-activating
                if (!playerScript.closestNoteInFrontScript.missed && electricityHasTurnedOff)
                {
                    //Debug.Log("1");
                    electricityHasTurnedOff = false;
                    am.PlaySound("PowerOn");
                }

                if (!playerScript.notesInfront[0].GetComponent<Note>().electricityHasTurnedOff && !doneOnce2)
                {
                    //Debug.Log("2");
                    playerScript.notesInfront[0].GetComponent<Note>().electricityHasTurnedOff = true;
                    am.PlaySound("PowerOn");
                }
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
            playerScript.KillPlayer();
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
            //Debug.Log(amount);
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

                    CancelInvoke("UpdateHealthRegen");
                }
            }
        }
    }

    // Happens when player dies
    void GameOver()
    {
        activeScene = "GameOver";

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
        }

        // Disable game UI
        gameUI.SetActive(false);

        lrs.Clear();

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

        UIHover(retryBtn.gameObject);
    }
    // Replay button
    public void Replay()
    {
        canReturn = false;

        DestroyAllRemainingNotes();
        pausedUI.SetActive(false);
        gamePaused = false;
        EndTrack(true);

        tutorialUI.SetActive(false);
    }

    // This function only happens when the game needs to restart from either DYING or RESTARTING or GOING TO MAP SELECTION
    void DestroyAllRemainingNotes()
    {
        // For every line renderer for electricity still alive, destroy it
        for (int i = 0; i < tc.notesObj.transform.childCount; i++)
        {
            if (tc.notesObj.transform.GetChild(i).GetComponent<Note>().lrObj)
            {
                Destroy(tc.notesObj.transform.GetChild(i).GetComponent<Note>().lrObj);
            }
        }

        DestroyAllNotes(true);

        tc.notes.Clear();
        playerScript.closestBehindNote = null;
        playerScript.activeNotes.Clear();
        playerScript.notesInfront.Clear();
        playerScript.electricNotes.Clear();
        // remove this note from the 'furthestbehindnote' variable
        playerScript.closestBehindNote = null;
    }
    void PostMapStatistics()
    {
        // Disable game UI
        gameUI.SetActive(false);

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
        }
        // Calculate the post map stat UI
        postScoreText.text = score.ToString();
        postTotalAccuracyText.text = totalAccuracy.ToString("F2") + "%";
        perfectAmountText.text = perfects.ToString() + "x";
        greatAmountText.text = greats.ToString() + "x";
        goodAmountText.text = goods.ToString() + "x";
        missAmountText.text = misses.ToString() + "x";
        postComboText.text = comboMulti.ToString() + "x";

        trackName.text = tc.selectedMap.title.ToString() + " - " + tc.selectedMap.difficulty.ToString();

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
        if (tc.selectedMap.title == "Tutorial")
        {
            am.StopSound("Tutorial");
            am.StopSound("Tut_Stage_" + tutorialStage.ToString());
        }
        // Enable the post map stats UI to see
        postMapStatsUI.SetActive(true);

        // Enable post map buttons to see
        postMapUI.SetActive(true);

        activeScene = "PostMap";

        #region Disable All UI from Tutorial

        tutorialUI.SetActive(false);
        #endregion

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

        UIHover(menuBtn.gameObject);
    }

    // Happens when the player presses quit
    public void ExitGamePrompt()
    {
        mainMenuUI.SetActive(false);
        exitPromptUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("ExitGamePromptContr");
        }

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
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

        UIHover(yesBtn.gameObject);
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

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
        }
    }

    public void PlayBtn()
    {
        mapSelectionUI.SetActive(true);

        if (controllerConnected)
        {
            StartCoroutine("MapSelectionContr");
        }

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
        }

        activeScene = "MapSelection";
    }

    // map selection button
    public void MapSelection()
    {
        canReturn = false;

        // Stop the tutorial's UI from displaying after the map has been closed
        if (tc.selectedMap && tc.selectedMap.title == "Tutorial")
        {
            am.StopSound("Tut_Stage_" + tutorialStage.ToString());
            if (tc.activeCoroutine != null)
            {
                StopCoroutine(tc.activeCoroutine);
            }
        }

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

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
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

        UIHover(map1Btn.gameObject);
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

        // Enable the blue background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
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

        UIHover(playBtn.gameObject);
    }

    public void EndTrack(bool retry)
    {
        #region Tutorial reset
        doneTutStageCount = 0;
        if (tc.activeCoroutine != null)
        {
            StopCoroutine(tc.activeCoroutine);
        }
        #endregion

        jScript.DisableJet();
        //Debug.Log("ending");

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
        totalAllNotes = 0;
        lrs.Clear();

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
        tc.index = 1;
        resetPosition = false;

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

        doneOnce2 = false;
        noNotesInFront = false;

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

            // Enable the blue background image
            if (!blur.activeSelf)
            {
                blur.SetActive(true);
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
        // Destroy all notes that are still alive if the player ended the map
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
        if (activeScene == "Game")
        {
            if (!cantPause)
            {
                // Input to pause the game
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9") && !mapSelectionUI.activeSelf && !countingDown && !gamePaused && !tc.allNotes[tc.allNotes.Count - 1].GetComponent<Note>().behindPlayer)
                {
                    if (tc.selectedMap.title == "Tutorial")
                    {
                        PauseGame(false);
                    }
                    else
                    {
                        PauseGame(false);
                    }
                }
            }

            // Input to resume the game
            else if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown("joystick button 2")) && gamePaused && !countingDown)
            {
                //Debug.Log("2");
                UnPauseGameBut();
            }
        }

        // If player presses escape in the main menu screen, send to exit game prompt
        if (Input.GetKeyDown(KeyCode.Escape) && mainMenuUI.activeSelf)
        {
            //Debug.Log("3");
            ExitGamePrompt();
        }

        // If player presses escape in the map selection screen, send to main menu
        if (Input.GetKeyDown(KeyCode.Escape) && mapSelectionUI.activeSelf)
        {
            //Debug.Log("4");
            MainMenu();
        }
    }

    IEnumerator enableReturnFromPause()
    {
        yield return new WaitForSecondsRealtime(.4f);
        canReturn = true;
    }

    // If fate is true, the pause is for the end of a map
    // if fate is false, it's for the tutorial stage pauses
    public void PauseGame(bool tutPause)
    {
        StartCoroutine(enableReturnFromPause());


        am.PauseSound(tc.selectedMap.title);

        AudioListener.pause = true;

        for (int i = 0; i < am.menuSources.Count; i++)
        {
            am.menuSources[i].ignoreListenerPause = true;
        }

        //Time.timeScale = 0;
        // Enable the blur background image
        if (!blur.activeSelf)
        {
            blur.SetActive(true);
        }

        activeScene = "Paused";

        if (tutPause)
        {
            //StopCoroutine(tc.activeCoroutine);
            //MapSelection();
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

        UIHover(continueBtn.gameObject);
    }

    // This exists so the continue button in the pause menu can be linked to this
    public void UnPauseGameBut()
    {
        if (tc.selectedMap.title == "Tutorial")
        {
            StartCoroutine(UnpauseGame(true));
        }
        else
        {
            StartCoroutine(UnpauseGame(false));
        }
    }

    public IEnumerator UnpauseGame(bool tut)
    {
        // Disable the blue background image
        blur.SetActive(false);

        canReturn = false;

        if (!tut)
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
            countingDown = false;
            gamePaused = false;
            cantPause = false;

            AudioListener.pause = false;
            am.UnPause(tc.selectedMap.title);

            activeScene = "Game";
        }
        else
        {
            pausedUI.SetActive(false);
            gamePaused = false;

            cantPause = false;

            AudioListener.pause = false;
            am.UnPause(tc.selectedMap.title);

            am.UnPause("Tut_Stage_" + tutorialStage.ToString());

            activeScene = "Game";
        }

    }

    public void UIHover(GameObject btn)
    {
        if (lastSelectedButton == btn)
        {
            return;
        }
        
        am.PlaySound("UI_Hover");

        if (btn.name.Contains("Map"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(1f, 1f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            
            // Change the colour of the button
            btn.GetComponent<Image>().color = highlightedColor;
            for (int i = 0; i < btn.transform.childCount; i++)
            {
                btn.transform.GetChild(i).GetComponent<Text>().color = Color.white;
            }
        }
        else if (!btn.name.Contains("Back"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(1.2f, 1.2f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = highlightedColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        }
        
        if (btn.name.Contains("Back"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(.75f, .75f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = highlightedColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        }

    }

    public void UILeave(GameObject btn)
    {
        if (lastSelectedButton == btn)
        {
            return;
        }

        if (lastSelectedButton)
        {
            lastSelectedButton.GetComponent<MapInfo>().selected = false;
        }

        if (btn.name.Contains("Map"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(.75f, .75f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = defMapColor;
            for (int i = 0; i < btn.transform.childCount; i++)
            {
                btn.transform.GetChild(i).GetComponent<Text>().color = Color.black;
            }
        }
        else if (!btn.name.Contains("Back"))
        {
            // Change the colour of the button
            btn.GetComponent<Image>().color = defColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;

            // Change the size of the button
            LeanTween.scale(btn, new Vector3(1f, 1f, 1), .2f).setEase(LeanTweenType.easeInOutBack);
        }

        if (btn.name.Contains("Back"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(.5f, .5f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = defColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
    }

    public void UISelected(GameObject btn)
    {
        am.PlaySound("UI_Select");

        if (lastSelectedButton)
        {
            if (lastSelectedButton != btn)
            {
                // Change the size of the button
                LeanTween.scale(lastSelectedButton, new Vector3(.75f, .75f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

                // Change the colour of the button
                lastSelectedButton.GetComponent<Image>().color = defMapColor;
                for (int i = 0; i < lastSelectedButton.transform.childCount; i++)
                {
                    lastSelectedButton.transform.GetChild(i).GetComponent<Text>().color = Color.black;
                }
            }
        }

        if (btn.name.Contains("Map"))
        {
            if (lastSelectedButton != btn)
            {
                lastSelectedButton = btn;
            }
            else
            {
                lastSelectedButton.GetComponent<MapInfo>().selected = true;
            }

            // Change the size of the button
            LeanTween.scale(btn, new Vector3(.9f, .9f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = selectedColor;

            for (int i = 0; i < btn.transform.childCount; i++)
            {
                btn.transform.GetChild(i).GetComponent<Text>().color = Color.white;
            }
        }
        else if (!btn.name.Contains("Back"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(1f, 1f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = defColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }

        if (btn.name.Contains("Back"))
        {
            // Change the size of the button
            LeanTween.scale(btn, new Vector3(.75f, .75f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

            // Change the colour of the button
            btn.GetComponent<Image>().color = defColor;
            btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }

        if (btn.name.Contains("Quit"))
        {
            ExitGamePrompt();
            return;
        }
        else if (btn.name.Contains("Play"))
        {
            MapSelection();
            return;
        }
        else if (btn.name.Contains("Yes"))
        {
            ExitGame();
            return;
        }
        else if (btn.name.Contains("No"))
        {
            CancelGamePrompt();
            return;
        }
        else if (btn.name.Contains("Retry") || btn.name.Contains("Restart"))
        {
            Replay();
            return;
        }
        else if (btn.name.Contains("Menu"))
        {
            MapSelection();
            return;
        }
        else if (btn.name.Contains("Map"))
        {
            tc.SelectMap(btn.GetComponent<Button>());
            if (lastSelectedButton.GetComponent<MapInfo>().selected)
            {
                // Change the size of the button
                LeanTween.scale(lastSelectedButton, new Vector3(.75f, .75f, 1), .2f).setEase(LeanTweenType.easeInOutBack);

                // Change the colour of the button
                lastSelectedButton.GetComponent<Image>().color = defMapColor;
                for (int i = 0; i < lastSelectedButton.transform.childCount; i++)
                {
                    lastSelectedButton.transform.GetChild(i).GetComponent<Text>().color = Color.black;
                }

                lastSelectedButton.GetComponent<MapInfo>().selected = false;
                lastSelectedButton = null;

                tc.LoadTrack();
            }
            return;
        }
        else if (btn.name.Contains("Back"))
        {
            MainMenu();
            return;
        }
        else if (btn.name.Contains("Resume"))
        {
            UnPauseGameBut();
            return;
        }
    }
}
