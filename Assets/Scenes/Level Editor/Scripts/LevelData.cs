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
    public class LevelData
    {
        public List<SaveBlockData> blockDatas;
        public LevelData(List<EditorBlock> pBlocks)
        {
            blockDatas = new List<SaveBlockData>();
            foreach (EditorBlock editorBlock in pBlocks)
            {
                blockDatas.Add(new SaveBlockData(
                    new Vector2Int(editorBlock.posX, editorBlock.posY),
                    editorBlock.blockType));
            }
        }
    }
}