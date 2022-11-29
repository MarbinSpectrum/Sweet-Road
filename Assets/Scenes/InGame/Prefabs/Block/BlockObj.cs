using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : ����� ǥ���մϴ�.
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
    /// : ��ϰ�ü �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitBlock(LevelEditor.BlockType pEBlockType)
    {
        string str = pEBlockType.ToString();
        BlockType blockType;
        if(Enum.TryParse(str,out blockType))
        {
            //�Ľ� ����
            InitBlock(blockType);
        }
    }
    public void InitBlock(BlockType pBlockType)
    {
        blockType = pBlockType;
        blockRenderer.sprite = blockSprite[blockType];
        blockAnimation.Play();
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