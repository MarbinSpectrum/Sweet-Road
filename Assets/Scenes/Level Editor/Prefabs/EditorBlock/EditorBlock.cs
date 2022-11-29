using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace LevelEditor
{
    ////////////////////////////////////////////////////////////////////////////////
    /// : 에디터에서 사용할 블록을 관리한다. 블록의 정보를 가지고 있다.
    ////////////////////////////////////////////////////////////////////////////////
    [ExecuteInEditMode]
    public class EditorBlock : SerializedMonoBehaviour
    {
        public BlockType blockType = BlockType.none;
        [SerializeField] private Dictionary<BlockType, Sprite> blockSprite 
            = new Dictionary<BlockType, Sprite>();
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

            if(blockSprite.ContainsKey(blockType) == false)
            {
                //해당 타입은 등록이 되어있지 않다.
                return;
            }

            Sprite sprite = blockSprite[blockType];
            blockRenderer.sprite = sprite;

            if (tileRenderer == null)
            {
                //랜더러 객체가 없다.
                return;
            }

            if(blockType == BlockType.none)
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

    public enum BlockType
    {
        //일반 블록
        red,
        orange,
        yellow,
        green,
        blue,
        purple,

        //방해 블록
        spin,

        //특수 블록
        rocket,

        //기타
        none,
        empty
    }
}