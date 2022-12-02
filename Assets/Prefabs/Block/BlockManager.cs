using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BlockManager : FieldObjectSingleton<BlockManager>
{
    [SerializeField] private Dictionary<BlockType, Dictionary<SpecialType, Sprite>> blockSprite
        = new Dictionary<BlockType, Dictionary<SpecialType, Sprite>>();

    public Sprite GetSprite(BlockType pBlockType,SpecialType pSpecialType)
    {
        Sprite sprite = null;
        if (blockSprite.ContainsKey(pBlockType))
        {
            if (blockSprite[pBlockType].ContainsKey(pSpecialType))
            {
                //��������Ʈ�� ã���ش�.
                sprite = blockSprite[pBlockType][pSpecialType];
            }
        }
        return sprite;
    }

    public static bool IsColorBlock(BlockType pBlockType)
    {
        switch (pBlockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
                return true;
        }
        return false;
    }

    public static bool IsHasDic(SpecialType pSpecialType)
    {
        switch (pSpecialType)
        {
            case SpecialType.normal:
                return false;
            case SpecialType.rocket:
                return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��� ü��
    ////////////////////////////////////////////////////////////////////////////////
    public static int GetBlockHp(BlockType pBlockType)
    {
        switch (pBlockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
                return 1;
            case BlockType.spin:
                return 2;
        }
        return 0;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ��ȯ�� �������� �˻�
    ////////////////////////////////////////////////////////////////////////////////
    public static bool CanSwap(BlockType pBlockType)
    {
        switch (pBlockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
            case BlockType.spin:
                return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�Ǽ� �ı� �Ǵ� Ÿ���ΰ�?
    ////////////////////////////////////////////////////////////////////////////////
    public static bool IsMatchBlock(BlockType pBlockType)
    {
        switch (pBlockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
                return true;
            case BlockType.spin:
                return false;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���� ����� �ı��Ǹ� �ı��Ǵ°�?
    ////////////////////////////////////////////////////////////////////////////////
    public static bool IsNearMatchBlock(BlockType pBlockType)
    {
        switch (pBlockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
                return false;
            case BlockType.spin:
                return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���⿡ ���� ������Ʈ�� ���� ���� ����
    ////////////////////////////////////////////////////////////////////////////////

    public static void BlockRotaion(Transform pTrans, BlockDic pBlockDic)
    {
        switch (pBlockDic)
        {
            case BlockDic.left:
                pTrans.rotation = Quaternion.Euler(0, 0, 60);
                break;
            case BlockDic.up:
                pTrans.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case BlockDic.right:
                pTrans.rotation = Quaternion.Euler(0, 0, -60);
                break;
        }
    }
}
