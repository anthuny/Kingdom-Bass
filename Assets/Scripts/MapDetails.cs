using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDetails : MonoBehaviour
{
    public Map map;

    [Header("UI")]
    public Text titleText;
    public Text difficultyText;
    public Text lengthText;
    public Text subGenreText;
    public Text bpmText;
    public Image stars;

    public bool selected;

    private void Start()
    {
        titleText.text = map.title;
        difficultyText.text = "Difficulty:   " + map.difficulty.ToString();
        lengthText.text = map.length.ToString();
        subGenreText.text = map.subGenre;
        bpmText.text = "BPM " + map.bpm.ToString() + "  |";
        stars.fillAmount = map.starCount / 8;
    }
}
