using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 보드의 상태를 관리한다.
////////////////////////////////////////////////////////////////////////////////
public class BoardManager : FieldObjectSingleton<BoardManager>
{
    //보드 잠금 여부
    public bool boardLock;

    //블록이 한칸을 이동하는데 걸리는 시간
    #region[moveTime]
    [SerializeField] [MinValue(0)] private float MoveTime;
    public float moveTime
    {
        get { return MoveTime; }
    }
    #endregion

    //블록이 터지기전 딜레이
    #region[matchDelay]
    [SerializeField] [MinValue(0)] private float MatchDelay;
    public float matchDelay
    {
        get { return MatchDelay; }
    }
    #endregion

    //블록의 크기 및 맵의 크기
    #region[blockWidth]
    private float BlockWidth;
    public float blockWidth
    {
        get { return BlockWidth; }
    }
    #endregion

    #region[blockWidth]
    private float BlockHeight;
    public float blockHeight
    {
        get { return BlockHeight; }
    }
    #endregion

    #region[mapWidth]
    private int MapWidth;
    public int mapWidth
    {
        get { return MapWidth; }
    }
    #endregion

    #region[mapHeight]
    private int MapHeight;
    public int mapHeight
    {
        get { return MapHeight; }
    }
    #endregion

    #region[centerPos]
    private Vector2 CenterPos;
    public Vector2 centerPos
    {
        get { return CenterPos; }
    }
    #endregion

    public void InitBoardData(float pBlockWidth, float pBlockHeight, 
        int pMapWidth, int pMapHeight, Vector2 pCenterPos)
    {
        BlockWidth = pBlockWidth;
        BlockHeight = pBlockHeight;
        MapWidth = pMapWidth;
        MapHeight = pMapHeight;
        CenterPos = pCenterPos;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록을 바꾸는 이벤트를 실행한다.
    ////////////////////////////////////////////////////////////////////////////////
    public void SwapBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        bool aroundBlock = IsAroundBlock(pPos0, pPos1);
        if (aroundBlock == false)
        {
            //주변 블록이 아니다.
            return;
        }

        StartCoroutine(SwapBlockEvent(pPos0, pPos1));
    }
    private IEnumerator SwapBlockEvent(Vector2Int pPos0, Vector2Int pPos1)
    {
        boardLock = true;

        BlockGroup blockGroup = BlockGroup.instance;
        if (blockGroup == null)
        {
            boardLock = false;
            yield break;
        }

        BlockObj block0 = blockGroup.GetBlock(pPos0);
        BlockObj block1 = blockGroup.GetBlock(pPos1);
        if (block0 == null || block1 == null)
        {
            //블록이 존재하지 않는다.
            boardLock = false;
            yield break;
        }

        bool canSwap = block0.CanSwap() && block1.CanSwap();
        if(canSwap == false)
        {
            //둘중 하나가 교체가 안되는 블록이다.
            boardLock = false;
            yield break;
        }

        yield return blockGroup.SwapBlockEvent(pPos0, pPos1);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : pos1이 pos0의 주변 블록인지 확인한다.
    ////////////////////////////////////////////////////////////////////////////////
    private bool IsAroundBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        int by = pPos0.y % 2; //y좌표에 따라서 주변 블록의 위치가 다르다.

        List<Vector2Int> aroundPos = MyLib.Calculator.GetAroundHexagonPos(pPos0.x, pPos0.y);
        for (int idx = 0; idx < aroundPos.Count; idx++)
        {
            int pX = aroundPos[idx].x;
            int pY = aroundPos[idx].y;
            if (pPos1.x == pX && pPos1.y == pY)
                return true;
        }
        return false;
    }
}
