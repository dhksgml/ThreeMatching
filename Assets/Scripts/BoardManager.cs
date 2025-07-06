using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject[] blockPrefabs;    //������ Ÿ�� ������
    public Transform boardHolder;   //�θ� ������Ʈ

    private Block[,] allBlocks;
    private Block selectedBlock = null;

    private bool isProcessing = false; //���� �� �Է� ����� ���� ����

    private void Start()
    {
        allBlocks = new Block[width, height];
        CreateBoard();
        CenterCamera(); //ī�޶� �߾� ����
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
                block.type = (BlockType)index;
                block.SetPosition(x, y);
                allBlocks[x, y] = block;
            }
        }
    }
    //ī�޶� ���� �߽� ����
    void CenterCamera()
    {
        float camX = (width - 1) / 2f;
        float camY = (height - 1) / 2f;
        Camera.main.transform.position = new Vector3(camX, camY, -10f);
    }

    //���� ũ�⿡ ���� ī�޶� �� ����
    void AdjustCameraZoom()
    {
        float margin = 1f;
        float sizeX = width / (2f * Camera.main.aspect);
        float sizeY = height / 2f;
        Camera.main.orthographicSize = Mathf.Max(sizeX, sizeY) + margin;
    }

    public void SelectBlock(Block block)
    {
        if (isProcessing) return;

        if (selectedBlock == null)
        {
            selectedBlock = block;
            block.Highlight(true);
        }
        else
        {
            if (IsAdjacent(selectedBlock, block))
            {
                isProcessing = true;
                StartCoroutine(SwapAndCheck(selectedBlock, block));
            }
            else
            {
                selectedBlock.Highlight(false);
                selectedBlock = block;
                block.Highlight(true);
            }
        }
    }

    private IEnumerator SwapAndCheck(Block a, Block b)
    {
        SwapBlocks(a, b);

        yield return new WaitForSeconds(0.2f);

        List<Block> matched = FindMatches();
        if (matched.Count > 0)
        {
            RemoveMatches(matched);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(HandleBoardAfterMatchCoroutine());
        }
        else
        {
            SwapBlocks(a, b);
        }

        a.Highlight(false);
        b.Highlight(false);
        selectedBlock = null;
        isProcessing = false;
    }

    private bool IsAdjacent(Block a, Block b)
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

        //1. �� ��� ��ġ ����
        a.SetPosition(bX, bY);
        b.SetPosition(aX, aY);

        //2. �迭�� �ٲ� ��ġ ����
        allBlocks[a.x, a.y] = a;
        allBlocks[b.x, b.y] = b;

        /*
        //3. ��Ī �˻�
        List<Block> matched = FindMatches();
        if (matched.Count > 0)
        {
            //��Ī ���� �� ��Ī ó��
            RemoveMatches(matched);
            StartCoroutine(HandleBoardAfterMatchCoroutine());
        }
        else
        {
            //��Ī ���� �� ������� �ǵ���
            a.SetPosition(aX, aY);
            b.SetPosition(bX, bY);

            allBlocks[aX, aY] = a;
            allBlocks[bX, bY] = b;
        }

        a.Highlight(false);
        b.Highlight(false);
        */
    }

    private List<Block> FindMatches()
    {
        List<Block> matchedBlocks = new List<Block>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Block current = allBlocks[x, y];
                if (current == null) continue;

                //���� �˻�
                if (x < width - 2)
                {
                    Block b1 = allBlocks[x + 1, y];
                    Block b2 = allBlocks[x + 2, y];
                    if (b1 != null && b2 != null &&
                        current.type == b1.type && current.type == b2.type)
                    {
                        if (!matchedBlocks.Contains(current)) matchedBlocks.Add(current);
                        if (!matchedBlocks.Contains(b1)) matchedBlocks.Add(b1);
                        if (!matchedBlocks.Contains(b2)) matchedBlocks.Add(b2);
                    }
                }

                //���� �˻�
                if (y < height - 2)
                {
                    Block b1 = allBlocks[x, y + 1];
                    Block b2 = allBlocks[x, y + 2];
                    if (b1 != null && b2 != null &&
                        current.type == b1.type && current.type == b2.type)
                    {
                        if (!matchedBlocks.Contains(current)) matchedBlocks.Add(current);
                        if (!matchedBlocks.Contains(b1)) matchedBlocks.Add(b1);
                        if (!matchedBlocks.Contains(b2)) matchedBlocks.Add(b2);
                    }
                }
            }
        }
        return matchedBlocks;
    }

    private void RemoveMatches(List<Block> matched)
    {
        foreach (Block b in matched)
        {
            if (b == null) continue;

            if (allBlocks[b.x, b.y] == b)
                allBlocks[b.x, b.y] = null;

            Destroy(b.gameObject);
        }
    }

    private void DropBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                if (allBlocks[x, y] == null)
                {
                    for (int ny = y + 1; ny < height; ny++)
                    {
                        Block blockToDrop = allBlocks[x, ny];

                        if (blockToDrop == null || blockToDrop.gameObject == null)
                            continue;

                        allBlocks[x, y] = blockToDrop;
                        allBlocks[x, ny] = null;

                        blockToDrop.SetPosition(x, y);
                        break;
                    }
                }
            }
        }
    }

    private void FillEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x, y] == null)
                {
                    int index = Random.Range(0, blockPrefabs.Length);
                    GameObject prefab = blockPrefabs[index];

                    GameObject newBlockObj = Instantiate(prefab, new Vector2(x, y + 1), Quaternion.identity, boardHolder);
                    Block newBlock = newBlockObj.GetComponent<Block>();
                    newBlock.type = (BlockType)index;
                    newBlock.SetPosition(x, y);

                    allBlocks[x, y] = newBlock;
                }
            }
        }
    }

    private IEnumerator HandleBoardAfterMatchCoroutine()
    {
        DropBlocks();
        FillEmptySpaces();

        yield return null;

        //���� ��Ī Ž��
        List<Block> nextMatch = FindMatches();
        if (nextMatch.Count > 0)
        {
            RemoveMatches(nextMatch);
            yield return StartCoroutine(HandleBoardAfterMatchCoroutine());
        }
    }
}
