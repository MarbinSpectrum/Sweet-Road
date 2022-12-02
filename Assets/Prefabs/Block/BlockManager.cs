using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BlockManager : FieldObjectSingleton<BlockManager>
{
    [SerializeField] private Dictionary<BlockType, Dictionary<SpecialType, Sprite>> blockSprite
        = new Dictionary<BlockType, Dictionary<SpecialType, Sprite>>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록타입에 종류에 따른 스프라이트 반환
    ////////////////////////////////////////////////////////////////////////////////
    public Sprite GetSprite(BlockType pBlockType,SpecialType pSpecialType)
    {
        Sprite sprite = null;
        if (blockSprite.ContainsKey(pBlockType))
        {
            if (blockSprite[pBlockType].ContainsKey(pSpecialType))
            {
                //스프라이트를 찾아준다.
                sprite = blockSprite[pBlockType][pSpecialType];
            }
        }
        return sprite;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 색상 블록인지 검사
    ////////////////////////////////////////////////////////////////////////////////
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : 색상 블록인지 검사
    ////////////////////////////////////////////////////////////////////////////////
    public static BlockType GetRandomColorBlock()
    {
        BlockType blockType = 
            (BlockType)Random.Range((int)BlockType.red, (int)BlockType.green + 1);
        return blockType;       
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 방향을 가진 블록인지 검사
    ////////////////////////////////////////////////////////////////////////////////
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
    /// : 블록 체력
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
    /// : 위치교환이 가능한지 검사
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
    /// : 매치되서 파괴 되는 타입인가?
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
    /// : 옆에 블록이 파괴되면 파괴되는가?
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
    /// : 방향에 따른 오브젝트가 보는 방향 설정
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
