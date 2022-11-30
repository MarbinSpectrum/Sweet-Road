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
    private bool BoardLock;
    public bool boardLock
    {
        get
        {
            return BoardLock;
        }
    }

    //블록이 한칸을 이동하는데 걸리는 시간
    [SerializeField] [MinValue(0)] private float MoveTime;
    public float moveTime
    {
        get { return MoveTime; }
    }

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
    /// : 어떠한 매치 이벤트가 발생하는지를 나타낸다.
    ////////////////////////////////////////////////////////////////////////////////
    public struct MatchEvent
    {
        public MatchType matchType;
        public List<Vector2Int> matchPos;
        public MatchEvent(MatchType pMatchType,
            List<Vector2Int> pMatchPos)
        {
            matchType = pMatchType;
            matchPos = new List<Vector2Int>(pMatchPos);
        }

        public MatchEvent(MatchType pMatchType)
        {
            matchType = pMatchType;
            matchPos = new List<Vector2Int>();
        }
    }
    public enum MatchType
    {
        none = -1,          //아무일도 안일어남
        match3,             //3개의 블록이 일직선
        gather4,            //4개의 블록이 모임

        size
    }

    //4개의 블록이 모였을때의 블록위치
    private int[][][,] gatherPos = new int[][][,]
    {
        new int[][,]{
            new int[,]{ { 0, 0 }, { +1, 1 }, { +1, -1 }, { 1, 0 }},
            new int[,]{ { 0, 0 }, { +1, 1 }, { +0, +2 }, { 1, 3 }},
            new int[,]{ { 0, 0 }, { +0, 1 }, { +0, +2 }, { 0, 3 }},
        },
        new int[][,]{
            new int[,]{ { 0, 0 }, { 0, 1 }, { +0, -1 }, { +1, 0 }},
            new int[,]{ { 0, 0 }, { 0, 2 }, { +0, +1 }, { +0, 3 }},
            new int[,]{ { 0, 0 }, { 0, 2 }, { -1, +1 }, { -1, 3 }},
        }
    };

    //주변 블록의 블록위치
    private int[][,] aroundPos = new int[][,]
    {
        new int[,]{{0, 2}, {+1, +1}, {1, -1}, {0, -2}, { +0, -1}, {+0, +1}},
        new int[,]{{0, 2}, {+0, +1}, {0, -1}, {0, -2}, { -1, -1}, {-1, +1}},
    };

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치가 일어나지 판정한다.
    ////////////////////////////////////////////////////////////////////////////////
    private List<MatchEvent> MatchBlock(int pX,int pY)
    {
        List<MatchEvent> matchEvents = new List<MatchEvent>();

        BlockGroup blockGroup = BlockGroup.instance;
        if (blockGroup == null)
        {
            return matchEvents;
        }

        BlockObj blockObj = blockGroup.GetBlock(pX, pY);
        if (blockObj == null)
        {
            return matchEvents;
        }

        if(blockObj.IsMatchBlock() == false)
        {
            //매치되어도 깨지지않는 블록이다.
            return matchEvents;
        }

        if (blockObj.CanSwap() == false)
        {
            //교체가 안되는 블록이다.
            //따라서 매치되서 깨질일도 없다.
            return matchEvents;
        }

        BlockType bType = blockObj.blockType;

        #region[세로 방향 매치 검사]
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            int newX = pX;
            int newY = pY;

            for (int cnt = 1; cnt <= (mapHeight + 1); cnt++)
            {
                newX += 0;
                newY += 2;

                BlockObj cblock = blockGroup.GetBlock(newX, newY);
                if (cblock == null)
                {
                    break;
                }
                BlockType cType = cblock.blockType;

                if (bType == cType)
                {
                    //같은 타입의 블록이다. 
                    points.Add(new Vector2Int(newX, newY));
                }
                else
                {
                    break;
                }
            }

            if(points.Count >= 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[대각선 오른쪽 위 방향 매치 검사]
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            int newX = pX;
            int newY = pY;

            for (int cnt = 1; cnt <= (mapHeight + 1); cnt++)
            {
                if (newY % 2 == 0)
                {
                    newX += 1;
                    newY += 1;
                }
                else
                {
                    newX += 0;
                    newY += 1;
                }

                BlockObj cblock = blockGroup.GetBlock(newX, newY);
                if (cblock == null)
                {
                    break;
                }
                BlockType cType = cblock.blockType;

                if (bType == cType)
                {
                    //같은 타입의 블록이다. 
                    points.Add(new Vector2Int(newX, newY));
                }
                else
                {
                    break;
                }
            }

            if (points.Count >= 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[대각선 왼쪽 위 방향 매치 검사]
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            int newX = pX;
            int newY = pY;

            for (int cnt = 1; cnt <= (mapHeight + 1); cnt++)
            {
                if (newY % 2 == 0)
                {
                    newX += 0;
                    newY += 1;
                }
                else
                {
                    newX += -1;
                    newY += 1;
                }

                BlockObj cblock = blockGroup.GetBlock(newX, newY);
                if (cblock == null)
                {
                    break;
                }
                BlockType cType = cblock.blockType;

                if (bType == cType)
                {
                    //같은 타입의 블록이다. 
                    points.Add(new Vector2Int(newX, newY));
                }
                else
                {
                    break;
                }
            }

            if (points.Count >= 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[4개의 블록이 모임을 검사]
        for(int gtype = 0; gtype < 3; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 1; idx <= 3; idx++)
            {
                int by = pY % 2; //y좌표에 따라서 블록의 위치가 다르다.

                int newX = pX + gatherPos[pY][gtype][idx, 0];
                int newY = pY + gatherPos[pY][gtype][idx, 1];

                BlockObj cblock = blockGroup.GetBlock(newX, newY);
                if (cblock == null)
                {
                    break;
                }
                BlockType cType = cblock.blockType;

                if (bType == cType)
                {
                    //같은 타입의 블록이다. 
                    points.Add(new Vector2Int(newX, newY));
                }
                else
                {
                    break;
                }
            }

            if (points.Count >= 4)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.gather4, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        return matchEvents;
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
        BoardLock = true;

        BlockGroup blockGroup = BlockGroup.instance;
        if (blockGroup == null)
        {
            BoardLock = false;
            yield break;
        }

        BlockObj block0 = blockGroup.GetBlock(pPos0);
        BlockObj block1 = blockGroup.GetBlock(pPos1);
        if (block0 == null || block1 == null)
        {
            //블록이 존재하지 않는다.
            BoardLock = false;
            yield break;
        }

        bool canSwap = block0.CanSwap() && block1.CanSwap();
        if(canSwap == false)
        {
            //둘중 하나가 교체가 안되는 블록이다.
            BoardLock = false;
            yield break;
        }

        yield return blockGroup.SwapBlockEvent(pPos0, pPos1);

        BoardLock = false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : pos1이 pos0의 주변 블록인지 확인한다.
    ////////////////////////////////////////////////////////////////////////////////
    private bool IsAroundBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        int by = pPos0.y % 2; //y좌표에 따라서 주변 블록의 위치가 다르다.
        int len = aroundPos[by].GetLength(0);
        for (int idx = 0; idx < len; idx++)
        {
            int pX = pPos0.x + aroundPos[by][idx, 0];
            int pY = pPos0.y + aroundPos[by][idx, 1];
            if (pPos1.x == pX && pPos1.y == pY)
                return true;
        }
        return false;
    }

    public bool MatchBlockEvent()
    {


        return false;
    }
}
