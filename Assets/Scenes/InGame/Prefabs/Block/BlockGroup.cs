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
    private List<Vector2Int> spawnPoints = new List<Vector2Int>();
    private HashSet<Vector2Int> createPoints = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> movePoints = new HashSet<Vector2Int>();

    [SerializeField] private BlockObj blockPrefab;

    private Queue<BlockObj> blockQueue = new Queue<BlockObj>();
    private BlockObj[,] blockArray;

    //블록이 낙하방향
    #region[private int[][,] dropDic]
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };
    #endregion

    //4개의 블록이 모였을때의 블록위치
    #region[private int[][][,] gatherPos]
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
            if (blocks.Count <= idx)
            {
                //데이터가 없다.
                break;
            }

            BlockType blockType = pEBlockDatas[idx].blockType;
            SpecialType specialType = pEBlockDatas[idx].specialType;
            BlockDic blockDic = pEBlockDatas[idx].blockDic;

            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;

            if(blockType == BlockType.none)
            {
                //none은 빈공간을 의미한다.
                //블록을 생성하지 않는다.
                blocks[idx].DisableBlock();
                continue;
            }

            if (blockType == BlockType.spawn)
            {
                //spawn은 블록이 새로 생성되는 위치이다.
                //블록을 생성하지 않는다.
                blocks[idx].DisableBlock();

                //블록생성구역으로 등록한다.
                spawnPoints.Add(new Vector2Int(blockX, blockY));
                continue;
            }

            //블록의 위치를 설정
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform blockTrans = blocks[idx].transform;
            blockTrans.position = tilePos;

            //블록 초기화
            blocks[idx].InitBlock(blockType, specialType, blockDic);

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
                //이동횟수 갱신
                InGameUI inGameUI = InGameUI.instance;
                inGameUI.UseMoveCnt();

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
        //흔들리는 연출을 더해준다.
        bool isShake = false;
        foreach (Vector2Int mblock in moveBlocks)
        {
            BlockObj block = blockArray[mblock.x, mblock.y];
            if (block == null)
                continue;

            bool shakeFlag = true;
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
                        shakeFlag = false;
                        break;
                    }

                    bool thisTile = tileGroup.IsTile(mblock.x, mblock.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //본인이 타일밖에 있는 블록이 아직떨어질수있다.
                        shakeFlag = false;
                        break;
                    }
                }
            }
            isShake |= shakeFlag;

            if (shakeFlag)
            {
                //블록이 흔들리는 애니메이션
                block.ShakeAni();
            }
        }
        if(isShake)
        {
            //블록이 흔들렸으므로 대기
           // yield return new WaitForSeconds(shakeDelay);
        }

        bool checkDownBlock = CheckDownBlock(); //떨어져야할 블록이 있는지 확인
        if (checkDownBlock == true)
        {
            //아직 떨어져야할 블록이 존재한다.
            StartCoroutine(BlockMoveEvent());
        }
        else
        {
            //매치되서 파괴되는 블록을 확인한다.
            bool matchBlock = MatchBlockEvent();

            if(matchBlock)
            {
                //블록이 파괴됬다 대기한다.
                yield return new WaitForSeconds(matchDelay);
            }

            if (matchBlock == false)
            {
                bool makeBlock = CreateNewBlockList(); //새로 생성해야할 블록이 있는지 확인
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치이벤트들을 처리한다.
    ////////////////////////////////////////////////////////////////////////////////
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
            HashSet<Vector2Int> nearBlocks = new HashSet<Vector2Int>();

            foreach(List<MatchEvent> matchEvents in matchEventList)
            {
                foreach(MatchEvent matchEvent in matchEvents)
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

            //매치 이벤트가 발생했다.
            return true;
        }
        else
        {
            //발생하지 않았다.
            return false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 새로운 블록을 생성한다.
    ////////////////////////////////////////////////////////////////////////////////
    private void CreateNewBlock()
    {
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
        else
        {
            return;
        }

        foreach (Vector2Int pos in createPoints)
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
            Vector2 blockPos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, pos.x, pos.y);
            blockPos += centerPos;
            block.transform.position = blockPos;
            block.InitBlock(false);

            blockArray[pos.x, pos.y] = block;
        }

        createPoints.Clear();
    }

    private bool CreateNewBlockList()
    {
        createPoints.Clear();

        HashSet<Vector2Int> createPoint = new HashSet<Vector2Int>();

        foreach (Vector2Int spawnPos in spawnPoints)
        {
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
