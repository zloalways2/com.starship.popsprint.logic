using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIManager : MonoBehaviour
{
    public GameObject statsDisplayPrefab; // ������ ��� ����������� ����������
    public RectTransform content; // ���������� ScrollRect
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
        DisplayStats(); // ��������� �����������
    }

    public float verticalSpacing = 1025f; // ���������� ����� ���������� (�������� �������� �� ��������� �� ������ ����������)

    public void DisplayStats()
    {
        levelStats = statsManager.LoadStats();
        Debug.Log($"��������� ����������: {levelStats.Count} ���������");
        foreach (var stat in levelStats)
        {
            Debug.Log($"�������: {stat.levelNumber}, ����: {stat.score}");
        }

        // ������� ������ ��������
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        float height = 0f; // �������������� ������
        float currentY = 0f; // ������� ������� Y

        foreach (LevelStats stats in levelStats)
        {
            GameObject display = Instantiate(statsDisplayPrefab, content);
            LevelStatsDisplay statsDisplay = display.GetComponent<LevelStatsDisplay>();
            statsDisplay.SetStats(stats);

            // �������� ������ �������� ������������� ��������
            float elementHeight = statsDisplay.GetComponent<RectTransform>().rect.height;

            // ������������� ������� �������� ��������
            RectTransform rectTransform = display.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, currentY);

            // ��������� ������ � ������� ������� Y
            height += elementHeight - verticalSpacing; // �������� �� + ��� ����������� �������
            currentY -= elementHeight - verticalSpacing; // �������� �� + ��� ����������� �������
        }

            // ������������� ������ content �� ��������� ������ ���������� ���������
        content.sizeDelta = new Vector2(content.sizeDelta.x, -height);

            // ��������� ScrollRect, ����� �� ������������ �� ��������� ������� content
        scrollRect.verticalNormalizedPosition = 1f; // ��������� �����
    }
}
