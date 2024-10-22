using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private const string LevelStatsKey = "LevelStats";

    public void SaveStats(LevelStats levelStats)
    {
        List<LevelStats> statsList = LoadStats();

        // ���������, ���������� �� ��� ������ ��� ������� ������
        LevelStats existingStats = statsList.Find(stats => stats.levelNumber == levelStats.levelNumber);
        if (existingStats != null)
        {
            // ���� ������ ����������, ��������� �
            existingStats.score = levelStats.score;
            existingStats.isComplete = levelStats.isComplete;
            Debug.Log($"���������� ������������ ������ ��� ������ {levelStats.levelNumber} � ����� ������: {levelStats.score}");
        } 
        else
        {
            // ���� ������ ���, ��������� �����
            statsList.Add(levelStats);
            Debug.Log($"���������� ����� ������ ��� ������ {levelStats.levelNumber} � ������: {levelStats.score}");
        }

        // ��������� ����������� ������ � PlayerPrefs
        string json = JsonUtility.ToJson(new LevelStatsList { stats = statsList }, true);
        Debug.Log("��������������� JSON: " + json);

        PlayerPrefs.SetString(LevelStatsKey, json);
        PlayerPrefs.Save();
        Debug.Log("���������� ���������.");
    }

    public void ClearStats()
    {
        PlayerPrefs.DeleteKey(LevelStatsKey);
        PlayerPrefs.Save();
        Debug.Log("���������� �������.");
    }

    public List<LevelStats> LoadStats()
    {
        /*if (PlayerPrefs.HasKey(LevelStatsKey))
        {
            string json = PlayerPrefs.GetString(LevelStatsKey);
            Debug.Log(json);
            LevelStatsWrapper wrapper = JsonUtility.FromJson<LevelStatsWrapper>(json);
            Debug.Log("���������� ���������.");
            return wrapper.levelStats;
        }
        else
        {
            Debug.Log("���� �� ������, ���������� ������ ������.");
            return new List<LevelStats>(); // ���������� ������ ������
        }*/
        if (PlayerPrefs.HasKey(LevelStatsKey))
        {
            string json = PlayerPrefs.GetString(LevelStatsKey);
            Debug.Log(json);
            LevelStatsList wrapper = JsonUtility.FromJson<LevelStatsList>(json);

            if (wrapper != null && wrapper.stats != null)
            {
                return wrapper.stats;
            }
            else
            {
                return new List<LevelStats>();
            }
        }
        else
        {
            return new List<LevelStats>();
        }
    }

    [System.Serializable]
    private class LevelStatsWrapper
    {
        public List<LevelStats> levelStats;

        public LevelStatsWrapper(List<LevelStats> levelStats)
        {
            this.levelStats = levelStats;
        }
    }

    [System.Serializable]
    public class LevelStatsList
    {
        public List<LevelStats> stats;
    }
}
