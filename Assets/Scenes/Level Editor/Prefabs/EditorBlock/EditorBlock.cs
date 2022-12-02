using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 에디터에서 사용할 블록을 관리한다. 블록의 정보를 가지고 있다.
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
    /// : 블록의 스프라이트를 업데이트하는 코드
    ////////////////////////////////////////////////////////////////////////////////
    private void UpdateBlockSprite()
    {
        if (blockRenderer == null)
        {
            //랜더러 객체가 없다.
            return;
        }

        if (blockManager == null)
        {
            return;
        }

        showIfColorBlock = BlockManager.IsColorBlock(blockType);
        if (showIfColorBlock == false)
        {
            //컬러블록이 아니면 노발 블록뿐이다.
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
            //랜더러 객체가 없다.
            return;
        }

        if (blockType == BlockType.none)
        {
            //none 빈공간이므로 타일도 없애준다.
            tileRenderer.enabled = false;
        }
        else
        {
            //다른 경우는 타일이 있어야된다 표시해준다.
            tileRenderer.enabled = true;
        }
    }
}
