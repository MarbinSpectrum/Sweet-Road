using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 터치객체입니다. 해당 객체로 터치를 판독합니다.
////////////////////////////////////////////////////////////////////////////////
public class TouchObj : MonoBehaviour
{
    private int posX;
    private int posY;
    private Vector3 swipeVector;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 터치객체 초기화
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
            //현재 보드를 건드릴수 없다.
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
            //현재 보드를 건드릴수 없다.
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
