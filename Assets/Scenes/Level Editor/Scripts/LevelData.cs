using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    ////////////////////////////////////////////////////////////////////////////////
    /// : 저장할 블록 데이터
    ////////////////////////////////////////////////////////////////////////////////
    [System.Serializable]
    public struct SaveBlockData
    {
        public Vector2Int pos;
        public BlockType blockType;
        public SaveBlockData(Vector2Int pPos, BlockType pBlockType)
        {
            pos = pPos;
            blockType = pBlockType;
        }
    }

    [System.Serializable]
    public struct SaveTargetData
    {
        public int targetNum;
        public BlockType blockType;
        public SaveTargetData(BlockType pBlockType, int ptargetNum)
        {
            blockType = pBlockType;
            targetNum = ptargetNum;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public int moveCnt;

        public List<SaveBlockData> blockDatas;
        public List<SaveTargetData> targetDatas;

        public LevelData(List<EditorBlock> pBlocks,
            Dictionary<BlockType, int> pTargetBlocks, int pMoveCnt)
        {
            blockDatas = new List<SaveBlockData>();
            foreach (EditorBlock editorBlock in pBlocks)
            {
                blockDatas.Add(new SaveBlockData(
                    new Vector2Int(editorBlock.posX, editorBlock.posY),
                    editorBlock.blockType));
            }

            targetDatas = new List<SaveTargetData>();
            foreach (KeyValuePair<BlockType, int> pTargetBlockPair in pTargetBlocks)
            {
                BlockType blockType = pTargetBlockPair.Key;
                int cnt = pTargetBlockPair.Value;
                targetDatas.Add(new SaveTargetData(blockType, cnt));
            }

            moveCnt = pMoveCnt;
        }
    }
}