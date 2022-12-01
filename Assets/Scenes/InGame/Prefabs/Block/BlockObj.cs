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
    [SerializeField] private GameObject blockImg;
    [SerializeField] private SpriteRenderer blockRenderer;
    [SerializeField] private Animator blockAnimator;

    private int blockHp;

    [SerializeField]
    private Dictionary<BlockType, Sprite> blockSprite
        = new Dictionary<BlockType, Sprite>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록객체 초기화
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
            //파싱 성공
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
            //타일 비활성화
            blockImg.SetActive(false);
        }
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : 옆에 블록이 파괴되면 파괴되는가?
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
    /// : 블록 체력
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
                case BlockType.rocket:
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