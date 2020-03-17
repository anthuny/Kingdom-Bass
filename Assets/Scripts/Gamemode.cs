using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    private TrackCreator tc;
    private AudioManager am;
    public GameObject player;
    public Player playerScript;
    [HideInInspector]
    public GameObject jet;
    public float jetZ;
    public float jetY;

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

    [Header("Map Buttons")]
    public Button[] mapButtons;
    public GameObject mapSelectionUI;

    [Header("Game UI")]
    public Text[] gameUI;

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
    public Text healthText;
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

    [Header("Accuracy")]
    public int perfectScore;
    public int goodScore;
    public int badScore;
    public int missScore;

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

    [Header("Health")]
    public float healthMax;
    public float regenPerfect;
    public float regenGreat;
    public float regenGood;
    public float regenMiss;
    public float regenBomb;
    public float healthRegen;
    public float healthRegenInterval;
    public float audioSpeedDec;
    public float minPitchDec;
    public float minPitchVolDecStart;
    private AudioSource activeTrack;
    float t = 1;
    float y = 1;
    public GameObject gameOverUI;
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

    [Header("UI Camera")]
    public GameObject UICam;

    [Header("Notes")]
    public float distPercArrowLock;
    public int totalNotes;

    [Header("Electricity")]
    public LineRenderer lr;
    public Transform electricityStart;
    public ParticleSystemRenderer ElectrictyBallPS;
    public Color missedColour;
    public Color readyColour;
    public Color stealthColor;
    public Gradient stealthGradient;
    public Gradient readyGradient;

    [Header("Bombs")]
    public float bombHitRange;

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        UICam.SetActive(true);

        player = FindObjectOfType<Player>().gameObject;
        playerScript = player.GetComponent<Player>();
        jet = FindObjectOfType<Jet>().gameObject;
        tc = FindObjectOfType<TrackCreator>();
        am = FindObjectOfType<AudioManager>();

        UpdateUI();

        Application.targetFrameRate = targetFps;

        greatMax = perfectMin + 0.01f;
        goodMax = greatMax + 0.01f;
        missMax = goodMax + 0.01f;

        perfectMin = 1 - perfectMin;
        greatMin = 1 - greatMin;
        goodMin = 1 - goodMin;

        health = healthMax;

        ToggleGameUI(false);
        StartGame();

        //UnityEditor.EditorPrefs.SetBool("DeveloperMode", true);
    }

    public void StartGame()
    {
        totalAccuracy = 100;
        totalAccuracyText.text = "Total Accuracy: " + totalAccuracy.ToString() + "%";

        lr.gameObject.SetActive(true);
        lr.SetPosition(0, electricityStart.transform.position);
        lr.gameObject.SetActive(false);
        gameOverUI.SetActive(false);
        postMapUI.SetActive(false);
        pausedUI.SetActive(false);
        countdownText.gameObject.SetActive(false);
        countingDown = false;
        //cantPause = true;

        mapSelectText.text = selectAMapText;

        playerScript.nearestLaneNumber = 3;
        playerScript.oldNearestLaneNumber = 2;

        originalFirstUIPos = firstUI.gameObject.GetComponent<RectTransform>().localPosition;
        originalSecondUIPos = secondUI.gameObject.GetComponent<RectTransform>().localPosition;

        //pressKeyToContinueEnd.gameObject.SetActive(false);
        postMapStatsUI.SetActive(false);

        if (tc.selectedMap)
        {
            // Update health altered values based on the health drain value of the map
            regenGood *= tc.selectedMap.healthDrain;
            regenGreat *= tc.selectedMap.healthDrain;
            regenPerfect *= tc.selectedMap.healthDrain;
            regenMiss *= tc.selectedMap.healthDrain;
            regenBomb *= tc.selectedMap.healthDrain;

            // begin the loop for health regeneration
            InvokeRepeating("UpdateHealthRegen", 0, healthRegenInterval);
        }
    }

    void UpdateHealthRegen()
    {
        UpdateHealth(healthRegen);
    }
    void Update()
    {
        UpdateShield();
        UpdateElectricity();
        TutorialUnpause();
        PlayerDeath();
        PauseInput();

        jetZ = jetDistance + player.transform.position.z;
        jet.transform.position = new Vector3(0, jetY, jetZ);

        // If there is a change in score, Update the UI
        if (score != oldScore)
        {
            //comboMulti += 1;
            UpdateUI();
        }

        currentFps = 1.0f / Time.deltaTime;

        fpsCounterText.text = Mathf.FloorToInt(currentFps).ToString();

        if (playerScript.nearestNote && !doneOnce)
        {
            doneOnce = true;
            playerScript.newGoodMiss = goodMin / (playerScript.nearestNoteScript.beatWait / accuracy);
        }
    }

    void UpdateElectricity()
    {
        // Update the elctricity's start location when the player moves
        if (playerScript.oldPlayerPos != playerScript.playerPos)
        {
            playerScript.oldPlayerPos = playerScript.playerPos;

            lr.SetPosition(0, electricityStart.transform.position);
        }

        // Make the end electricity's end point above the nearest note
        if (playerScript.closestNoteInFrontScript)
        {
            // If there is a note infront of the player, continue
            if (playerScript.notesInfront.Count > 0)
            {
                lr.SetPosition(1, playerScript.closestNoteInFrontScript.ElectrictyEnd.transform.position);
            }
        }

        // Set the colour of the laser ball to the ready colour when it should be
        if (playerScript.isShielding && playerScript.notesInfront.Count < 1)
        {
            lr.colorGradient = readyGradient;

            ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);
        }

        // Set the colour of the laser ball to the stealth colour when it should be
        else if (!playerScript.isShielding || playerScript.notesInfront.Count < 1)
        {
            if (playerScript.closestNoteInFrontScript)
            {
                playerScript.closestNoteInFrontScript.doneElecrictyEffect = 0;
            }

            ElectrictyBallPS.material.SetColor("Color_CFB35B33", stealthColor * 15);

            lr.colorGradient = stealthGradient;

            if (playerScript.closestNoteInFrontScript)
            {
                if (!playerScript.closestNoteInFrontScript.missed)
                {
                    lr.startWidth = 1;
                    lr.endWidth = 1;
                }
            }
        }

        // If there is no note infront of the player, disable the electricity
        if (playerScript.notesInfront.Count < 1)
        {
            lr.startWidth = 0;
            lr.endWidth = 0;
            lr.SetPosition(1, new Vector3(3, 0.81f, 70));
            lr.gameObject.SetActive(false);
            return;
        }
        // If there is a note infront of the player, enable the electricity
        else
        {
            lr.gameObject.SetActive(true);
        }

        // If there is no note in front of the player, do not continue
        if (!playerScript.closestNoteInFrontScript)
        {
            return;
        }

        // Set the colour of the laser ball to the ready colour when it should be
        if (playerScript.closestNoteInFrontScript.doneElecrictyEffect == 0 && playerScript.isShielding)
        {
            playerScript.closestNoteInFrontScript.doneElecrictyEffect = 1;
            lr.gameObject.SetActive(true);
            lr.startWidth = 1;
            lr.endWidth = 1;


            ElectrictyBallPS.material.SetColor("Color_CFB35B33", readyColour * 15);

            lr.colorGradient = readyGradient;
        }

        // If the player got a miss for the nearest note infront, set the colour of the laser ball to the missed colour
        // And disable the electricity
        else if (playerScript.closestNoteInFrontScript.missed && playerScript.closestNoteInFrontScript.doneElecrictyEffect == 1)
        {
            playerScript.closestNoteInFrontScript.doneElecrictyEffect = 2;
            lr.startWidth = 0;
            lr.endWidth = 0;

            ElectrictyBallPS.material.SetColor("Color_CFB35B33", missedColour * 15);
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
    public void UpdateUI()
    {
        if (score != oldScore && tc.notesSpawned > 0)
        {
            comboMulti += 1;
        }
        oldScore = score;
        scoreIncreased = true;

        accuracyText.text = "Perfect " + perfects.ToString() +
            "\nGreat " + greats.ToString() +
            "\nGood " + goods.ToString() +
            "\nMiss " + misses.ToString();
        comboText.text = "Combo x " + comboMulti.ToString();
        if (health >= 0)
        {
            healthText.text = "Health " + health.ToString();
        }
        else
        {
            healthText.text = "Health 0";
        }

        scoreText.text = "Score " + score.ToString();
    }

    public void UpdateHealth(float amount)
    {
        if (health <= 0 && !playerDead)
        {
            killingPlayer = true;
            health = 0f;
            PlayerDeath();
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
            UpdateUI();
        }
    }

    void PlayerDeath()
    {
        if (killingPlayer)
        {
            playerDead = true;

            // If the pitch of the track has not yet reached it's minimum:
            if (t > minPitchDec)
            {
                // Get access to the active track playing
                foreach (AudioSource aSource in am.gameObject.GetComponents<AudioSource>())
                {
                    if (aSource.clip.name == tc.selectedMap.title)
                    {
                        if (aSource.isPlaying)
                        {
                            activeTrack = aSource;
                        }
                    }
                }

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
        ToggleGameUI(false);

        DestroyAllRemainingNotes();

        gameOverUI.SetActive(true);

        AudioListener.pause = true;
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
        // remove this note from the 'furthestbehindnote' variable
        playerScript.furthestBehindNote = null;
    }
    void PostMapStatistics()
    {
        // Disable game UI
        ToggleGameUI(false);

        // Calculate the post map stat UI
        endScoreText.text = "Score " + score.ToString();
        endTotalAccuracyText.text = "Accuracy " + totalAccuracy.ToString() + "%";
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
    }

    // main menu button
    public void SendToMapSelection()
    {
        DestroyAllRemainingNotes();
        pausedUI.SetActive(false);
        gamePaused = false;
        cantPause = true;
        EndTrack(false);
    }
    public void EndTrack(bool retry)
    {
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
        ToggleGameUI(false);

        t = 1;
        y = 1;

        if (activeTrack)
        {
            activeTrack.pitch = 1;
            activeTrack.volume = 1;
            activeTrack.Stop();
        }

        tc.notesSpawned = 0;
        tc.allNotes.Clear();
        tc.allNoteTypes.Clear();
        tc.beatWaitCountAccum.Clear();

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

            // If the player reached the end of the map
            if (!playerDead && health <= 0)
            {
                DestroyAllNotes(false);
            }
            playerDead = false;

            mapSelectionUI.SetActive(true);
        }
        else
        {
            //DestroyAllRemainingNotes();
            StartCoroutine(tc.LoadTrackCo());
        }

        playerScript.RepositionPlayer();

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

        // Display the total accuracy UI only in 2 decimal places
        totalAccuracyText.text = "Total Accuracy: " + totalAccuracy.ToString("F2") + "%";
        //Debug.Break();
    }

    public void ToggleGameUI(bool fate)
    {
        foreach (Text t in gameUI)
        {
            t.gameObject.SetActive(fate);
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
        // Input to pause the game
        if (Input.GetKeyDown(KeyCode.Escape) && !mapSelectionUI.activeSelf && !countingDown && !gamePaused && !tc.allNotes[tc.allNotes.Count - 1].GetComponent<Note>().behindPlayer && !cantPause)
        {
            PauseGame(false);
        }
        // Input to resume the game
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused && !countingDown)
        {
            StartCoroutine(UnpauseGame());
        }
    }
    // If fate is true, the pause is for the end of a map
    // if fate is false, it's for the tutorial stage pauses
    public void PauseGame(bool tutPause)
    {
        AudioListener.pause = true;

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
        }
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
