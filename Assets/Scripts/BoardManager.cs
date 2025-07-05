using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject[] blockPrefabs;    //생성할 타일 프리팹
    public Transform boardHolder;   //부모 오브젝트

    private Block[,] allBlocks;
    private Block selectedBlock = null;

    private void Start()
    {
        allBlocks = new Block[width, height];
        CreateBoard();
        CenterCamera(); //카메라 중앙 정렬
        AdjustCameraZoom();
    }

    void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = Random.Range(0, blockPrefabs.Length);
                GameObject blockObj = Instantiate(blockPrefabs[index], Vector2.zero, Quaternion.identity);
                blockObj.transform.parent = boardHolder;
                
                Block block = blockObj.GetComponent<Block>();
                block.SetPosition(x, y);
                allBlocks[x, y] = block;
            }
        }
    }
    //카메라 보드 중심 정렬
    void CenterCamera()
    {
        float camX = (width - 1) / 2f;
        float camY = (height - 1) / 2f;
        Camera.main.transform.position = new Vector3(camX, camY, -10f);
    }

    //보드 크기에 맞춰 카메라 줌 조절
    void AdjustCameraZoom()
    {
        float margin = 1f;
        float sizeX = width / (2f * Camera.main.aspect);
        float sizeY = height / 2f;
        Camera.main.orthographicSize = Mathf.Max(sizeX, sizeY) + margin;
    }

    public void SelectBlock(Block block)
    {
        if(selectedBlock == null)
        {
            selectedBlock = block;
            block.Highlight(true);
        }
        else
        {
            if(block == selectedBlock)
            {
                selectedBlock.Highlight(false);
                selectedBlock = null;
                return;
            }

            if(AreAdjacent(selectedBlock, block))
            {
                SwapBlocks(selectedBlock, block);
            }
            selectedBlock.Highlight(false);
            selectedBlock = null;
        }
    }

    private bool AreAdjacent(Block a, Block b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private void SwapBlocks(Block a, Block b)
    {
        int aX = a.x;
        int aY = a.y;
        int bX = b.x;
        int bY = b.y;

        a.SetPosition(bX, bY);
        b.SetPosition(aX, aY);

        allBlocks[a.x, a.y] = a;
        allBlocks[b.x, b.y] = b;

        a.Highlight(false);
        b.Highlight(false);
    }
}
