using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ��ġ�������� �����մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TouchManager : FieldObjectSingleton<TouchManager>
{
    [SerializeField] private List<TouchObj> touchs = new List<TouchObj>();
    private Vector2Int mouseDownPos;

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTouchMap(List<LevelEditor.SaveBlockData> pEBlockDatas, Vector2 pCenterPos,
        float pBlockWidth, float pBlockHeight, int pMapWidth, int pMapHeight)
    {
        for (int idx = 0; idx < touchs.Count; idx++)
        {
            if (touchs[idx] == null)
            {
                //������Ʈ�� ����.
                continue;
            }
            if (pEBlockDatas.Count <= idx)
            {
                //�����Ͱ� ����.
                break;
            }

            //none�� ������� �ǹ��Ѵ�.
            //Ÿ���� �������� �ʴ´�.
            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if (blockType == LevelEditor.BlockType.none)
            {
                touchs[idx].gameObject.SetActive(false);
                continue;
            }
            touchs[idx].gameObject.SetActive(true);

            //Ÿ���� ��ġ�� ����
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (pBlockWidth, pBlockHeight, blockX, blockY);
            tilePos += pCenterPos;

            Transform tileTrans = touchs[idx].transform;
            tileTrans.position = tilePos;

            //Ÿ���ʱ�ȭ
            touchs[idx].InitTouch(blockX, blockY);
        }

        return true;
    }

    public void MouseDown(int pX, int pY)
    {
        mouseDownPos.Set(pX, pY);
    }

    public void MouseUp(int pX, int pY)
    {
        
    }
}
