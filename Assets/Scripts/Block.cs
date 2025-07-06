using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}

public class Block : MonoBehaviour
{
    public BlockType type;
    public int x, y; //블록의 현재 위치
    private BoardManager boardManager;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnMouseDown()
    {
        boardManager.SelectBlock(this);
    }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
        transform.position = new Vector2(x, y);
    }

    public void Highlight(bool on)
    {
        if (sr != null)
            sr.color = on ? Color.gray : originalColor;
    }
}
