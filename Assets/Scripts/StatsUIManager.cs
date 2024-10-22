using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIManager : MonoBehaviour
{
    public GameObject statsDisplayPrefab; // Префаб для отображения статистики
    public RectTransform content; // Содержимое ScrollRect
    private StatsManager statsManager;
    private List<LevelStats> levelStats;
    public ScrollRect scrollRect;

    private void Awake()
    {
        statsManager = GetComponent<StatsManager>();
        DisplayStats();
    }

    public void OnClearStatsButtonClicked()
    {
        statsManager.ClearStats();
        DisplayStats(); // Обновляем отображение
    }

    public float verticalSpacing = 1025f; // Расстояние между элементами (измените значение по умолчанию по вашему усмотрению)

    public void DisplayStats()
    {
        levelStats = statsManager.LoadStats();
        Debug.Log($"Загружено статистики: {levelStats.Count} элементов");
        foreach (var stat in levelStats)
        {
            Debug.Log($"Уровень: {stat.levelNumber}, Счет: {stat.score}");
        }

        // Удаляем старые элементы
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        float height = 0f; // Инициализируем высоту
        float currentY = 0f; // Текущая позиция Y

        foreach (LevelStats stats in levelStats)
        {
            GameObject display = Instantiate(statsDisplayPrefab, content);
            LevelStatsDisplay statsDisplay = display.GetComponent<LevelStatsDisplay>();
            statsDisplay.SetStats(stats);

            // Получаем высоту текущего отображаемого элемента
            float elementHeight = statsDisplay.GetComponent<RectTransform>().rect.height;

            // Устанавливаем позицию текущего элемента
            RectTransform rectTransform = display.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, currentY);

            // Обновляем высоту и текущую позицию Y
            height += elementHeight - verticalSpacing; // Измените на + для правильного расчета
            currentY -= elementHeight - verticalSpacing; // Измените на + для правильного расчета
        }

            // Устанавливаем высоту content на основании общего количества элементов
        content.sizeDelta = new Vector2(content.sizeDelta.x, -height);

            // Обновляем ScrollRect, чтобы он отреагировал на изменение размера content
        scrollRect.verticalNormalizedPosition = 1f; // Прокрутка вверх
    }
}
