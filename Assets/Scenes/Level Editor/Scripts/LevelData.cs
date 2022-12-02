using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////////////////////////////////////////////////////////////////////////////////
/// : 저장할 블록 데이터
////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public struct BlockData
{
    public Vector2Int pos;
    public BlockType blockType;
    public SpecialType specialType;
    public BlockDic blockDic;

    public BlockData(Vector2Int pPos, BlockType pBlockType,
        SpecialType pSpecialType,BlockDic pBlockDic)
    {
        pos = pPos;
        blockType = pBlockType;
        specialType = pSpecialType;
        blockDic = pBlockDic;
    }
}

[System.Serializable]
public struct TargetData
{
    public int targetNum;
    public BlockType blockType;
    public SpecialType specialType;

    public TargetData(BlockType pBlockType, SpecialType pSpecialType, int ptargetNum)
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

    public List<BlockData> blockDatas;
    public List<TargetData> targetDatas;

    public LevelData(List<EditorBlock> pBlocks, HashSet<TargetData> pTargetBlocks, int pMoveCnt)
    {
        blockDatas = new List<BlockData>();
        foreach (EditorBlock editorBlock in pBlocks)
        {
            Vector2Int pos = new Vector2Int(editorBlock.posX, editorBlock.posY);
            BlockType blockType = editorBlock.blockType;
            SpecialType specialType = editorBlock.specialType;
            BlockDic blockDic = editorBlock.blockDic;

            blockDatas.Add(new BlockData(pos, blockType, specialType, blockDic));
        }

        targetDatas = new List<TargetData>();
        foreach (TargetData targetData in pTargetBlocks)
        {
            BlockType blockType = targetData.blockType;
            SpecialType specialType = targetData.specialType;
            int cnt = targetData.targetNum;

            targetDatas.Add(new TargetData(blockType,specialType, cnt));
        }

        moveCnt = pMoveCnt;
    }
}
