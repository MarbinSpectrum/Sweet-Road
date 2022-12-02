using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 터치판정들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class TouchManager : FieldObjectSingleton<TouchManager>
{
    [SerializeField] private List<TouchObj> touchs = new List<TouchObj>();
    private Vector2Int mouseDownPos;
    private Vector2Int mouseUpPos;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 터치맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitTouchMap(List<SaveBlockData> pEBlockDatas)
    {
        //보드 데이터를 받아온다.
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
                //오브젝트가 없다.
                continue;
            }
            if (pEBlockDatas.Count <= idx)
            {
                //데이터가 없다.
                break;
            }

            //none은 빈공간을 의미한다.
            //타일을 생성하지 않는다.
            BlockType blockType = pEBlockDatas[idx].blockType;
            if (blockType == BlockType.none)
            {
                touchs[idx].gameObject.SetActive(false);
                continue;
            }
            touchs[idx].gameObject.SetActive(true);

            //타일의 위치를 설정
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform tileTrans = touchs[idx].transform;
            tileTrans.position = tilePos;

            //타일초기화
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
