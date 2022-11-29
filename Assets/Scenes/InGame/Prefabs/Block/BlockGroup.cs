using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 블록들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class BlockGroup : FieldObjectSingleton<BlockGroup>
{
    [SerializeField] private List<BlockObj> blocks = new List<BlockObj>();
    private BlockObj[,] blockArray;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<LevelEditor.SaveBlockData> pEBlockDatas, Vector2 pCenterPos,
        float pBlockWidth, float pBlockHeight, int pMapWidth, int pMapHeight)
    {
        blockArray = new BlockObj[pMapWidth + 1, pMapHeight];

        for (int idx = 0; idx < pEBlockDatas.Count; idx++)
        {
            if (blocks.Count <= idx)
            {
                //데이터가 없다.
                break;
            }

            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                //none은 빈공간을 의미한다.
                //블록을 생성하지 않는다.
                continue;
            }
    
            //블록의 위치를 설정
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (pBlockWidth, pBlockHeight, blockX, blockY);
            tilePos += pCenterPos;

            Transform blockTrans = blocks[idx].transform;
            blockTrans.position = tilePos;

            //블록 초기화
            blocks[idx].InitBlock(blockType);

            //해당 위치에 블록을 등록
            blockArray[blockX, blockY] = blocks[idx];
        }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당위치의 블록을 불러온다.
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
