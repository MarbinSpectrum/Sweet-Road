using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 블록을 표시합니다.
////////////////////////////////////////////////////////////////////////////////
public class BlockObj : SerializedMonoBehaviour
{
    public BlockType blockType;
    [SerializeField] private SpriteRenderer blockRenderer;
    [SerializeField] private Animation blockAnimation;

    [SerializeField]
    private Dictionary<BlockType, Sprite> blockSprite
        = new Dictionary<BlockType, Sprite>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록객체 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public void InitBlock(LevelEditor.BlockType pEBlockType)
    {
        string str = pEBlockType.ToString();
        BlockType blockType;
        if(Enum.TryParse(str,out blockType))
        {
            //파싱 성공
            InitBlock(blockType);
        }
    }
    public void InitBlock(BlockType pBlockType)
    {
        blockType = pBlockType;
        blockRenderer.sprite = blockSprite[blockType];
        blockAnimation.Play();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 위치교환이 가능한지 검사
    ////////////////////////////////////////////////////////////////////////////////
    public bool CanSwap()
    {
        switch(blockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
            case BlockType.spin:
            case BlockType.rocket:
                return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치되서 파괴 되는 타입인가?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsMatchBlock()
    {
        switch (blockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
            case BlockType.rocket:
                return true;
            case BlockType.spin:
                return false;
        }
        return false;
    }
}

public enum BlockType
{
    red,
    orange,
    yellow,
    green,
    blue,
    purple,
    spin,
    rocket
}