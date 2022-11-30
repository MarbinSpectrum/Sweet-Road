using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ��ϵ��� �����մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class BlockGroup : FieldObjectSingleton<BlockGroup>
{
    [SerializeField] private List<BlockObj> blocks = new List<BlockObj>();
    private BlockObj[,] blockArray;

    //����� ���Ϲ���
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ϸ� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<LevelEditor.SaveBlockData> pEBlockDatas)
    {
        //���� �����͸� �޾ƿ´�.
        float blockWidth = 0;
        float blockHeight = 0;
        int mapWidth = 0;
        int mapHeight = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
            centerPos = boardManager.centerPos;
        }

        blockArray = new BlockObj[mapWidth + 1, mapHeight + 1];

        for (int idx = 0; idx < pEBlockDatas.Count; idx++)
        {
            if (blocks.Count <= idx)
            {
                //�����Ͱ� ����.
                break;
            }

            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                //none�� ������� �ǹ��Ѵ�.
                //����� �������� �ʴ´�.
                continue;
            }
    
            //����� ��ġ�� ����
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform blockTrans = blocks[idx].transform;
            blockTrans.position = tilePos;

            //��� �ʱ�ȭ
            blocks[idx].InitBlock(blockType);

            //�ش� ��ġ�� ����� ���
            blockArray[blockX, blockY] = blocks[idx];
        }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش���ġ�� ����� �ҷ��´�.
    ////////////////////////////////////////////////////////////////////////////////
    public BlockObj GetBlock(Vector2Int pPos)
    {
        return GetBlock(pPos.x,pPos.y);
    }
    public BlockObj GetBlock(int pX, int pY)
    {
        if (MyLib.Exception.IndexOutRange(pX, pY, blockArray) == false)
        {
            return null;
        }
        return blockArray[pX, pY];
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ����� ��ü�Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator SwapBlockEvent(Vector2Int pPos0, Vector2Int pPos1, bool pMatch = true)
    {
        BlockObj block0 = GetBlock(pPos0);
        BlockObj block1 = GetBlock(pPos1);
        if (block0 == null || block1 == null)
        {
            yield break;
        }

        //���� �����͸� �޾ƿ´�.
        float moveTime = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            moveTime = boardManager.moveTime;
        }

        //����� �ڵ������ �̵���Ų��.(�迭�� ����)
        BlockObj temp = block0;
        blockArray[pPos0.x, pPos0.y] = block1;
        blockArray[pPos1.x, pPos1.y] = temp;

        //����� �̵��� ���� ���س�����.
        Vector3 vectorPos0 = block0.transform.position;
        Vector3 vectorPos1 = block1.transform.position;

        //�̵� �ִϸ��̼� ����
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos0.x, pPos0.y].transform, vectorPos0, moveTime));
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos1.x, pPos1.y].transform, vectorPos1, moveTime));
        yield return new WaitForSeconds(moveTime);

        if(pMatch == true)
        {
            bool matchBlock = boardManager.MatchBlockEvent();
            if (matchBlock == false)
            {
                StartCoroutine(SwapBlockEvent(pPos0, pPos1, false));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ����� ��ü�Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator BlockMoveEvent()
    {
        //Ÿ�� �����͸� �޾ƿ´�.
        TileGroup tileGroup = TileGroup.instance;
        if(tileGroup == null)
        {
            yield break;
        }

        //���� �����͸� �޾ƿ´�.
        int mapWidth = 0;
        int mapHeight = 0;
        float blockWidth = 0;
        float blockHeight = 0;
        Vector2 centerPos = Vector2.zero;
        float moveTime = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            moveTime = boardManager.moveTime;
            centerPos = boardManager.centerPos;
        }
        else
        {
            yield break;
        }

        List<Vector2Int> from = new List<Vector2Int>();
        List<Vector2Int> to = new List<Vector2Int>();

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                BlockObj block = blockArray[x,y];
                if (block == null)
                    continue;

                int by = y % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.
                for (int idx = 0; idx < 3; idx++)
                {
                    int nextX = x + dropDic[by][idx, 0];
                    int nextY = y + dropDic[by][idx, 1];
                    if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                    {
                        BlockObj nBlock = blockArray[nextX, nextY];
                        bool isTile = tileGroup.IsTile(nextX, nextY);

                        if (nBlock == null && isTile)
                        {
                            //Ÿ���� �κ��̰�
                            //������ �κп� ����� ����.
                            from.Add(new Vector2Int(x, y));
                            to.Add(new Vector2Int(nextX, nextY));
                            blockArray[nextX, nextY] = blockArray[x, y];
                            blockArray[x, y] = null;
                            continue;
                        }
                    }
                }
            }
        }

        //�̵� �ִϸ��̼� ����
        for(int idx = 0; idx < from.Count; idx++)
        {
            BlockObj block0 = GetBlock(to[idx]);

            Vector2 toPos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, to[idx].x, to[idx].y);
            toPos += centerPos;

            StartCoroutine(MyLib.Action2D.MoveTo(
            block0.transform, toPos, moveTime));
        }

        yield return new WaitForSeconds(moveTime);

        if(CheckDownBlock() == true)
        {
            //���� ���������� ����� �����Ѵ�.
            StartCoroutine(BlockMoveEvent());
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ������ �����ϴ��� �˻��Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool CheckDownBlock()
    {
        //Ÿ�� �����͸� �޾ƿ´�.
        TileGroup tileGroup = TileGroup.instance;
        if (tileGroup == null)
        {
            return false;
        }

        //���� �����͸� �޾ƿ´�.
        int mapWidth = 0;
        int mapHeight = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
        }
        else
        {
            return false;
        }

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                BlockObj block = blockArray[x, y];
                if (block == null)
                    continue;

                int by = y % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.
                for (int idx = 0; idx < 3; idx++)
                {
                    int nextX = x + dropDic[by][idx, 0];
                    int nextY = y + dropDic[by][idx, 1];
                    if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                    {
                        BlockObj nBlock = blockArray[nextX, nextY];
                        bool isTile = tileGroup.IsTile(nextX, nextY);

                        if (nBlock == null && isTile)
                        {
                            //Ÿ���� �κ��̰�
                            //������ �κп� ����� ����.
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
