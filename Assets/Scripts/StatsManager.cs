using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private const string LevelStatsKey = "LevelStats";

    public void SaveStats(LevelStats levelStats)
    {
        List<LevelStats> statsList = LoadStats();

        // Проверяем, существует ли уже запись для данного уровня
        LevelStats existingStats = statsList.Find(stats => stats.levelNumber == levelStats.levelNumber);
        if (existingStats != null)
        {
            // Если запись существует, обновляем её
            existingStats.score = levelStats.score;
            existingStats.isComplete = levelStats.isComplete;
            Debug.Log($"Обновление существующей записи для уровня {levelStats.levelNumber} с новым счетом: {levelStats.score}");
        } 
        else
        {
            // Если записи нет, добавляем новую
            statsList.Add(levelStats);
            Debug.Log($"Добавление новой записи для уровня {levelStats.levelNumber} с счетом: {levelStats.score}");
        }

        // Сохраняем обновленный список в PlayerPrefs
        string json = JsonUtility.ToJson(new LevelStatsList { stats = statsList }, true);
        Debug.Log("Сериализованный JSON: " + json);

        PlayerPrefs.SetString(LevelStatsKey, json);
        PlayerPrefs.Save();
        Debug.Log("Статистика сохранена.");
    }

    public void ClearStats()
    {
        PlayerPrefs.DeleteKey(LevelStatsKey);
        PlayerPrefs.Save();
        Debug.Log("Статистика очищена.");
    }

    public List<LevelStats> LoadStats()
    {
        /*if (PlayerPrefs.HasKey(LevelStatsKey))
        {
            string json = PlayerPrefs.GetString(LevelStatsKey);
            Debug.Log(json);
            LevelStatsWrapper wrapper = JsonUtility.FromJson<LevelStatsWrapper>(json);
            Debug.Log("Статистика загружена.");
            return wrapper.levelStats;
        }
        else
        {
            Debug.Log("Файл не найден, возвращаем пустой список.");
            return new List<LevelStats>(); // Возвращаем пустой список
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
