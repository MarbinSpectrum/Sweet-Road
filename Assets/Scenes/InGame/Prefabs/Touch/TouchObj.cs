using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ��ġ��ü�Դϴ�. �ش� ��ü�� ��ġ�� �ǵ��մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TouchObj : MonoBehaviour
{
    private int posX;
    private int posY;
    private Vector3 swipeVector;

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ��ü �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitTouch(int pX, int pY)
    {
        posX = pX;
        posY = pY;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : OnMouseDown
    ////////////////////////////////////////////////////////////////////////////////
    public void OnMouseDown()
    {
        BoardManager boardManager = BoardManager.instance;
        if (boardManager == null)
        {
            return;
        }
        if(boardManager.boardLock)
        {
            //���� ���带 �ǵ帱�� ����.
            return;
        }

        TouchManager touchManager = TouchManager.instance;
        if(touchManager == null)
        {
            return;
        }
        swipeVector = Input.mousePosition;

        touchManager.MouseDown(posX, posY);
        Debug.Log(posX + "," + posY);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : OnMouseUp
    ////////////////////////////////////////////////////////////////////////////////
    public void OnMouseUp()
    {
        BoardManager boardManager = BoardManager.instance;
        if (boardManager == null)
        {
            return;
        }
        if (boardManager.boardLock)
        {
            //���� ���带 �ǵ帱�� ����.
            return;
        }

        TouchManager touchManager = TouchManager.instance;
        if (touchManager == null)
        {
            return;
        }
        swipeVector = Input.mousePosition - swipeVector;
        Vector2 v = swipeVector.normalized;
        float angle = Mathf.Atan2(v.y, v.x);
        if (angle < 0f)
        {
            angle = Mathf.PI * 2 + angle;
        }
        angle *= Mathf.Rad2Deg;

        Vector2Int upBlockPos = MyLib.Calculator.GetDicHeHexagonPos(posX, posY, angle);

        touchManager.MouseUp(upBlockPos.x, upBlockPos.y);
    }
}
