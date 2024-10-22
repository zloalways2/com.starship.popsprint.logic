[System.Serializable]
public class LevelStats
{
    public int levelNumber; // ����� ������
    public int score; // ��������� ����
    public string completionDate; // ���� ���������� ������
    public bool isComplete;

    public LevelStats(int levelNumber, int score, string completionDate, bool isComplete)
    {
        this.levelNumber = levelNumber;
        this.score = score;
        this.completionDate = completionDate;
        this.isComplete = isComplete;
    }
}
