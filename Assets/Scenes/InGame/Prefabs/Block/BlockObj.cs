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
    public void InitBlock(LevelEditor.BlockType pEBlockType, bool pAniRun = true)
    {
        string str = pEBlockType.ToString();
        BlockType blockType;
        if(Enum.TryParse(str,out blockType))
        {
            //�Ľ� ����
            InitBlock(blockType);
        }
    }
    public void InitBlock(BlockType pBlockType, bool pAniRun = true)
    {
        blockType = pBlockType;
        if (blockRenderer != null)
        {
            blockRenderer.gameObject.SetActive(true);
            blockRenderer.sprite = blockSprite[blockType];
        }
        if(pAniRun == true)
        {
            blockAnimation.Play();
        }
    }

    public void InitBlock(bool pAniRun = true)
    {
        BlockType blockType =
            (BlockType)UnityEngine.Random.Range(0, 6);
        InitBlock(blockType, pAniRun);
    }

    public void DisableBlock()
    {
        if (blockRenderer != null)
        {
            //Ÿ�� ��Ȱ��ȭ
            blockRenderer.gameObject.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ��ȯ�� �������� �˻�
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
    /// : ��ġ�Ǽ� �ı� �Ǵ� Ÿ���ΰ�?
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