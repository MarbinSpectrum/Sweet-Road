using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : Ÿ�ϵ��� �����մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TileGroup : FieldObjectSingleton<TileGroup>
{
    [SerializeField] private List<TileObj> tiles = new List<TileObj>();
    private bool[,] isTile;

    ////////////////////////////////////////////////////////////////////////////////
    /// : Ÿ�ϸ� �ʱ�ȭ
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
                (pBlockWidth, pBlockHeight, blockX, blockY);
            tilePos += pCenterPos;

            Transform tileTrans = tiles[idx].transform;
            tileTrans.position = tilePos;

            //Ÿ���ʱ�ȭ
            tiles[idx].InitTile(sortOrder++);

            //�ش� ��ġ���� Ÿ���� �ִٴ� ���� ǥ��
            isTile[blockX, blockY] = true;
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
        return isTile[pX, pY];
    }
}
