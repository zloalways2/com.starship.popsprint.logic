using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bubble : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Sprite[] bubbleSprites; // ������ �������� ��� ���������
    public RectTransform uiBoundary;
    public int rows = 5;
    public int columns = 5;
    public float bubbleSpacing = 1.0f;
    private List<BubbleBehavior> bubbles = new List<BubbleBehavior>();

    public TextMeshProUGUI scoreTxt;
    public int score = 0; // ���������� ��� �������� ������ �����
    private const int pointsPerBubble = 25; // ���������� ����� �� ������ ��������� �������

    Game _game; 
    private StatsManager statsManager;
    private List<LevelStats> levelStats;

    private void Start()
    {
        _game = FindObjectOfType<Game>();
        statsManager = GetComponent<StatsManager>();
        levelStats = statsManager.LoadStats();
    }

    public void HilightLevelBtn()
    {
        Debug.Log("HilightLevelBtn");
        foreach(LevelStats level in levelStats)
        {
            Debug.Log($"level: {level.levelNumber} => {level.isComplete} => '{"LevelChoiceBtn " + level.levelNumber}' => {GameObject.Find("LevelChoiceBtn " + level.levelNumber)}");
            if(level.isComplete)
            {
                GameObject levelBtn = GameObject.Find("LevelChoiceBtn " + level.levelNumber);
                if(levelBtn)
                {
                    levelBtn.GetComponent<Image>().color = Color.green;
                }
            }
        }
    }

    public void GenerateBubbles()
    {
        score = 0;
        UpdateScoreDisplay();
        Vector3[] corners = new Vector3[4];
        uiBoundary.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0]; // ����� ������ ����
        Vector3 topRight = corners[2]; // ������ ������� ����

        // ������������ ������ � ������ ������
        float width = topRight.x - bottomLeft.x + .2f;
        float height = topRight.y - bottomLeft.y + .2f;

        // ���������� �������� � �������� ������
        float bubbleSize = Mathf.Min(width / columns, height / rows) * .5f;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // ������������ ������� ��������
                float posX = bottomLeft.x + .2f + (x * bubbleSpacing) + (bubbleSpacing / 2);
                float posY = bottomLeft.y + .2f + (y * bubbleSpacing) + (bubbleSpacing / 2);
                Vector3 position = new Vector3(posX, posY, 0);

                // ���������, ��������� �� ������� ������ ������
                if (position.x >= bottomLeft.x - .2f && position.x <= topRight.x - .2f &&
                    position.y >= bottomLeft.y - .2f && position.y <= topRight.y - .2f)
                {
                    GameObject bubbleObject = Instantiate(bubblePrefab, position, Quaternion.identity);
                    RectTransform bubbleRectTransform = bubbleObject.GetComponent<RectTransform>();
                    bubbleRectTransform.sizeDelta = new Vector2(bubbleSize, bubbleSize); // ������������� ������ ��������

                    BubbleBehavior bubble = bubbleObject.GetComponent<BubbleBehavior>();
                    bubble.SetPosition(x, y);
                    bubble.SetSprite(bubbleSprites[Random.Range(0, bubbleSprites.Length)]); // ������������� ��������� ������
                    bubbles.Add(bubble);                    
                }
            }
        }
    }

    public void CheckAndRemoveBubbles(BubbleBehavior clickedBubble)
    {
        List<BubbleBehavior> bubblesToRemove = new List<BubbleBehavior>();
        FindConnectedBubbles(clickedBubble, bubblesToRemove);

        if (bubblesToRemove.Count >= 2)
        {
            score += bubblesToRemove.Count * pointsPerBubble;

            foreach (BubbleBehavior bubble in bubblesToRemove)
            {
                bubbles.Remove(bubble);
                Destroy(bubble.gameObject);
            }

            MoveBubblesDown(bubblesToRemove);
        } 
        UpdateScoreDisplay();

        Debug.Log($"CheckAndRemoveBubbles: Checking {bubbles.Count} bubbles.");
        CheckLevelCompletion(bubbles);
    }
    private void CheckLevelCompletion(List<BubbleBehavior> bubbles)
    {
        int singleBubblesCount = 0;
        Debug.Log($"CheckLevelCompletion: Checking {bubbles.Count} bubbles.");

        foreach (BubbleBehavior bubble in bubbles)
        {
            Debug.Log($"Checking bubble with ID: {bubble.GetInstanceID()}");

            List<BubbleBehavior> bubblesToRemove = new List<BubbleBehavior>();
            FindConnectedBubbles(bubble, bubblesToRemove);

            if (bubblesToRemove.Count == 1)
            {
                singleBubblesCount++;
                Debug.Log($"Bubble with ID: {bubble.GetInstanceID()} is a single bubble. Single bubble count: {singleBubblesCount}");
            }
        }

        Debug.Log($"Total single bubbles count: {singleBubblesCount}. Total bubbles count: {bubbles.Count}");

        // ���� �������� ������ ��������� �������� ��� �� ���, ������� ��������� ����������
        if (singleBubblesCount == bubbles.Count || bubbles.Count == 0)
        {
            Debug.Log("Level completed: All bubbles are single or no bubbles left.");
            _game.inGame = false;
            saveStat(true);
            _game.endLevel();
        }
        else
        {
            Debug.Log("Level not completed: Some bubbles still have neighbors.");
        }
    }


    public void saveStat(bool isComplete)
    {
            string completionDate = System.DateTime.Now.ToString("dd.MM.yyyy");
            int num;
            int.TryParse(_game.lvlName, out num);
            LevelStats stats = new LevelStats(num, score, completionDate, isComplete);
            levelStats.Add(stats);
            statsManager.SaveStats(stats);
    }

    private void UpdateScoreDisplay()
    {
        scoreTxt.text = "Score: " + score.ToString(); // ��������� ����� �� UI
    }

    private void FindConnectedBubbles(BubbleBehavior bubble, List<BubbleBehavior> bubblesToRemove)
    {
        Queue<BubbleBehavior> queue = new Queue<BubbleBehavior>();
        HashSet<BubbleBehavior> visited = new HashSet<BubbleBehavior>();

        queue.Enqueue(bubble);
        visited.Add(bubble);

        while (queue.Count > 0)
        {
            BubbleBehavior current = queue.Dequeue();
            bubblesToRemove.Add(current);

            foreach (BubbleBehavior neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && neighbor.GetSprite() == current.GetSprite()) // ���������� �������
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private IEnumerable<BubbleBehavior> GetNeighbors(BubbleBehavior bubble)
    {
        int x = bubble.GridX;
        int y = bubble.GridY;

        foreach (BubbleBehavior b in bubbles)
        {
            if (b.GridX == x && (b.GridY == y + 1 || b.GridY == y - 1) || // �����/����
                b.GridY == y && (b.GridX == x + 1 || b.GridX == x - 1)) // �����/������
            {
                yield return b;
            }
        }
    }

    private void MoveBubblesDown(List<BubbleBehavior> removedBubbles)
    {
        // ������� ������� ��� �������� ��������� �� ��������
        Dictionary<int, List<BubbleBehavior>> columnBubbles = new Dictionary<int, List<BubbleBehavior>>();

        // ��������� ������� ����������, ������� �������� ����� �������� 
        foreach (BubbleBehavior bubble in bubbles)
        {
            if (!removedBubbles.Contains(bubble))
            {
                if (!columnBubbles.ContainsKey(bubble.GridX))
                {
                    columnBubbles[bubble.GridX] = new List<BubbleBehavior>();
                }
                columnBubbles[bubble.GridX].Add(bubble);
            }
        }

        // ���������� �������� ���� �� ������ �������
        foreach (int column in columnBubbles.Keys)
        {
            List<BubbleBehavior> currentColumnBubbles = columnBubbles[column];

            // �������� � ������ ����� ������� � ��������� �����
            int emptySpace = 0;

            for (int y = 0; y < rows; y++)
            {
                // ���� ������� ������ �����
                if (currentColumnBubbles.Find(b => b.GridY == y) == null)
                {
                    emptySpace++;
                }
            else
                {
                    // ���� ������� ������, ���������� ��� �� ���������� ������ ����
                    BubbleBehavior bubble = currentColumnBubbles.Find(b => b.GridY == y);
                    if (bubble != null)
                    {
                        bubble.GridY -= emptySpace;
                        bubble.transform.position += Vector3.down * emptySpace * bubbleSpacing;
                    }
                }
            }
        }

        // �������� ���������� ������� �����, ���� ��� ������
        ShiftColumnsLeft(columnBubbles);
    }

    private void ShiftColumnsLeft(Dictionary<int, List<BubbleBehavior>> columnBubbles)
    {
        // ������� ��� ������ ������� 
        List<int> emptyColumns = new List<int>();

        for (int x = 0; x < columns; x++)
        {
            if (!columnBubbles.ContainsKey(x))
            {
                emptyColumns.Add(x);
            }
        }

        // �������� ���������� ������� ������
        foreach (int emptyColumn in emptyColumns)
        {
            for (int x = emptyColumn + 1; x < columns; x++)
            {
                // ������� �������� � ������� x 
                List<BubbleBehavior> currentColumnBubbles = columnBubbles.ContainsKey(x) ? columnBubbles[x] : new List<BubbleBehavior>();

                foreach (BubbleBehavior bubble in currentColumnBubbles)
                {
                    // �������� �������� ����� �� ���� �������
                    bubble.GridX--;
                    bubble.transform.position -= Vector3.right * bubbleSpacing;
                }

                // ��������� �������� � ����� ������� � �������
                if (!columnBubbles.ContainsKey(x - 1))
                {
                    columnBubbles[x - 1] = new List<BubbleBehavior>();
                }
                columnBubbles[x - 1].AddRange(currentColumnBubbles);
            }
        }

        // ������� ������ ������� �� �������
        foreach (int emptyColumn in emptyColumns)
        {
            columnBubbles.Remove(emptyColumn);
        }
    }

    public void clearAll()
    {
        // ������� ��� ������� � ����� "Bubble"
        GameObject[] remainingBubbles = GameObject.FindGameObjectsWithTag("Bubble");

        // ������� ������ �������
        foreach (GameObject bubble in remainingBubbles)
        {
            Destroy(bubble);
        }
        bubbles.Clear();
    }

    public void HideBubbles()
    {
        foreach (BubbleBehavior bubble in bubbles)
        {
            bubble.gameObject.SetActive(false); // �������� �������� }
            Debug.Log("��� ���������� �������� ������.");
        }
    }

    public void ShowBubbles()
    {
        foreach (BubbleBehavior bubble in bubbles)
        {
            bubble.gameObject.SetActive(true); // ���������� �������� }
            Debug.Log("��� ���������� �������� ��������.");
        }
    }
}

