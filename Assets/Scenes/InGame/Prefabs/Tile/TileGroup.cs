using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : Ÿ�ϵ��� �����մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TileGroup : FieldObjectSingleton<TileGroup>
{
    [SerializeField] private List<TileObj> tiles = new List<TileObj>();
    private int[,] isTile;

    ////////////////////////////////////////////////////////////////////////////////
    /// : Ÿ�ϸ� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTileMap(List<LevelEditor.SaveBlockData> pEBlockDatas)
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

        isTile = new int[mapWidth + 1, mapHeight + 1];
        int sortOrder = 0;

        for (int idx = 0; idx < tiles.Count; idx++)
        {
            if (tiles[idx] == null)
            {
                //Ÿ���� ����.
                continue;
            }
            if (pEBlockDatas.Count <= idx)
            {
                //�����Ͱ� ����.
                break;
            }

            //none�� ������� �ǹ��Ѵ�.
            //Ÿ���� �������� �ʴ´�.
            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                tiles[idx].DisableTile();
                continue;
            }

            //Ÿ���� ��ġ�� ����
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform tileTrans = tiles[idx].transform;
            tileTrans.position = tilePos;

            //Ÿ���ʱ�ȭ
            tiles[idx].InitTile(sortOrder++);

            //�ش� ��ġ���� Ÿ���� �ִٴ� ���� ǥ��
            isTile[blockX, blockY] = -1;
        }

        int[][,] aroundPos = new int[][,]
        {
            new int[,]{{0, 2}, {+1, +1}, {1, -1}, {0, -2}, { +0, -1}, {+0, +1}},
            new int[,]{{0, 2}, {+0, +1}, {0, -1}, {0, -2}, { -1, -1}, {-1, +1}},
        };




    //Ÿ���� �׷캰�� ������.
    int tileGroup = 1;
        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                if(isTile[x, y] == -1)
                {
                    Queue<Vector2Int> tileQueue =
                        new Queue<Vector2Int>();
                    tileQueue.Enqueue(new Vector2Int(x, y));
                    isTile[x, y] = tileGroup;

                    while (tileQueue.Count > 0)
                    {
                        Vector2Int nowQ = tileQueue.Dequeue();
                        for(int i = 0; i < 6; i++)
                        {
                            int by = nowQ.y % 2;
                            int nextX = nowQ.x + aroundPos[by][i, 0];
                            int nextY = nowQ.y + aroundPos[by][i, 1];
                            if (MyLib.Exception.IndexOutRange
                                (nextX, nextY, isTile) == false)
                            {
                                continue;
                            }
                            if(isTile[nextX, nextY] != -1)
                            {
                                continue;
                            }
                            tileQueue.Enqueue(new Vector2Int(nextX, nextY));
                            isTile[nextX, nextY] = tileGroup;
                        }
                    }
                    tileGroup++;
                }
            }
        }
        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش���ġ�� Ÿ���� �ִ��� �˻�
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsTile(int pX, int pY)
    {
        if (MyLib.Exception.IndexOutRange(pX, pY, isTile) == false)
        {
            return false;
        }
        return isTile[pX, pY] > 0;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : Ÿ���� ���ؿ� �ش��ϴ� ��ġ�� ��´�.
    ////////////////////////////////////////////////////////////////////////////////
    public Vector2Int GetTileRoot(int pX, int pY)
    {
        Vector2Int rootPos = new Vector2Int(-1, -1);
        int tileGroupNum = isTile[pX, pY];

        //���� �����͸� �޾ƿ´�.
        int mapHeight = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapHeight = boardManager.mapHeight;
        }

        for (int y = pY; y <= mapHeight; y += 2)
        {
            bool exitTile = IsTile(pX, y);
            if (exitTile == false)
            {
                //Ÿ���� ���� Ÿ���� ������ �ƴϴ�
                continue;
            }

            if (tileGroupNum != isTile[pX, y])
            {
                //���� �׷��� �ƴϴ�.
                continue;
            }

            if (y + 2 <= mapHeight)
            {
                bool tileCap = IsTile(pX, y+2);
                if (tileCap == true)
                {
                    //���� �ܰ����� �ƴϴ�.
                    continue;
                }
                rootPos.Set(pX, y);
            }
        }
        return rootPos;
    }
}
