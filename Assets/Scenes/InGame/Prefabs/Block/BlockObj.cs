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
    /// : ��ϰ�ü �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitBlock(bool pCreateAni = true)
    {
        //�����ϰ� �÷������ �ϳ� �޾ƿ´�.
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
            //��Ͽ� �ش��ϴ� ��������Ʈ�� �����.
            blockImg.SetActive(true);
            blockRenderer.sprite = blockManager.GetSprite(pBlockType, pSpecialType);
        }

        if (pCreateAni == true)
        {
            //�����ִϸ��̼� ���
            blockAnimator.SetTrigger("CreateBlock");
        }
        else
        {
            //�ִϸ��̼��� ���� ����
            blockAnimator.SetTrigger("Default");
        }

        //���Ⱚ�� ���� ��Ϲ���ó��
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
            //Ÿ�� ��Ȱ��ȭ
            blockImg.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ��ȯ�� �������� �˻�
    ////////////////////////////////////////////////////////////////////////////////
    public bool CanSwap()
    {
        return BlockManager.CanSwap(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�Ǽ� �ı� �Ǵ� Ÿ���ΰ�?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsMatchBlock()
    {
        return BlockManager.IsMatchBlock(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���� ����� �ı��Ǹ� �ı��Ǵ°�?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsNearMatchBlock()
    {
        return BlockManager.IsNearMatchBlock(blockType);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش� ����� �ı��Ǵ°�?
    ////////////////////////////////////////////////////////////////////////////////
    public bool DestroyBlock()
    {
        EffectManager effectManager = EffectManager.instance;

        DamageBlock();

        bool destroy = (blockHp <= 0);

        if(destroy)
        {
            //����� �ı����� UI�� ǥ��
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
    /// : �ش� ����� �������� �޾����� ó��
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