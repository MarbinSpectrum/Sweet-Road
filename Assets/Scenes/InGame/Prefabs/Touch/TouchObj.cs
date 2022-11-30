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
        touchManager.MouseUp();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : OnMouseEnter
    ////////////////////////////////////////////////////////////////////////////////
    public void OnMouseEnter()
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
        touchManager.MouseEnter(posX, posY);
    }
}
