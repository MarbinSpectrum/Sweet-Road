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
    [SerializeField] private GameObject blockImg;
    [SerializeField] private SpriteRenderer blockRenderer;
    [SerializeField] private Animator blockAnimator;

    private int blockHp;

    [SerializeField]
    private Dictionary<BlockType, Sprite> blockSprite
        = new Dictionary<BlockType, Sprite>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ϰ�ü �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitBlock(bool pAniRun = true)
    {
        BlockType blockType =
            (BlockType)UnityEngine.Random.Range(
                (int)BlockType.red, (int)BlockType.purple + 1);
        InitBlock(blockType, pAniRun);
    }

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
            blockImg.SetActive(true);
            blockRenderer.sprite = blockSprite[blockType];
        }

        if (pAniRun == true)
        {
            blockAnimator.SetTrigger("CreateBlock");
        }
        else
        {
            blockAnimator.SetTrigger("Default");
        }
        blockHp = IsBlockHp();
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���� ����� �ı��Ǹ� �ı��Ǵ°�?
    ////////////////////////////////////////////////////////////////////////////////
    public bool IsNearMatchBlock()
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
                return false;
            case BlockType.spin:
                return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��� ü��
    ////////////////////////////////////////////////////////////////////////////////
    public int IsBlockHp()
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
                return 1;
            case BlockType.spin:
                return 2;
        }
        return 0;
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
                case BlockType.rocket:
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
            case BlockType.rocket:
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