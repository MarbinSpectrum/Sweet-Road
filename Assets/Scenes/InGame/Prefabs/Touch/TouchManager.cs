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
    private Vector2Int mouseUpPos;

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTouchMap(List<SaveBlockData> pEBlockDatas)
    {
        //���� �����͸� �޾ƿ´�.
        float blockWidth = 0;
        float blockHeight = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if(boardManager != null)
        {
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            centerPos = boardManager.centerPos;
        }

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
            BlockType blockType = pEBlockDatas[idx].blockType;
            if (blockType == BlockType.none)
            {
                touchs[idx].gameObject.SetActive(false);
                continue;
            }
            touchs[idx].gameObject.SetActive(true);

            //Ÿ���� ��ġ�� ����
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

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
        BoardManager boardManager = BoardManager.instance;
        if(boardManager == null)
        {
            return;
        }
        mouseUpPos.Set(pX, pY);
        boardManager.SwapBlock(mouseDownPos, mouseUpPos);
    }
}
