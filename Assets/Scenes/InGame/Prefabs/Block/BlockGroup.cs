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

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ϸ� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<LevelEditor.SaveBlockData> pEBlockDatas, Vector2 pCenterPos,
        float pBlockWidth, float pBlockHeight, int pMapWidth, int pMapHeight)
    {
        blockArray = new BlockObj[pMapWidth + 1, pMapHeight];

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
                (pBlockWidth, pBlockHeight, blockX, blockY);
            tilePos += pCenterPos;

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
    public BlockObj GetBlock(int pX,int pY)
    {
        if (MyLib.Exception.IndexOutRange(pX, pY, blockArray) == false)
        {
            return null;
        }
        return blockArray[pX, pY];
    }
}
