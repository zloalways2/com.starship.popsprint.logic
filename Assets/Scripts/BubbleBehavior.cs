using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBehavior : MonoBehaviour
{
    public int GridX { get; set; }
    public int GridY { get; set; }
    private SpriteRenderer spriteRenderer; // ��� ����������� �������
    private Sprite currentSprite; // ������� ������ ��������

    Game _game;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _game = FindObjectOfType<Game>();
    }

    public void SetPosition(int x, int y)
    {
        GridX = x;
        GridY = y;
    }

    public void SetSprite(Sprite sprite)
    {
        currentSprite = sprite;
        spriteRenderer.sprite = sprite; // ������������� ������
    }

    public Sprite GetSprite()
    {
        return currentSprite; // ���������� ������� ������
    }

    private void OnMouseDown()
    {
        FindObjectOfType<Bubble>().CheckAndRemoveBubbles(this);
        _game.aEffsects.Play();
    }
}

