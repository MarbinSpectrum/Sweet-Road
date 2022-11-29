using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 타일들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class TileGroup : FieldObjectSingleton<TileGroup>
{
    [SerializeField] private List<TileObj> tiles = new List<TileObj>();
    private bool[,] isTile;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 타일맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTileMap(List<LevelEditor.SaveBlockData> pEBlockDatas, Vector2 pCenterPos,
        float pBlockWidth, float pBlockHeight,int pMapWidth,int pMapHeight)
    {
        isTile = new bool[pMapWidth + 1, pMapHeight];
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
                (pBlockWidth, pBlockHeight, blockX, blockY);
            tilePos += pCenterPos;

            Transform tileTrans = tiles[idx].transform;
            tileTrans.position = tilePos;

            //타일초기화
            tiles[idx].InitTile(sortOrder++);

            //해당 위치에는 타일이 있다는 것을 표시
            isTile[blockX, blockY] = true;
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
        return isTile[pX, pY];
    }
}
