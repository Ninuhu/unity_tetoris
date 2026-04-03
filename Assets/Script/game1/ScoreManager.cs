using System.Collections.Generic;
using UnityEngine;
public static class ScoreManager
{
    private const int RankCount =5;
    public static string mode = "Classic"; // 今のモード名いれる

    public static void AddScore(int score)
    {
        List<int> scores =LoadScores();

        scores.Add(score);

        scores.Sort((a,b) => b.CompareTo(a));

        if(scores.Count > RankCount)
            scores.RemoveRange(RankCount,scores.Count - RankCount);

        SaveScores(scores);
    }

    static void SaveScores(List<int> scores)
    {
        for(int i =0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt(mode + "_Rank" +i,scores[i]);
        }

        PlayerPrefs.Save();
    }

    public static List<int> LoadScores()
    {
        List<int> scores =new List<int>();

        for(int i =0; i < RankCount; i++)
        {
            scores.Add(PlayerPrefs.GetInt(mode +"_Rank" +i,0));
        }

        return scores;
    }
}