using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////////////////////////////////////////////////////////////////////////////////
/// : 저장할 블록 데이터
////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public struct SaveBlockData
{
    public Vector2Int pos;
    public BlockType blockType;
    public SpecialType specialType;
    public BlockDic blockDic;

    public SaveBlockData(Vector2Int pPos, BlockType pBlockType,
        SpecialType pSpecialType,BlockDic pBlockDic)
    {
        pos = pPos;
        blockType = pBlockType;
        specialType = pSpecialType;
        blockDic = pBlockDic;
    }
}

[System.Serializable]
public struct SaveTargetData
{
    public int targetNum;
    public BlockType blockType;
    public SpecialType specialType;

    public SaveTargetData(BlockType pBlockType, SpecialType pSpecialType, int ptargetNum)
    {
        blockType = pBlockType;
        specialType = pSpecialType;
        targetNum = ptargetNum;
    }
}

[System.Serializable]
public class LevelData
{
    public int moveCnt;

    public List<SaveBlockData> blockDatas;
    public List<SaveTargetData> targetDatas;

    public LevelData(List<EditorBlock> pBlocks, HashSet<SaveTargetData> pTargetBlocks, int pMoveCnt)
    {
        blockDatas = new List<SaveBlockData>();
        foreach (EditorBlock editorBlock in pBlocks)
        {
            Vector2Int pos = new Vector2Int(editorBlock.posX, editorBlock.posY);
            BlockType blockType = editorBlock.blockType;
            SpecialType specialType = editorBlock.specialType;
            BlockDic blockDic = editorBlock.blockDic;

            blockDatas.Add(new SaveBlockData(pos, blockType, specialType, blockDic));
        }

        targetDatas = new List<SaveTargetData>();
        foreach (SaveTargetData saveTargetData in pTargetBlocks)
        {
            BlockType blockType = saveTargetData.blockType;
            SpecialType specialType = saveTargetData.specialType;
            int cnt = saveTargetData.targetNum;

            targetDatas.Add(new SaveTargetData(blockType,specialType, cnt));
        }

        moveCnt = pMoveCnt;
    }
}
