[System.Serializable]
public class LevelStats
{
    public int levelNumber; // Номер уровня
    public int score; // Набранные очки
    public string completionDate; // Дата завершения уровня
    public bool isComplete;

    public LevelStats(int levelNumber, int score, string completionDate, bool isComplete)
    {
        this.levelNumber = levelNumber;
        this.score = score;
        this.completionDate = completionDate;
        this.isComplete = isComplete;
    }
}
