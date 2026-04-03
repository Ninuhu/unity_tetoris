using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingUI : MonoBehaviour
{
    public TextMeshProUGUI[] rankTexts;

    void Start()
    {
        List<int> scores = ScoreManager.LoadScores();

        for(int i = 0; i < rankTexts.Length; i++)
        {
            rankTexts[i].text = i + 1 + ": " + scores[i];
        }
    }
}