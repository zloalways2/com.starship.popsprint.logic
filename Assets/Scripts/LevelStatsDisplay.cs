using UnityEngine;
using TMPro;

public class LevelStatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dateText;

    public void SetStats(LevelStats stats)
    {
        levelText.text = stats.levelNumber.ToString();
        scoreText.text = stats.score.ToString();
        dateText.text = stats.completionDate.ToString();
    }
}
