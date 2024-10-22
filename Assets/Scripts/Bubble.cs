using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bubble : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Sprite[] bubbleSprites; // Массив спрайтов для пузырьков
    public RectTransform uiBoundary;
    public int rows = 5;
    public int columns = 5;
    public float bubbleSpacing = 1.0f;
    private List<BubbleBehavior> bubbles = new List<BubbleBehavior>();

    public TextMeshProUGUI scoreTxt;
    public int score = 0; // Переменная для хранения общего счета
    private const int pointsPerBubble = 25; // Количество очков за каждый удаленный пузырек

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
        Vector3 bottomLeft = corners[0]; // Левый нижний угол
        Vector3 topRight = corners[2]; // Правый верхний угол

        // Рассчитываем ширину и высоту границ
        float width = topRight.x - bottomLeft.x + .2f;
        float height = topRight.y - bottomLeft.y + .2f;

        // Генерируем пузырьки в пределах границ
        float bubbleSize = Mathf.Min(width / columns, height / rows) * .5f;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Рассчитываем позицию пузырька
                float posX = bottomLeft.x + .2f + (x * bubbleSpacing) + (bubbleSpacing / 2);
                float posY = bottomLeft.y + .2f + (y * bubbleSpacing) + (bubbleSpacing / 2);
                Vector3 position = new Vector3(posX, posY, 0);

                // Проверяем, находится ли позиция внутри границ
                if (position.x >= bottomLeft.x - .2f && position.x <= topRight.x - .2f &&
                    position.y >= bottomLeft.y - .2f && position.y <= topRight.y - .2f)
                {
                    GameObject bubbleObject = Instantiate(bubblePrefab, position, Quaternion.identity);
                    RectTransform bubbleRectTransform = bubbleObject.GetComponent<RectTransform>();
                    bubbleRectTransform.sizeDelta = new Vector2(bubbleSize, bubbleSize); // Устанавливаем размер пузырька

                    BubbleBehavior bubble = bubbleObject.GetComponent<BubbleBehavior>();
                    bubble.SetPosition(x, y);
                    bubble.SetSprite(bubbleSprites[Random.Range(0, bubbleSprites.Length)]); // Устанавливаем случайный спрайт
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

        // Если остались только одиночные пузырьки или их нет, уровень считается пройденным
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
        scoreTxt.text = "Score: " + score.ToString(); // Обновляем текст на UI
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
                if (!visited.Contains(neighbor) && neighbor.GetSprite() == current.GetSprite()) // Сравниваем спрайты
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
            if (b.GridX == x && (b.GridY == y + 1 || b.GridY == y - 1) || // Вверх/вниз
                b.GridY == y && (b.GridX == x + 1 || b.GridX == x - 1)) // Влево/вправо
            {
                yield return b;
            }
        }
    }

    private void MoveBubblesDown(List<BubbleBehavior> removedBubbles)
    {
        // Создаем словарь для хранения пузырьков по колонкам
        Dictionary<int, List<BubbleBehavior>> columnBubbles = new Dictionary<int, List<BubbleBehavior>>();

        // Заполняем словарь пузырьками, которые остались после удаления 
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

        // Перемещаем пузырьки вниз по каждой колонке
        foreach (int column in columnBubbles.Keys)
        {
            List<BubbleBehavior> currentColumnBubbles = columnBubbles[column];

            // Начинаем с нижней части колонки и двигаемся вверх
            int emptySpace = 0;

            for (int y = 0; y < rows; y++)
            {
                // Если текущая ячейка пуста
                if (currentColumnBubbles.Find(b => b.GridY == y) == null)
                {
                    emptySpace++;
                }
            else
                {
                    // Если пузырек найден, перемещаем его на количество пустых мест
                    BubbleBehavior bubble = currentColumnBubbles.Find(b => b.GridY == y);
                    if (bubble != null)
                    {
                        bubble.GridY -= emptySpace;
                        bubble.transform.position += Vector3.down * emptySpace * bubbleSpacing;
                    }
                }
            }
        }

        // Сдвигаем оставшиеся колонки влево, если они пустые
        ShiftColumnsLeft(columnBubbles);
    }

    private void ShiftColumnsLeft(Dictionary<int, List<BubbleBehavior>> columnBubbles)
    {
        // Находим все пустые колонки 
        List<int> emptyColumns = new List<int>();

        for (int x = 0; x < columns; x++)
        {
            if (!columnBubbles.ContainsKey(x))
            {
                emptyColumns.Add(x);
            }
        }

        // Сдвигаем оставшиеся колонки вправо
        foreach (int emptyColumn in emptyColumns)
        {
            for (int x = emptyColumn + 1; x < columns; x++)
            {
                // Находим пузырьки в колонке x 
                List<BubbleBehavior> currentColumnBubbles = columnBubbles.ContainsKey(x) ? columnBubbles[x] : new List<BubbleBehavior>();

                foreach (BubbleBehavior bubble in currentColumnBubbles)
                {
                    // Сдвигаем пузырьки влево на одну колонку
                    bubble.GridX--;
                    bubble.transform.position -= Vector3.right * bubbleSpacing;
                }

                // Переносим пузырьки в новую колонку в словарь
                if (!columnBubbles.ContainsKey(x - 1))
                {
                    columnBubbles[x - 1] = new List<BubbleBehavior>();
                }
                columnBubbles[x - 1].AddRange(currentColumnBubbles);
            }
        }

        // Удаляем пустые колонки из словаря
        foreach (int emptyColumn in emptyColumns)
        {
            columnBubbles.Remove(emptyColumn);
        }
    }

    public void clearAll()
    {
        // Находим все объекты с тегом "Bubble"
        GameObject[] remainingBubbles = GameObject.FindGameObjectsWithTag("Bubble");

        // Удаляем каждый пузырек
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
            bubble.gameObject.SetActive(false); // Скрываем пузырьки }
            Debug.Log("Все оставшиеся пузырьки скрыты.");
        }
    }

    public void ShowBubbles()
    {
        foreach (BubbleBehavior bubble in bubbles)
        {
            bubble.gameObject.SetActive(true); // Показываем пузырьки }
            Debug.Log("Все оставшиеся пузырьки показаны.");
        }
    }
}

