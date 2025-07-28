using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private float spacing = 0.1f;
    private BoardManager boardManager;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector2 targetPos;
    private Vector2 mouseStart;
    private float minDragDistance = 0.3f;

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
        if (boardManager.IsProcessing) return;

        if (boardManager.inputMode == InputMode.ClickToSelect)
        {
            if (boardManager.SelectedBlock == this)
                boardManager.CancelSelectedBlock();
            else
                boardManager.SelectBlock(this);
        }
        else if(boardManager.inputMode == InputMode.DragToSwap)
        {
            mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (boardManager.IsProcessing) return;
        if (boardManager.inputMode != InputMode.DragToSwap) return;

        Vector2 mouseEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mouseEnd - mouseStart;

        if (delta.magnitude < minDragDistance) return;

        Vector2 dir = Vector2.zero;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dir = (delta.x > 0) ? Vector2.right : Vector2.left;
        else
            dir = (delta.y > 0) ? Vector2.up : Vector2.down;

        int targetX = x + (int)dir.x;
        int targetY = y + (int)dir.y;
        if(boardManager.IsInside(targetX, targetY))
        {
            Block other = boardManager.GetBlock(targetX, targetY);
            if (other != null)
            {
                boardManager.RequestSwap(this, other);
            }
        }
    }

    public void SetSpacing(float s)
    {
        spacing = s;
    }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
        targetPos = new Vector2(x * (1 + spacing), y * (1 + spacing));

        transform.DOMove(targetPos, 0.2f)
            .SetEase(Ease.OutBack)
            .OnUpdate(() =>
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.y);
            });
    }

    public void Highlight(bool on)
    {
        if (sr != null)
            sr.color = on ? Color.gray : originalColor;
    }
}
