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

    #region[public SpecialType specialType]
    private bool showIfColorBlock;
    [ShowIf("showIfColorBlock")] public SpecialType specialType;
    #endregion

    #region[public BlockDic blockDic]
    private bool showIfBlockDic;
    [ShowIf("showIfBlockDic")] public BlockDic blockDic;
    #endregion

    [SerializeField] private GameObject blockImg;
    [SerializeField] private SpriteRenderer blockRenderer;
    [SerializeField] private Animator blockAnimator;
    [SerializeField] private BlockManager blockManager;

    private int blockHp;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록객체 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public void InitBlock(bool pCreateAni = true)
    {
        //랜덤하게 컬러블록을 하나 받아온다.
        BlockType blockType = BlockManager.GetRandomColorBlock();
        InitBlock(blockType, SpecialType.normal, BlockDic.up, pCreateAni);
    }

    public void InitBlock(BlockType pBlockType, SpecialType pSpecialType,
        BlockDic pBlockDic, bool pCreateAni = true)
    {
        blockType = pBlockType;
        specialType = pSpecialType;
        blockDic = pBlockDic;

        showIfColorBlock = BlockManager.IsColorBlock(blockType);
        showIfBlockDic = BlockManager.IsHasDic(pSpecialType);

        if (blockRenderer != null)
        {
            //블록에 해당하는 스프라이트로 만든다.
            blockImg.SetActive(true);
            blockRenderer.sprite = blockManager.GetSprite(pBlockType, pSpecialType);
        }

        if (pCreateAni == true)
        {
            //생성애니메이션 재생
            blockAnimator.SetTrigger("CreateBlock");
        }
        else
        {
            //애니메이션이 없는 상태
            blockAnimator.SetTrigger("Default");
        }

        //방향값에 따른 블록방향처리
        BlockManager.BlockRotaion(transform, blockDic);

        blockHp = BlockManager.GetBlockHp(pBlockType);
    }

    public void ShakeAni() => StartCoroutine(ShakeAniEvent());
    private IEnumerator ShakeAniEvent()
    {
        Vector3 basePos = transform.position;
        Vector3 movePos = transform.position - new Vector3(0, 0.15f, 0);
        yield return MyLib.Action2D.MoveTo(transform, movePos, 0.05f);
        yield return MyLib.Action2D.MoveTo(transform, basePos, 0.03f);
    }

    public void DisableBlock()
    {
        if (blockRenderer != null)
        {
            //타일 비활성화
            blockImg.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 위치교환이 가능한지 검사
    ////////////////////////////////////////////////////////////////////////////////
    public bool CanSwap()
    {
        return BlockManager.CanSwap(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치되서 파괴 되는 타입인가?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsMatchBlock()
    {
        return BlockManager.IsMatchBlock(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 옆에 블록이 파괴되면 파괴되는가?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsNearMatchBlock()
    {
        return BlockManager.IsNearMatchBlock(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당 블록이 파괴되는가?
    ////////////////////////////////////////////////////////////////////////////////
    public bool DestroyBlock()
    {
        EffectManager effectManager = EffectManager.instance;

        DamageBlock();

        bool destroy = (blockHp <= 0);

        if(destroy)
        {
            //블록이 파괴됨을 UI에 표시
            InGameUI inGameUI = InGameUI.instance;
            inGameUI.DestroyBlock(blockType);

            switch (blockType)
            {
                case BlockType.red:
                case BlockType.orange:
                case BlockType.yellow:
                case BlockType.green:
                case BlockType.blue:
                case BlockType.purple:
                case BlockType.spin:
                    effectManager.CrushEffect(blockType, transform.position);
                    break;
            }
        }

        return destroy;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당 블록이 데미지를 받았을때 처리
    ////////////////////////////////////////////////////////////////////////////////
    private void DamageBlock()
    {
        blockHp--;
        switch (blockType)
        {
            case BlockType.red:
            case BlockType.orange:
            case BlockType.yellow:
            case BlockType.green:
            case BlockType.blue:
            case BlockType.purple:
                break;
            case BlockType.spin:
                {
                    if (blockHp == 1)
                    {
                        blockAnimator.SetTrigger("Spin");
                    }
                }
                break;
        }
    }
}