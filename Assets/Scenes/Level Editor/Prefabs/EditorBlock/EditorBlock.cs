using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : �����Ϳ��� ����� ����� �����Ѵ�. ����� ������ ������ �ִ�.
////////////////////////////////////////////////////////////////////////////////
[ExecuteInEditMode]
public class EditorBlock : SerializedMonoBehaviour
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

    [SerializeField] private BlockManager blockManager;

    public SpriteRenderer blockRenderer;
    public SpriteRenderer tileRenderer;
    [HideInInspector] public int posX;
    [HideInInspector] public int posY;

    ////////////////////////////////////////////////////////////////////////////////
    /// : Update
    ////////////////////////////////////////////////////////////////////////////////
    private void Update()
    {
        UpdateBlockSprite();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ����� ��������Ʈ�� ������Ʈ�ϴ� �ڵ�
    ////////////////////////////////////////////////////////////////////////////////
    private void UpdateBlockSprite()
    {
        if (blockRenderer == null)
        {
            //������ ��ü�� ����.
            return;
        }

        if (blockManager == null)
        {
            return;
        }

        showIfColorBlock = BlockManager.IsColorBlock(blockType);
        if (showIfColorBlock == false)
        {
            //�÷������ �ƴϸ� ��� ��ϻ��̴�.
            specialType = SpecialType.normal;
        }

        Sprite sprite = blockManager.GetSprite(blockType, specialType);
        blockRenderer.sprite = sprite;

        Transform blockTrans = blockRenderer.transform;
        showIfBlockDic = BlockManager.IsHasDic(specialType);
        if (showIfBlockDic == false)
        {
            blockDic = BlockDic.up;          
        }
        BlockManager.BlockRotaion(blockTrans, blockDic);

        if (tileRenderer == null)
        {
            //������ ��ü�� ����.
            return;
        }

        if (blockType == BlockType.none)
        {
            //none ������̹Ƿ� Ÿ�ϵ� �����ش�.
            tileRenderer.enabled = false;
        }
        else
        {
            //�ٸ� ���� Ÿ���� �־�ߵȴ� ǥ�����ش�.
            tileRenderer.enabled = true;
        }
    }
}
