using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 블록들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class BlockGroup : FieldObjectSingleton<BlockGroup>
{
    [SerializeField] private BlockObj blockPrefab;
    private Queue<BlockObj> blockQueue = new Queue<BlockObj>();
    private BlockObj[,] blockArray;

    //블록 생성포인트
    private List<Vector2Int> spawnPoints = new List<Vector2Int>();

    //생성될 블록의 위치값
    private HashSet<Vector2Int> createPoints = new HashSet<Vector2Int>();

    //이동할 블록의 위치값
    private HashSet<Vector2Int> movePoints = new HashSet<Vector2Int>();

    //매치되서 폭발하는 이벤트들
    private Queue<List<MatchEvent>> matchEventQueue = new Queue<List<MatchEvent>>();


    //블록이 낙하방향
    #region[private int[][,] dropDic]
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };
    #endregion

    //일자로 블록이 쌓였는지 검사할 방향이다.
    #region[private int[][][,] matchDic]
    private int[][][,] matchDic = new int[][][,]
    {
                //위 아래
                new int [][,]
                {
                    new int[,]{{ 0, 2 }, { 0, -2 } },
                    new int[,]{{ 0, 2 }, { 0, -2 } },
                },

                //왼위, 오른아래
                new int [][,]
                {
                    new int[,]{{ 0, 1 }, { 1,-1 } },
                    new int[,]{{ -1, 1 }, { 0, -1 } },
                },

                //오른위, 왼아래
                new int [][,]
                {
                    new int[,]{{ 1, 1 }, { 0,-1 } },
                    new int[,]{{ 0, 1 }, { -1, -1 } },
                },
    };
    #endregion

    //4개의 블록이 모였을때의 블록위치
    #region[private int[][][,] gatherPos]
    //짝수위치, 검사 모양,
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
    /// : 블록맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<SaveBlockData> pEBlockDatas)
    {
        //보드 데이터를 받아온다.
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
                //empty이랑 none은 빈공간을 의미한다.
                //블록을 생성하지 않는다.
                blockObj.DisableBlock();
                continue;
            }

            if (blockType == BlockType.spawn)
            {
                //spawn은 블록이 새로 생성되는 위치이다.
                //블록을 생성하지 않는다.
                blockObj.DisableBlock();

                //블록생성구역으로 등록한다.
                spawnPoints.Add(new Vector2Int(blockX, blockY));
                continue;
            }

            //블록 초기화
            blockObj.InitBlock(blockType, specialType, blockDic);

            //해당 위치에 블록을 등록
            blockArray[blockX, blockY] = blockObj;
        }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당위치의 블록을 불러온다.
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
    /// : 보드의 블록을 교체한다.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator SwapBlockEvent(Vector2Int pPos0, Vector2Int pPos1, bool pMatch = true)
    {
        BlockObj block0 = GetBlock(pPos0);
        BlockObj block1 = GetBlock(pPos1);
        if (block0 == null || block1 == null)
        {
            yield break;
        }

        //보드 데이터를 받아온다.
        float moveTime = 0;
        float matchDelay = 0;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            matchDelay = boardManager.matchDelay;
            moveTime = boardManager.moveTime;
        }

        //블록을 코드상으로 이동시킨다.(배열값 변경)
        BlockObj temp = block0;
        blockArray[pPos0.x, pPos0.y] = block1;
        blockArray[pPos1.x, pPos1.y] = temp;

        //블록이 이동할 값을 구해놓은다.
        Vector3 vectorPos0 = block0.transform.position;
        Vector3 vectorPos1 = block1.transform.position;

        //이동 애니메이션 실행
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos0.x, pPos0.y].transform, vectorPos0, moveTime));
        StartCoroutine(MyLib.Action2D.MoveTo(
            blockArray[pPos1.x, pPos1.y].transform, vectorPos1, moveTime));
        yield return new WaitForSeconds(moveTime);

        if(pMatch == true)
        {
            //바꾼 블록 위치에서 둘중하나여도
            //매치가 발생했는 검사
            bool matchBlock = MatchCheck(pPos0.x, pPos0.y);
            matchBlock |= MatchCheck(pPos1.x, pPos1.y);
            if (matchBlock == false)
            {
                StartCoroutine(SwapBlockEvent(pPos0, pPos1, false));
            }
            else
            {
                //이동횟수 갱신
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
    /// : 보드의 블록을 낙하시킨다.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator BlockMoveEvent()
    {
        CreateNewBlockList(); //새로 생성해야할 블록이 있는지 확인
        CreateNewBlock();

        //타일 데이터를 받아온다.
        TileGroup tileGroup = TileGroup.instance;
        if(tileGroup == null)
        {
            yield break;
        }

        //보드 데이터를 받아온다.
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

        //내려갈 블록이 있을수도있다 한번더 확인한다.
        CheckDownBlock();

        for (int idx = 0; idx < 3; idx++)
        {
            foreach(Vector2Int movePos in movePoints)
            {
                BlockObj block = blockArray[movePos.x, movePos.y];
                if (block == null)
                    continue;

                int by = movePos.y % 2; //y좌표에 따라서 블록의 위치가 다르다.
                int nextX = movePos.x + dropDic[by][idx, 0];
                int nextY = movePos.y + dropDic[by][idx, 1];
                if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                {
                    BlockObj nBlock = blockArray[nextX, nextY];
                    bool isTile = tileGroup.IsTile(nextX, nextY);

                    if (nBlock == null && isTile)
                    {
                        //타일인 부분이고
                        //떨어질 부분에 블록이 없다.
                        moveBlocks.Add(new Vector2Int(nextX, nextY));
                        blockArray[nextX, nextY] = blockArray[movePos.x, movePos.y];
                        blockArray[movePos.x, movePos.y] = null;
                        continue;
                    }

                    bool thisTile = tileGroup.IsTile(movePos.x, movePos.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //본인이 타일밖이라면
                        //타일을 만날수있도록 도와준다.
                        moveBlocks.Add(new Vector2Int(nextX, nextY));
                        blockArray[nextX, nextY] = blockArray[movePos.x, movePos.y];
                        blockArray[movePos.x, movePos.y] = null;
                        continue;
                    }
                }
            }
        }

        //이동 애니메이션 실행
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
            //블록이 이동했기 때문에
            //해당 시간만큼 대기한다.
            yield return new WaitForSeconds(moveTime);
        }

        //내려간 블록이 더 이상 내려갈 곳이 없으면
        //해당 블록에 의해서 매치가 성사되는지 파악할 필요가 있다.
        foreach (Vector2Int mblock in moveBlocks)
        {
            BlockObj block = blockArray[mblock.x, mblock.y];
            if (block == null)
                continue;

            bool downBlock = false;
            for (int idx = 0; idx < 3; idx++)
            {
                int by = mblock.y % 2; //y좌표에 따라서 블록의 위치가 다르다.
                int nextX = mblock.x + dropDic[by][idx, 0];
                int nextY = mblock.y + dropDic[by][idx, 1];
                if (MyLib.Exception.IndexOutRange(nextX, nextY, blockArray))
                {
                    BlockObj nBlock = blockArray[nextX, nextY];
                    bool isTile = tileGroup.IsTile(nextX, nextY);

                    if (nBlock == null && isTile)
                    {
                        //타일인 부분이고 떨어질 부분에 블록이 없다.
                        //아직 떨어질수있다.
                        downBlock = true;
                        break;
                    }

                    bool thisTile = tileGroup.IsTile(mblock.x, mblock.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //본인이 타일밖에 있는 블록이 아직떨어질수있다.
                        downBlock = true;
                        break;
                    }
                }
            }

            if (downBlock == false)
            {
                //떨어질 블록이 아니다.
                //매치가 성사되는지 파악하자.
                MatchCheck(mblock.x, mblock.y);

                //블록이 흔들리는 애니메이션
                block.ShakeAni();
            }
        }

        bool checkDownBlock = CheckDownBlock(); //떨어져야할 블록이 있는지 확인
        if (checkDownBlock == true)
        {
            //아직 떨어져야할 블록이 존재한다.
            StartCoroutine(BlockMoveEvent());
        }
        else
        {
            bool makeBlock = CreateNewBlockList(); //새로 생성해야할 블록이 있는지 확인
            if (makeBlock)
            {
                StartCoroutine(BlockMoveEvent());
            }
            else
            {
                //매치되서 파괴되는 블록을 확인한다.
                bool matchBlock = MatchCheck();

                if (matchBlock)
                {
                    //블록이 파괴됬다 대기한다.
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
    /// : 내려갈 공간이 존재하는지 검사한다.
    ////////////////////////////////////////////////////////////////////////////////
    private bool CheckDownBlock()
    {
        //타일 데이터를 받아온다.
        TileGroup tileGroup = TileGroup.instance;
        if (tileGroup == null)
        {
            return false;
        }

        //보드 데이터를 받아온다.
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

                int by = y % 2; //y좌표에 따라서 블록의 위치가 다르다.
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
                            //타일인 부분이고
                            //떨어질 부분에 블록이 없다.
                            movePoints.Add(new Vector2Int(x, y));
                            continue;
                        }

                        bool thisTile = tileGroup.IsTile(x, y);
                        if (nBlock == null && thisTile == false)
                        {
                            //본인이 타일밖이라면
                            //타일을 만날수있도록 도와준다.
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
    /// : 어떠한 매치 이벤트가 발생하는지를 나타낸다.
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
    /// : 매치가 일어나지 판정한다.
    ////////////////////////////////////////////////////////////////////////////////
    private bool MatchCheck()
    {
        if (matchEventQueue.Count > 0)
        {
            //매치 이벤트큐에 매치가 존재한다.
            //매치할것이 존재한다는뜻
            return true;
        }
        return false;
    }

    private bool MatchCheck(int pX, int pY)
    {
        List<MatchEvent> matchEvents = MatchBlock(pX, pY);

        if(matchEvents != null)
        {
            //매치가 발생했다.
            //매치를 등록해준다.
            matchEventQueue.Enqueue(matchEvents);
            return true;
        }

        //매치가 발생하지 않았다.
        return false;
    }

    private List<MatchEvent> MatchBlock(int pX, int pY)
    {
        List<MatchEvent> matchEvents = null;

        //보드 데이터를 받아온다.
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
            //매치되어도 깨지지않는 블록이다.
            return null;
        }

        if (blockObj.CanSwap() == false)
        {
            //교체가 안되는 블록이다.
            //따라서 매치되서 깨질일도 없다.
            return null;
        }

        BlockType bType = blockObj.blockType;
        Vector2Int firePos = new Vector2Int(pX, pY);

        #region[일자로 블록이 3개 이상 모였는지 검사한다.]     
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

                    //같은 타입의 블록이다. 
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

        #region[4개의 블록이 모임을 검사]
        for (int gtype = 0; gtype < 12; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 0; idx < 3; idx++)
            {
                int by = pY % 2; //y좌표에 따라서 블록의 위치가 다르다.

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
    /// : 매치이벤트들을 처리한다.
    ////////////////////////////////////////////////////////////////////////////////
    public IEnumerator MatchBlockEvent()
    {
        HashSet<Vector2Int> nearBlocks = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, MatchType> matchMap = new Dictionary<Vector2Int, MatchType>();

        List<MatchEvent> tempEvents = new List<MatchEvent>();
        while (matchEventQueue.Count > 0)
        {
            //매치 이벤트를 모아서 정렬
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
            //매치가 더 다른 매치에 겹치면 처리할필요가없고 처리해서도 안된다.
            //겹치는 매치들을 제거해준다.
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
                                    //매치에 해당하는 블록이 상위 매치에 겹쳤다.
                                    matchCoverCnt--;
                                }
                            }
                        }

                        if (matchCoverCnt > 0)
                        {
                            //매치가 상위 매치에게 완전히 덮혀버린 경우가
                            //아닌걸로 판단되었다. 해당 매치도 등록해준다.
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
            //특수 블록을 생성할 부분을 기록해둔다.
            Vector2Int firePos = matchEvent.firePos;
            BlockObj fireBlock = GetBlock(firePos.x, firePos.y);
            if (fireBlock == null)
            {
                //해당위치에 블록이없다.
                continue;
            }

            if (makeBlockDatas.ContainsKey(firePos))
            {
                //해당 위치에는 이미 블록이 생성되었다.
                continue;
            }

            BlockType blockType = fireBlock.blockType;
            MatchType matchType = matchEvent.matchType;
            switch(matchType)
            {
                //매치 종류에 따라 생성되는 특수블록이 다르다.
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


        //주변에서 매치될때 터지는 블록처리
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
    /// : 새로운 블록을 생성한다.
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
            //블록큐에 블록이 없다.
            //블록을 생성해주자.
            BlockObj newBlock = Instantiate(blockPrefab);
            blockQueue.Enqueue(newBlock);
        }

        //블록 넣어준다.
        BlockObj block = blockQueue.Dequeue();
        return block;
    }
    private BlockObj MakeBlockObj(Vector2Int pos)
    {
        //블록 넣어준다.
        BlockObj block = MakeBlockObj();

        if(block == null)
        {
            return null;
        }

        //보드 데이터를 받아온다.
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
            //블록 넣어준다.
            BlockObj block = MakeBlockObj(pos);
            block.InitBlock(false);

            blockArray[pos.x, pos.y] = block;
        }

        createPoints.Clear();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 새로운 생성할 블록의 위치를 구한다.
    ////////////////////////////////////////////////////////////////////////////////
    private bool CreateNewBlockList()
    {
        createPoints.Clear();

        HashSet<Vector2Int> createPoint = new HashSet<Vector2Int>();

        foreach (Vector2Int spawnPos in spawnPoints)
        {
            //스폰위치의 한칸아래에 블록이 비게되면
            //블록을 생성하는 방식이다.
            //한칸아래를 확인해본다.
            Vector2Int checkPos = spawnPos + new Vector2Int(0, -2);

            BlockObj block = GetBlock(checkPos.x, checkPos.y);
            if (block != null)
            {
                //아직 아래에 블록이 존재한다.
                continue;
            }
            createPoint.Add(spawnPos);
        }

        if (createPoint.Count > 0)
        {
            //아직 블록을 생성할곳이 존재한다.
            foreach (Vector2Int pos in createPoint)
            {
                createPoints.Add(pos);
            }
            return true;
        }
        else
        {
            //블록을 생성할게 없다.
            return false;
        }       
    }
}
