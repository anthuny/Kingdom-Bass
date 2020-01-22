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

    public Text scoreText;
    public float launchRotAmount;
    public float launchRotTime;

    public Color horizontalNoteArrowC;
    public Color horizontalLaunchArrowC;
    public Color upArrowC;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        jet = FindObjectOfType<Jet>().gameObject;

        UpdateUI();

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
    }

    void UpdateUI()
    {
        scoreText.text = "Score | " + score.ToString();
    }
}
