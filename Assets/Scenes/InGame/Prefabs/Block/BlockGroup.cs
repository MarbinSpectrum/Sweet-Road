using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ��ϵ��� �����մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class BlockGroup : FieldObjectSingleton<BlockGroup>
{
    [SerializeField] private BlockObj blockPrefab;
    private Queue<BlockObj> blockQueue = new Queue<BlockObj>();
    private BlockObj[,] blockArray;

    //��� ��������Ʈ
    private List<Vector2Int> spawnPoints = new List<Vector2Int>();

    //������ ����� ��ġ��
    private HashSet<Vector2Int> createPoints = new HashSet<Vector2Int>();

    //�̵��� ����� ��ġ��
    private HashSet<Vector2Int> movePoints = new HashSet<Vector2Int>();

    //��ġ�Ǽ� �����ϴ� �̺�Ʈ��
    private Queue<List<MatchEvent>> matchEventQueue = new Queue<List<MatchEvent>>();


    //����� ���Ϲ���
    #region[private int[][,] dropDic]
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };
    #endregion

    //���ڷ� ����� �׿����� �˻��� �����̴�.
    #region[private int[][][,] matchDic]
    private int[][][,] matchDic = new int[][][,]
    {
                //�� �Ʒ�
                new int [][,]
                {
                    new int[,]{{ 0, 2 }, { 0, -2 } },
                    new int[,]{{ 0, 2 }, { 0, -2 } },
                },

                //����, �����Ʒ�
                new int [][,]
                {
                    new int[,]{{ 0, 1 }, { 1,-1 } },
                    new int[,]{{ -1, 1 }, { 0, -1 } },
                },

                //������, �޾Ʒ�
                new int [][,]
                {
                    new int[,]{{ 1, 1 }, { 0,-1 } },
                    new int[,]{{ 0, 1 }, { -1, -1 } },
                },
    };
    #endregion

    //4���� ����� �������� �����ġ
    #region[private int[][][,] gatherPos]
    //¦����ġ, �˻� ���,
    private int[][][,] gatherPos = new int[][][,]
    {
        new int[][,]
        {
            new int[,]{{ 0, 2 }, { +1, -1 }, { 1, 3 }},
            new int[,]{{ 0, -1 }, { +0, +2 }, { 0, 1 }},
            new int[,]{{ +0, -2 }, { +1, -1 }, { 1, 1 }},
            new int[,]{{ 0, -1 }, { 0, -2 }, { 0, -3 }},

            new int[,]{{ 0, 2 }, { 0, 3 }, { 0, 1 }},
            new int[,]{{ 1, -1 }, { 1, 1 }, { 0, 2 }},
            new int[,]{{ 0, -2 }, { 0, -1 }, { 0, 1 }},
            new int[,]{{ 0, -2 }, { 1, -1 }, { 1, -3 }},

            new int[,]{{ 1, 1 }, { 1, -1 }, { 1, 0 }},
            new int[,]{{ 0, -1 }, { 1, -1 }, { 0,-2 }},
            new int[,]{{ 0, 1 }, { 1, +1 }, { 0,+2 }},
            new int[,]{{ 0, -1 }, { 0, +1 }, { -1,0 }},
        },
        new int[][,]
        {
            new int[,]{ { 0, 1 }, { 0, 2 }, { +0, 3 }},
            new int[,]{ { 0, -2 }, { 0, -1 }, { +0, 1 }},
            new int[,]{ { 0, +2 }, { -1, -1 }, { -1, 1 }},
            new int[,]{ { 0, -2 }, { -1, -1 }, { -1, -3 }},

            new int[,]{ { 0, 2 }, { -1, +1 }, { -1, 3 }},
            new int[,]{ { 0, 2 }, { 0, -1 }, { 0, 1 }},
            new int[,]{ { 0, -2 }, { -1, -1 }, { -1, 1 }},
            new int[,]{ { 0, -2 }, { 0, -1 }, { 0, -3 }},

            new int[,]{ { 0, 1 }, { 0, -1 }, { 1, 0 }},
            new int[,]{ { 0, -2 }, { -1, -1 }, { 0, -1 }},
            new int[,]{ { 0, 2 }, { -1, +1 }, { 0, 1 }},
            new int[,]{ { -1, 1 }, { -1, -1 }, { -1, 0 }},
        }
    };
    #endregion

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ϸ� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<SaveBlockData> pEBlockDatas)
    {
        //���� �����͸� �޾ƿ´�.
        float blockWidth = 0;
        float blockHeight = 0;
        int mapWidth = 0;
        int mapHeight = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
            centerPos = boardManager.centerPos;
        }

        blockArray = new BlockObj[mapWidth + 1, mapHeight + 1];

        for (int idx = 0; idx < pEBlockDatas.Count; idx++)
        {
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;

            BlockObj blockObj = MakeBlockObj(new Vector2Int(blockX, blockY));

            BlockType blockType = pEBlockDatas[idx].blockType;
            SpecialType specialType = pEBlockDatas[idx].specialType;
            BlockDic blockDic = pEBlockDatas[idx].blockDic;

            if(blockType == BlockType.none || 
                blockType == BlockType.empty)
            {
                //empty�̶� none�� ������� �ǹ��Ѵ�.
                //����� �������� �ʴ´�.
                blockObj.DisableBlock();
                continue;
            }

            if (blockType == BlockType.spawn)
            {
                //spawn�� ����� ���� �����Ǵ� ��ġ�̴�.
                //����� �������� �ʴ´�.
                blockObj.DisableBlock();

                //��ϻ����������� ����Ѵ�.
                spawnPoints.Add(new Vector2Int(blockX, blockY));
                continue;
            }

            //��� �ʱ�ȭ
            blockObj.InitBlock(blockType, specialType, blockDic);

            //�ش� ��ġ�� ����� ���
            blockArray[blockX, blockY] = blockObj;
        }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش���ġ�� ����� �ҷ��´�.
    ////////////////////////////////////////////////////////////////////////////////
    public BlockObj GetBlock(Vector2Int pPos)
    {
        return GetBlock(pPos.x,pPos.y);
    }
    public BlockObj GetBlock(int pX, int pY)
    {
        if (MyLib.Exception.IndexOutRange(pX, pY, blockArray) == false)
        {
            return null;
        }
        return blockArray[pX, pY];
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ����� ��ü�Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator SwapBlockEvent(Vector2Int pPos0, Vector2Int pPos1, bool pMatch = true)
    {
        BlockObj block0 = GetBlock(pPos0);
        BlockObj block1 = GetBlock(pPos1);
        if (block0 == null || block1 == null)
        {
            yield break;
        }

        //���� �����͸� �޾ƿ´�.
        float moveTime = 0;
        float matchDelay = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            matchDelay = boardManager.matchDelay;
            moveTime = boardManager.moveTime;
        }

        //����� �ڵ������ �̵���Ų��.(�迭�� ����)
        BlockObj temp = block0;
        blockArray[pPos0.x, pPos0.y] = block1;
        blockArray[pPos1.x, pPos1.y] = temp;

        //����� �̵��� ���� ���س�����.
        Vector3 vectorPos0 = block0.transform.position;
        Vector3 vectorPos1 = block1.transform.position;

        //�̵� �ִϸ��̼� ����
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos0.x, pPos0.y].transform, vectorPos0, moveTime));
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos1.x, pPos1.y].transform, vectorPos1, moveTime));
        yield return new WaitForSeconds(moveTime);

        if(pMatch == true)
        {
            //�ٲ� ��� ��ġ���� �����ϳ�����
            //��ġ�� �߻��ߴ� �˻�
            bool matchBlock = MatchCheck(pPos0.x, pPos0.y);
            matchBlock |= MatchCheck(pPos1.x, pPos1.y);
            if (matchBlock == false)
            {
                StartCoroutine(SwapBlockEvent(pPos0, pPos1, false));
            }
            else
            {
                //�̵�Ƚ�� ����
                InGameUI inGameUI = InGameUI.instance;
                inGameUI.UseMoveCnt();

                yield return MatchBlockEvent();
                yield return new WaitForSeconds(matchDelay);

                StartCoroutine(BlockMoveEvent());
            }
        }
        else
        {
            boardManager.boardLock = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ����� ���Ͻ�Ų��.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator BlockMoveEvent()
    {
        CreateNewBlockList(); //���� �����ؾ��� ����� �ִ��� Ȯ��
        CreateNewBlock();

        //Ÿ�� �����͸� �޾ƿ´�.
        TileGroup tileGroup = TileGroup.instance;
        if(tileGroup == null)
        {
            yield break;
        }

        //���� �����͸� �޾ƿ´�.
        float blockWidth = 0;
        float blockHeight = 0;
        float moveTime = 0;
        float matchDelay = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            moveTime = boardManager.moveTime;
            matchDelay = boardManager.matchDelay;
            centerPos = boardManager.centerPos;
        }
        else
        {
            yield break;
        }

        List<Vector2Int> moveBlocks = new List<Vector2Int>();

        //������ ����� ���������ִ� �ѹ��� Ȯ���Ѵ�.
        CheckDownBlock();

        for (int idx = 0; idx < 3; idx++)
        {
            foreach(Vector2Int movePos in movePoints)
            {
                BlockObj block = blockArray[movePos.x, movePos.y];
                if (block == null)
                    continue;

                int by = movePos.y % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.
                int nextX = movePos.x + dropDic[by][idx, 0];
                int nextY = movePos.y + dropDic[by][idx, 1];
                if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                {
                    BlockObj nBlock = blockArray[nextX, nextY];
                    bool isTile = tileGroup.IsTile(nextX, nextY);

                    if (nBlock == null && isTile)
                    {
                        //Ÿ���� �κ��̰�
                        //������ �κп� ����� ����.
                        moveBlocks.Add(new Vector2Int(nextX, nextY));
                        blockArray[nextX, nextY] = blockArray[movePos.x, movePos.y];
                        blockArray[movePos.x, movePos.y] = null;
                        continue;
                    }

                    bool thisTile = tileGroup.IsTile(movePos.x, movePos.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //������ Ÿ�Ϲ��̶��
                        //Ÿ���� �������ֵ��� �����ش�.
                        moveBlocks.Add(new Vector2Int(nextX, nextY));
                        blockArray[nextX, nextY] = blockArray[movePos.x, movePos.y];
                        blockArray[movePos.x, movePos.y] = null;
                        continue;
                    }
                }
            }
        }

        //�̵� �ִϸ��̼� ����
        bool blockMoveCheck = false;
        foreach(Vector2Int mblock in moveBlocks)
        {
            BlockObj block = GetBlock(mblock);

            Vector2 toPos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, mblock.x, mblock.y);
            toPos += centerPos;

            if(block != null)
            {
                blockMoveCheck = true;
                StartCoroutine(MyLib.Action2D.MoveTo(block.transform, toPos, moveTime));
            }
        }     
        if(blockMoveCheck)
        {
            //����� �̵��߱� ������
            //�ش� �ð���ŭ ����Ѵ�.
            yield return new WaitForSeconds(moveTime);
        }

        //������ ����� �� �̻� ������ ���� ������
        //�ش� ��Ͽ� ���ؼ� ��ġ�� ����Ǵ��� �ľ��� �ʿ䰡 �ִ�.
        foreach (Vector2Int mblock in moveBlocks)
        {
            BlockObj block = blockArray[mblock.x, mblock.y];
            if (block == null)
                continue;

            bool downBlock = false;
            for (int idx = 0; idx < 3; idx++)
            {
                int by = mblock.y % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.
                int nextX = mblock.x + dropDic[by][idx, 0];
                int nextY = mblock.y + dropDic[by][idx, 1];
                if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                {
                    BlockObj nBlock = blockArray[nextX, nextY];
                    bool isTile = tileGroup.IsTile(nextX, nextY);

                    if (nBlock == null && isTile)
                    {
                        //Ÿ���� �κ��̰� ������ �κп� ����� ����.
                        //���� ���������ִ�.
                        downBlock = true;
                        break;
                    }

                    bool thisTile = tileGroup.IsTile(mblock.x, mblock.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //������ Ÿ�Ϲۿ� �ִ� ����� �������������ִ�.
                        downBlock = true;
                        break;
                    }
                }
            }

            if (downBlock == false)
            {
                //������ ����� �ƴϴ�.
                //��ġ�� ����Ǵ��� �ľ�����.
                MatchCheck(mblock.x, mblock.y);

                //����� ��鸮�� �ִϸ��̼�
                block.ShakeAni();
            }
        }

        bool checkDownBlock = CheckDownBlock(); //���������� ����� �ִ��� Ȯ��
        if (checkDownBlock == true)
        {
            //���� ���������� ����� �����Ѵ�.
            StartCoroutine(BlockMoveEvent());
        }
        else
        {
            bool makeBlock = CreateNewBlockList(); //���� �����ؾ��� ����� �ִ��� Ȯ��
            if (makeBlock)
            {
                StartCoroutine(BlockMoveEvent());
            }
            else
            {
                //��ġ�Ǽ� �ı��Ǵ� ����� Ȯ���Ѵ�.
                bool matchBlock = MatchCheck();

                if (matchBlock)
                {
                    //����� �ı���� ����Ѵ�.
                    yield return MatchBlockEvent();
                    yield return new WaitForSeconds(matchDelay);

                    StartCoroutine(BlockMoveEvent());
                }
                else
                {
                    boardManager.boardLock = false;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ������ ������ �����ϴ��� �˻��Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool CheckDownBlock()
    {
        //Ÿ�� �����͸� �޾ƿ´�.
        TileGroup tileGroup = TileGroup.instance;
        if (tileGroup == null)
        {
            return false;
        }

        //���� �����͸� �޾ƿ´�.
        int mapWidth = 0;
        int mapHeight = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
        }
        else
        {
            return false;
        }

        movePoints.Clear();

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                BlockObj block = blockArray[x, y];
                if (block == null)
                    continue;

                int by = y % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.
                for (int idx = 0; idx < 3; idx++)
                {
                    int nextX = x + dropDic[by][idx, 0];
                    int nextY = y + dropDic[by][idx, 1];
                    if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                    {
                        BlockObj nBlock = blockArray[nextX, nextY];
                        bool isTile = tileGroup.IsTile(nextX, nextY);

                        if (nBlock == null && isTile)
                        {
                            //Ÿ���� �κ��̰�
                            //������ �κп� ����� ����.
                            movePoints.Add(new Vector2Int(x, y));
                            continue;
                        }

                        bool thisTile = tileGroup.IsTile(x, y);
                        if (nBlock == null && thisTile == false)
                        {
                            //������ Ÿ�Ϲ��̶��
                            //Ÿ���� �������ֵ��� �����ش�.
                            movePoints.Add(new Vector2Int(x, y));
                            continue;
                        }
                    }
                }
            }
        }

        if(movePoints.Count > 0)
        {
            return true;
        }    
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��� ��ġ �̺�Ʈ�� �߻��ϴ����� ��Ÿ����.
    ////////////////////////////////////////////////////////////////////////////////
    public struct MatchEvent
    {
        public MatchType matchType;
        public Vector2Int firePos;
        public List<Vector2Int> matchPos;

        public MatchEvent(MatchType pMatchType, Vector2Int pFirePos,
            List<Vector2Int> pMatchPos)
        {
            matchType = pMatchType;
            firePos = pFirePos;
            matchPos = new List<Vector2Int>(pMatchPos);
        }

        public MatchEvent(MatchType pMatchType)
        {
            matchType = pMatchType;
            firePos = Vector2Int.one * -1;
            matchPos = new List<Vector2Int>();
        }

        public static int Compare(MatchEvent eventA, MatchEvent eventB)
        {
            if (eventA.matchType == MatchType.match5 && eventA.matchType == eventB.matchType)
            {
                int eventACnt = -eventA.matchPos.Count;
                int eventBCnt = -eventB.matchPos.Count;
                return eventACnt.CompareTo(eventBCnt);
            }
            else
                return eventA.matchType.CompareTo(eventB.matchType);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�� �Ͼ�� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool MatchCheck()
    {
        if (matchEventQueue.Count > 0)
        {
            //��ġ �̺�Ʈť�� ��ġ�� �����Ѵ�.
            //��ġ�Ұ��� �����Ѵٴ¶�
            return true;
        }
        return false;
    }

    private bool MatchCheck(int pX, int pY)
    {
        List<MatchEvent> matchEvents = MatchBlock(pX, pY);

        if(matchEvents != null)
        {
            //��ġ�� �߻��ߴ�.
            //��ġ�� ������ش�.
            matchEventQueue.Enqueue(matchEvents);
            return true;
        }

        //��ġ�� �߻����� �ʾҴ�.
        return false;
    }

    private List<MatchEvent> MatchBlock(int pX, int pY)
    {
        List<MatchEvent> matchEvents = null;

        //���� �����͸� �޾ƿ´�.
        int mapHeight = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapHeight = boardManager.mapHeight;
        }
        else
        {
            return null;
        }

        BlockObj blockObj = GetBlock(pX, pY);
        if (blockObj == null)
        {
            return null;
        }

        if (blockObj.IsMatchBlock() == false)
        {
            //��ġ�Ǿ �������ʴ� ����̴�.
            return null;
        }

        if (blockObj.CanSwap() == false)
        {
            //��ü�� �ȵǴ� ����̴�.
            //���� ��ġ�Ǽ� �����ϵ� ����.
            return null;
        }

        BlockType bType = blockObj.blockType;
        Vector2Int firePos = new Vector2Int(pX, pY);

        #region[���ڷ� ����� 3�� �̻� �𿴴��� �˻��Ѵ�.]     
        for (int idx = 0; idx < 3; idx++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int checkDic = 0; checkDic < 2; checkDic++)
            {
                int newX = pX;
                int newY = pY;

                for (int cnt = 1; cnt <= (mapHeight + 1); cnt++)
                {
                    int by = newY % 2;
                    by = Mathf.Abs(by);

                    newX += matchDic[idx][by][checkDic, 0];
                    newY += matchDic[idx][by][checkDic, 1];

                    BlockObj block = GetBlock(newX, newY);
                    if (block == null)
                        break;

                    BlockType cType = block.blockType;

                    if (bType != cType)
                        break;

                    //���� Ÿ���� ����̴�. 
                    points.Add(new Vector2Int(newX, newY));
                }
            }

            if(points.Count >= 3)
            {
                if (matchEvents == null)
                {
                    matchEvents = new List<MatchEvent>();
                }
            }

            if (points.Count == 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, firePos, points);
                matchEvents.Add(matchEvent);
            }
            else if (points.Count == 4)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match4, firePos, points);
                matchEvents.Add(matchEvent);
            }
            else if (points.Count >= 5)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match5, firePos, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[4���� ����� ������ �˻�]
        for (int gtype = 0; gtype < 12; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 0; idx < 3; idx++)
            {
                int by = pY % 2; //y��ǥ�� ���� ����� ��ġ�� �ٸ���.

                int newX = pX + gatherPos[by][gtype][idx, 0];
                int newY = pY + gatherPos[by][gtype][idx, 1];

                BlockObj cblock = GetBlock(newX, newY);
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
                if (matchEvents == null)
                {
                    matchEvents = new List<MatchEvent>();
                }

                MatchEvent matchEvent = new MatchEvent(MatchType.gather4, firePos, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        return matchEvents;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�̺�Ʈ���� ó���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator MatchBlockEvent()
    {
        HashSet<Vector2Int> nearBlocks = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, MatchType> matchMap = new Dictionary<Vector2Int, MatchType>();

        List<MatchEvent> tempEvents = new List<MatchEvent>();
        while (matchEventQueue.Count > 0)
        {
            //��ġ �̺�Ʈ�� ��Ƽ� ����
            List<MatchEvent> mEvents = matchEventQueue.Dequeue();
            foreach(MatchEvent match in mEvents)
            {
                tempEvents.Add(match);
            }
        }
        tempEvents.Sort(MatchEvent.Compare);

        List<MatchEvent> matchEvents = new List<MatchEvent>();
        foreach (MatchEvent tMatch in tempEvents)
        {
            //��ġ�� �� �ٸ� ��ġ�� ��ġ�� ó�����ʿ䰡���� ó���ؼ��� �ȵȴ�.
            //��ġ�� ��ġ���� �������ش�.
            List<Vector2Int> matchPoints = tMatch.matchPos;

            switch (tMatch.matchType)
            {
                case MatchType.match3:
                case MatchType.match4:
                case MatchType.match5:
                    {
                        int matchCoverCnt = matchPoints.Count;

                        foreach (Vector2Int pos in matchPoints)
                        {
                            if (matchMap.ContainsKey(pos))
                            {
                                if (tMatch.matchType >= matchMap[pos])
                                {
                                    //��ġ�� �ش��ϴ� ����� ���� ��ġ�� ���ƴ�.
                                    matchCoverCnt--;
                                }
                            }
                        }

                        if (matchCoverCnt > 0)
                        {
                            //��ġ�� ���� ��ġ���� ������ �������� ��찡
                            //�ƴѰɷ� �ǴܵǾ���. �ش� ��ġ�� ������ش�.
                            foreach (Vector2Int pos in matchPoints)
                                matchMap[pos] = tMatch.matchType;
                            matchEvents.Add(tMatch);
                        }
                    }
                    break;
                case MatchType.gather4:
                    matchEvents.Add(tMatch);
                    break;
            }
        }

        Dictionary<Vector2Int,MakeBlockData> makeBlockDatas = 
            new Dictionary<Vector2Int, MakeBlockData>();

        foreach (MatchEvent matchEvent in matchEvents)
        {
            //Ư�� ����� ������ �κ��� ����صд�.
            Vector2Int firePos = matchEvent.firePos;
            BlockObj fireBlock = GetBlock(firePos.x, firePos.y);
            if (fireBlock == null)
            {
                //�ش���ġ�� ����̾���.
                continue;
            }

            if (makeBlockDatas.ContainsKey(firePos))
            {
                //�ش� ��ġ���� �̹� ����� �����Ǿ���.
                continue;
            }

            BlockType blockType = fireBlock.blockType;
            MatchType matchType = matchEvent.matchType;
            switch(matchType)
            {
                //��ġ ������ ���� �����Ǵ� Ư������� �ٸ���.
                case MatchType.match4:
                case MatchType.match5:
                    makeBlockDatas[firePos] = 
                        new MakeBlockData(blockType, SpecialType.rocket, BlockDic.up);
                    break;
            }
        }

        foreach (MatchEvent matchEvent in matchEvents)
        {
            foreach (Vector2Int pos in matchEvent.matchPos)
            {
                BlockObj block = GetBlock(pos.x, pos.y);
                if (block == null)
                    continue;

                bool destroy = block.DestroyBlock();
                if (destroy == false)
                    continue;

                block.DisableBlock();

                blockQueue.Enqueue(block);
                blockArray[pos.x, pos.y] = null;

                List<Vector2Int> aroundPos =
                    MyLib.Calculator.GetAroundHexagonPos(pos.x, pos.y);

                foreach (Vector2Int aPos in aroundPos)
                {
                    nearBlocks.Add(aPos);
                }
            }
        }

        foreach (KeyValuePair<Vector2Int, MakeBlockData> makeBlockDataPair in makeBlockDatas)
        {
            Vector2Int pos = makeBlockDataPair.Key;
            MakeBlockData makeBlockData = makeBlockDataPair.Value;

            BlockType blockType = makeBlockData.blockType;
            SpecialType specialType = makeBlockData.specialType;
            BlockDic blockDic = makeBlockData.blockDic;

            BlockObj blockObj = MakeBlockObj(pos);
            blockObj.InitBlock(blockType, specialType, blockDic);
            blockArray[pos.x, pos.y] = blockObj;
        }


        //�ֺ����� ��ġ�ɶ� ������ ���ó��
        foreach (Vector2Int pos in nearBlocks)
        {
            BlockObj block = GetBlock(pos.x, pos.y);
            if (block == null)
                continue;

            bool isNearMatchBlock = block.IsNearMatchBlock();
            if (isNearMatchBlock == false)
                continue;

            bool destroy = block.DestroyBlock();
            if (destroy == false)
                continue;

            block.DisableBlock();

            blockQueue.Enqueue(block);
            blockArray[pos.x, pos.y] = null;
        }


        yield break;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���ο� ����� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    
    private struct MakeBlockData
    {
        public BlockType blockType;
        public SpecialType specialType;
        public BlockDic blockDic;
        public MakeBlockData(BlockType pBlockType, SpecialType pSpecialType,
            BlockDic pBlockDic)
        {
            blockType = pBlockType;
            specialType = pSpecialType;
            blockDic = pBlockDic;
        }
    }

    private BlockObj MakeBlockObj()
    {
        if (blockQueue.Count == 0)
        {
            //���ť�� ����� ����.
            //����� ����������.
            BlockObj newBlock = Instantiate(blockPrefab);
            blockQueue.Enqueue(newBlock);
        }

        //��� �־��ش�.
        BlockObj block = blockQueue.Dequeue();
        return block;
    }
    private BlockObj MakeBlockObj(Vector2Int pos)
    {
        //��� �־��ش�.
        BlockObj block = MakeBlockObj();

        if(block == null)
        {
            return null;
        }

        //���� �����͸� �޾ƿ´�.
        float blockWidth = 0;
        float blockHeight = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            centerPos = boardManager.centerPos;
        }
 
        Vector2 blockPos = MyLib.Calculator.CalculateHexagonPos
            (blockWidth, blockHeight, pos.x, pos.y);
        blockPos += centerPos;
        block.transform.position = blockPos;

        return block;
    }

    private void CreateNewBlock()
    {
        foreach (Vector2Int pos in createPoints)
        {
            //��� �־��ش�.
            BlockObj block = MakeBlockObj(pos);
            block.InitBlock(false);

            blockArray[pos.x, pos.y] = block;
        }

        createPoints.Clear();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���ο� ������ ����� ��ġ�� ���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool CreateNewBlockList()
    {
        createPoints.Clear();

        HashSet<Vector2Int> createPoint = new HashSet<Vector2Int>();

        foreach (Vector2Int spawnPos in spawnPoints)
        {
            //������ġ�� ��ĭ�Ʒ��� ����� ��ԵǸ�
            //����� �����ϴ� ����̴�.
            //��ĭ�Ʒ��� Ȯ���غ���.
            Vector2Int checkPos = spawnPos + new Vector2Int(0, -2);

            BlockObj block = GetBlock(checkPos.x, checkPos.y);
            if (block != null)
            {
                //���� �Ʒ��� ����� �����Ѵ�.
                continue;
            }
            createPoint.Add(spawnPos);
        }

        if (createPoint.Count > 0)
        {
            //���� ����� �����Ұ��� �����Ѵ�.
            foreach (Vector2Int pos in createPoint)
            {
                createPoints.Add(pos);
            }
            return true;
        }
        else
        {
            //����� �����Ұ� ����.
            return false;
        }       
    }
}
