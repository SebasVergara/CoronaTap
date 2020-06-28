using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreTMPText : MonoBehaviour
{
    TextMeshProUGUI score;

    void OnEnable()
    {
        score = GetComponent<TextMeshProUGUI>();
        score.text = "Puntaje Máximo: " + PlayerPrefs.GetInt("HighScore").ToString();
    }
}