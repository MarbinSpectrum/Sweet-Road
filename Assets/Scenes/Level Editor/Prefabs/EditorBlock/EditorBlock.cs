using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace LevelEditor
{
    ////////////////////////////////////////////////////////////////////////////////
    /// : �����Ϳ��� ����� ����� �����Ѵ�. ����� ������ ������ �ִ�.
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
        /// : ����� ��������Ʈ�� ������Ʈ�ϴ� �ڵ�
        ////////////////////////////////////////////////////////////////////////////////
        private void UpdateBlockSprite()
        {
            if (blockRenderer == null)
            {
                //������ ��ü�� ����.
                return;
            }

            if(blockSprite.ContainsKey(blockType) == false)
            {
                //�ش� Ÿ���� ����� �Ǿ����� �ʴ�.
                return;
            }

            Sprite sprite = blockSprite[blockType];
            blockRenderer.sprite = sprite;

            if (tileRenderer == null)
            {
                //������ ��ü�� ����.
                return;
            }

            if(blockType == BlockType.none)
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

    public enum BlockType
    {
        //�Ϲ� ���
        red,
        orange,
        yellow,
        green,
        blue,
        purple,

        //���� ���
        spin,

        //Ư�� ���
        rocket,

        //��Ÿ
        none,
        empty
    }
}