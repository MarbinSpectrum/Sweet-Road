using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 블록들을 관리합니다.
////////////////////////////////////////////////////////////////////////////////
public class BlockGroup : FieldObjectSingleton<BlockGroup>
{
    [SerializeField] private List<BlockObj> blocks = new List<BlockObj>();
    [SerializeField] private BlockObj blockPrefab;

    private Queue<BlockObj> blockQueue = new Queue<BlockObj>();
    private BlockObj[,] blockArray;

    //블록이 낙하방향
    #region[dropDic]
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };
    #endregion

    //4개의 블록이 모였을때의 블록위치
    #region[gatherPos]
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
    #endregion

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록맵 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public bool InitBlockMap(List<LevelEditor.SaveBlockData> pEBlockDatas)
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
            if (blocks.Count <= idx)
            {
                //데이터가 없다.
                break;
            }

            LevelEditor.BlockType blockType = pEBlockDatas[idx].blockType;
            if(blockType == LevelEditor.BlockType.none)
            {
                //none은 빈공간을 의미한다.
                //블록을 생성하지 않는다.
                blocks[idx].DisableBlock();
                continue;
            }
    
            //블록의 위치를 설정
            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform blockTrans = blocks[idx].transform;
            blockTrans.position = tilePos;

            //블록 초기화
            blocks[idx].InitBlock(blockType);

            //해당 위치에 블록을 등록
            blockArray[blockX, blockY] = blocks[idx];
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
            bool matchBlock = MatchBlockEvent();

            if (matchBlock == false)
            {
                StartCoroutine(SwapBlockEvent(pPos0, pPos1, false));
            }
            else
            {
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
        CreateNewBlock();

        //타일 데이터를 받아온다.
        TileGroup tileGroup = TileGroup.instance;
        if(tileGroup == null)
        {
            yield break;
        }

        //보드 데이터를 받아온다.
        int mapWidth = 0;
        int mapHeight = 0;
        float blockWidth = 0;
        float blockHeight = 0;
        float moveTime = 0;
        float matchDelay = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
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

        List<Vector2Int> movePos = new List<Vector2Int>();

        for (int idx = 0; idx < 3; idx++)
        {
            for (int y = 0; y <= mapHeight; y++)
            {
                for (int x = 0; x <= mapWidth; x++)
                {
                    BlockObj block = blockArray[x, y];
                    if (block == null)
                        continue;

                    int by = y % 2; //y좌표에 따라서 블록의 위치가 다르다.
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
                            movePos.Add(new Vector2Int(nextX, nextY));
                            blockArray[nextX, nextY] = blockArray[x, y];
                            blockArray[x, y] = null;
                            continue;
                        }

                        bool thisTile = tileGroup.IsTile(x, y);
                        if (nBlock == null&& thisTile == false)
                        {
                            //본인이 타일밖이라면
                            //타일을 만날수있도록 도와준다.
                            movePos.Add(new Vector2Int(nextX, nextY));
                            blockArray[nextX, nextY] = blockArray[x, y];
                            blockArray[x, y] = null;
                            continue;
                        }
                    }
                }
            }
        }

        //이동 애니메이션 실행
        for(int idx = 0; idx < movePos.Count; idx++)
        {
            BlockObj block = GetBlock(movePos[idx]);

            Vector2 toPos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, movePos[idx].x, movePos[idx].y);
            toPos += centerPos;

            if(block)
            {
                StartCoroutine(MyLib.Action2D.MoveTo(
                block.transform, toPos, moveTime));
            }
        }

        yield return new WaitForSeconds(moveTime);

        if(CheckDownBlock() == true)
        {
            //아직 떨어져야할 블록이 존재한다.
            StartCoroutine(BlockMoveEvent());
        }
        else
        {
            bool matchBlock = MatchBlockEvent();
            if(matchBlock)
            {
                yield return new WaitForSeconds(matchDelay);
            }
            if (matchBlock == false)
            {
                bool makeBlock = CreateNewBlock();
                if(makeBlock)
                {
                    StartCoroutine(BlockMoveEvent());
                }
                else
                {
                    boardManager.boardLock = false;
                }
            }
            else
            {
                StartCoroutine(BlockMoveEvent());
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
                            return true;
                        }
                    }
                }
            }
        }
        return false;
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치가 일어나지 판정한다.
    ////////////////////////////////////////////////////////////////////////////////
    private List<MatchEvent> MatchBlock(int pX, int pY)
    {
        List<MatchEvent> matchEvents = new List<MatchEvent>();

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
            return matchEvents;
        }

        BlockObj blockObj = GetBlock(pX, pY);
        if (blockObj == null)
        {
            return matchEvents;
        }

        if (blockObj.IsMatchBlock() == false)
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

            if (points.Count >= 3)
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

            if (points.Count >= 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[4개의 블록이 모임을 검사]
        for (int gtype = 0; gtype < 3; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 1; idx <= 3; idx++)
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
                MatchEvent matchEvent = new MatchEvent(MatchType.gather4, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        return matchEvents;
    }

    public bool MatchBlockEvent()
    {
        List<List<MatchEvent>> matchEventList = new List<List<MatchEvent>>();

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

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                List<MatchEvent> matchEvents = MatchBlock(x, y);
                if(matchEvents.Count > 0)
                {
                    //이벤트가 발생했다. 추가한다.
                    matchEventList.Add(matchEvents);
                }
            }
        }

        if (matchEventList.Count > 0)
        {
            foreach(List<MatchEvent> matchEvents in matchEventList)
            {
                foreach(MatchEvent matchEvent in matchEvents)
                {
                    foreach (Vector2Int pos in matchEvent.matchPos)
                    {
                        BlockObj block = GetBlock(pos.x, pos.y);
                        if(block != null)
                        {
                            block.DisableBlock();
                            blockQueue.Enqueue(block);

                            blockArray[pos.x, pos.y] = null;
                        }
                    }
                }
            }

            //매치 이벤트가 발생했다.
            return true;
        }
        else
        {
            //발생하지 않았다.
            return false;
        }
    }

    private bool CreateNewBlock()
    {
        HashSet<Vector2Int> CreatePoint = new HashSet<Vector2Int>();

        //타일 데이터를 받아온다.
        TileGroup tileGroup = TileGroup.instance;
        if (tileGroup == null)
        {
            return false;
        }

        //보드 데이터를 받아온다.
        int mapWidth = 0;
        int mapHeight = 0;
        float blockWidth = 0;
        float blockHeight = 0;
        Vector2 centerPos = Vector2.zero;
        BoardManager boardManager = BoardManager.instance;
        if (boardManager != null)
        {
            mapWidth = boardManager.mapWidth;
            mapHeight = boardManager.mapHeight;
            blockWidth = boardManager.blockWidth;
            blockHeight = boardManager.blockHeight;
            centerPos = boardManager.centerPos;
        }
        else
        {
            return false;
        }

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                //블록이 생성되어야하는 부분을 찾아준다.
                //타일 그룹에서 해당 x좌표에 생성될 블록의 위치는 정해져있다.
                //타일의 지붕에 해당하는 부분에
                //블록이 없다면 생성하게된다.
                Vector2Int createPos = tileGroup.GetTileRoot(x, y);
                if (MyLib.Exception.IndexOutRange(createPos.x, createPos.y, blockArray) == false)
                {
                    continue;
                }

                BlockObj block = GetBlock(createPos.x,createPos.y);
                if (block != null)
                    continue;
                createPos += new Vector2Int(0, 2);

                if (CreatePoint.Contains(createPos))
                    continue;
                CreatePoint.Add(createPos);
            }
        }

        foreach(Vector2Int pos in CreatePoint)
        {
            if(blockQueue.Count == 0)
            {
                //블록큐에 블록이 없다.
                //블록을 생성해주자.
                BlockObj newBlock = Instantiate(blockPrefab);
                blockQueue.Enqueue(newBlock);
            }

            //블록 넣어준다.
            BlockObj block = blockQueue.Dequeue();
            Vector2 blockPos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, pos.x, pos.y);
            blockPos += centerPos;
            block.transform.position = blockPos;
            block.InitBlock(false);

            blockArray[pos.x, pos.y] = block;
        }

        if(CreatePoint.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
