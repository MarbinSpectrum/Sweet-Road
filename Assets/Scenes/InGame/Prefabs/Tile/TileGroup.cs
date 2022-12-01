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

        isTile = new bool[mapWidth + 1, mapHeight + 1];
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

            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                //none�� ������� �ǹ��Ѵ�.
                //Ÿ���� �������� �ʴ´�.
                tiles[idx].DisableTile();
                continue;
            }

            if (blockType == LevelEditor.BlockType.spawn)
            {
                //spawn�� ����� �����ϴ� �κ��̴�.
                //Ÿ���� �������� �ʴ´�.
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
