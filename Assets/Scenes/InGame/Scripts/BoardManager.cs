using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : ������ ���¸� �����Ѵ�.
////////////////////////////////////////////////////////////////////////////////
public class BoardManager : FieldObjectSingleton<BoardManager>
{
    //���� ��� ����
    private bool BoardLock;
    public bool boardLock
    {
        get
        {
            return BoardLock;
        }
    }

    //����� ��ĭ�� �̵��ϴµ� �ɸ��� �ð�
    [SerializeField] [MinValue(0)] private float MoveTime;
    public float moveTime
    {
        get { return MoveTime; }
    }

    //����� ũ�� �� ���� ũ��
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
    /// : ��� ��ġ �̺�Ʈ�� �߻��ϴ����� ��Ÿ����.
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
        none = -1,          //�ƹ��ϵ� ���Ͼ
        match3,             //3���� ����� ������
        gather4,            //4���� ����� ����

        size
    }

    //4���� ����� �������� �����ġ
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

    //�ֺ� ����� �����ġ
    private int[][,] aroundPos = new int[][,]
    {
        new int[,]{{0, 2}, {+1, +1}, {1, -1}, {0, -2}, { +0, -1}, {+0, +1}},
        new int[,]{{0, 2}, {+0, +1}, {0, -1}, {0, -2}, { -1, -1}, {-1, +1}},
    };

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�� �Ͼ�� �����Ѵ�.
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
            //��ġ�Ǿ �������ʴ� ����̴�.
            return matchEvents;
        }

        if (blockObj.CanSwap() == false)
        {
            //��ü�� �ȵǴ� ����̴�.
            //���� ��ġ�Ǽ� �����ϵ� ����.
            return matchEvents;
        }

        BlockType bType = blockObj.blockType;

        #region[���� ���� ��ġ �˻�]
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
                    //���� Ÿ���� ����̴�. 
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

        #region[�밢�� ������ �� ���� ��ġ �˻�]
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
                    //���� Ÿ���� ����̴�. 
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

        #region[�밢�� ���� �� ���� ��ġ �˻�]
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
                    //���� Ÿ���� ����̴�. 
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

        #region[4���� ����� ������ �˻�]
        for(int gtype = 0; gtype < 3; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 1; idx <= 3; idx++)
            {
                int by = pY % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.

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
                    //���� Ÿ���� ����̴�. 
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
    /// : ����� �ٲٴ� �̺�Ʈ�� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public void SwapBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        bool aroundBlock = IsAroundBlock(pPos0, pPos1);
        if (aroundBlock == false)
        {
            //�ֺ� ����� �ƴϴ�.
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
            //����� �������� �ʴ´�.
            BoardLock = false;
            yield break;
        }

        bool canSwap = block0.CanSwap() && block1.CanSwap();
        if(canSwap == false)
        {
            //���� �ϳ��� ��ü�� �ȵǴ� ����̴�.
            BoardLock = false;
            yield break;
        }

        yield return blockGroup.SwapBlockEvent(pPos0, pPos1);

        BoardLock = false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : pos1�� pos0�� �ֺ� ������� Ȯ���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool IsAroundBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        int by = pPos0.y % 2; //y��ǥ�� ���� �ֺ� ����� ��ġ�� �ٸ���.
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
