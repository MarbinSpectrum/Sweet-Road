using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 타일들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class TileGroup : FieldObjectSingleton<TileGroup>
{
    [SerializeField] private List<TileObj> tiles = new List<TileObj>();
    private int[,] isTile;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 타일맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTileMap(List<LevelEditor.SaveBlockData> pEBlockDatas)
    {
        //보드 데이터를 받아온다.
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
                //타일이 없다.
                continue;
            }
            if (pEBlockDatas.Count <= idx)
            {
                //데이터가 없다.
                break;
            }

            //none은 빈공간을 의미한다.
            //타일을 생성하지 않는다.
            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                tiles[idx].DisableTile();
                continue;
            }

            //타일의 위치를 설정
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform tileTrans = tiles[idx].transform;
            tileTrans.position = tilePos;

            //타일초기화
            tiles[idx].InitTile(sortOrder++);

            //해당 위치에는 타일이 있다는 것을 표시
            isTile[blockX, blockY] = -1;
        }

        int[][,] aroundPos = new int[][,]
        {
            new int[,]{{0, 2}, {+1, +1}, {1, -1}, {0, -2}, { +0, -1}, {+0, +1}},
            new int[,]{{0, 2}, {+0, +1}, {0, -1}, {0, -2}, { -1, -1}, {-1, +1}},
        };




    //타일을 그룹별로 나눈다.
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
    /// : 해당위치에 타일이 있는지 검사
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
    /// : 타일의 지붕에 해당하는 위치를 얻는다.
    ////////////////////////////////////////////////////////////////////////////////
    public Vector2Int GetTileRoot(int pX, int pY)
    {
        Vector2Int rootPos = new Vector2Int(-1, -1);
        int tileGroupNum = isTile[pX, pY];

        //보드 데이터를 받아온다.
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
                //타일의 밖은 타일의 지붕이 아니다
                continue;
            }

            if (tileGroupNum != isTile[pX, y])
            {
                //같은 그룹이 아니다.
                continue;
            }

            if (y + 2 <= mapHeight)
            {
                bool tileCap = IsTile(pX, y+2);
                if (tileCap == true)
                {
                    //아직 외각선은 아니다.
                    continue;
                }
                rootPos.Set(pX, y);
            }
        }
        return rootPos;
    }
}
