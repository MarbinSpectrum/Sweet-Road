using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ��ϵ��� �����մϴ�.
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

    //����� ���Ϲ���
    #region[private int[][,] dropDic]
    private int[][,] dropDic = new int[][,]
    {
        new int [,]{{ 0, -2 },{ 1, -1 },{ +0, -1 },},
        new int [,]{{ 0, -2 },{ 0, -1 },{ -1, -1 },}
    };
    #endregion

    //4���� ����� �������� �����ġ
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
            if (blocks.Count <= idx)
            {
                //�����Ͱ� ����.
                break;
            }

            BlockType blockType = pEBlockDatas[idx].blockType;
            SpecialType specialType = pEBlockDatas[idx].specialType;
            BlockDic blockDic = pEBlockDatas[idx].blockDic;

            int blockX = pEBlockDatas[idx].pos.x;
            int blockY = pEBlockDatas[idx].pos.y;

            if(blockType == BlockType.none)
            {
                //none�� ������� �ǹ��Ѵ�.
                //����� �������� �ʴ´�.
                blocks[idx].DisableBlock();
                continue;
            }

            if (blockType == BlockType.spawn)
            {
                //spawn�� ����� ���� �����Ǵ� ��ġ�̴�.
                //����� �������� �ʴ´�.
                blocks[idx].DisableBlock();

                //��ϻ����������� ����Ѵ�.
                spawnPoints.Add(new Vector2Int(blockX, blockY));
                continue;
            }

            //����� ��ġ�� ����
            Vector2 tilePos = MyLib.Calculator.CalculateHexagonPos
                (blockWidth, blockHeight, blockX, blockY);
            tilePos += centerPos;

            Transform blockTrans = blocks[idx].transform;
            blockTrans.position = tilePos;

            //��� �ʱ�ȭ
            blocks[idx].InitBlock(blockType, specialType, blockDic);

            //�ش� ��ġ�� ����� ���
            blockArray[blockX, blockY] = blocks[idx];
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
            bool matchBlock = MatchBlockEvent();

            if (matchBlock == false)
            {
                StartCoroutine(SwapBlockEvent(pPos0, pPos1, false));
            }
            else
            {
                //�̵�Ƚ�� ����
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
        //��鸮�� ������ �����ش�.
        bool isShake = false;
        foreach (Vector2Int mblock in moveBlocks)
        {
            BlockObj block = blockArray[mblock.x, mblock.y];
            if (block == null)
                continue;

            bool shakeFlag = true;
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
                        shakeFlag = false;
                        break;
                    }

                    bool thisTile = tileGroup.IsTile(mblock.x, mblock.y);
                    if (nBlock == null && thisTile == false)
                    {
                        //������ Ÿ�Ϲۿ� �ִ� ����� �������������ִ�.
                        shakeFlag = false;
                        break;
                    }
                }
            }
            isShake |= shakeFlag;

            if (shakeFlag)
            {
                //����� ��鸮�� �ִϸ��̼�
                block.ShakeAni();
            }
        }
        if(isShake)
        {
            //����� �������Ƿ� ���
           // yield return new WaitForSeconds(shakeDelay);
        }

        bool checkDownBlock = CheckDownBlock(); //���������� ����� �ִ��� Ȯ��
        if (checkDownBlock == true)
        {
            //���� ���������� ����� �����Ѵ�.
            StartCoroutine(BlockMoveEvent());
        }
        else
        {
            //��ġ�Ǽ� �ı��Ǵ� ����� Ȯ���Ѵ�.
            bool matchBlock = MatchBlockEvent();

            if(matchBlock)
            {
                //����� �ı���� ����Ѵ�.
                yield return new WaitForSeconds(matchDelay);
            }

            if (matchBlock == false)
            {
                bool makeBlock = CreateNewBlockList(); //���� �����ؾ��� ����� �ִ��� Ȯ��
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

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�� �Ͼ�� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private List<MatchEvent> MatchBlock(int pX, int pY)
    {
        List<MatchEvent> matchEvents = new List<MatchEvent>();

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
            return matchEvents;
        }

        BlockObj blockObj = GetBlock(pX, pY);
        if (blockObj == null)
        {
            return matchEvents;
        }

        if (blockObj.IsMatchBlock() == false)
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

            if (points.Count >= 3)
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

            if (points.Count >= 3)
            {
                MatchEvent matchEvent = new MatchEvent(MatchType.match3, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        #region[4���� ����� ������ �˻�]
        for (int gtype = 0; gtype < 3; gtype++)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            points.Add(new Vector2Int(pX, pY));

            for (int idx = 1; idx <= 3; idx++)
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
                MatchEvent matchEvent = new MatchEvent(MatchType.gather4, points);
                matchEvents.Add(matchEvent);
            }
        }
        #endregion

        return matchEvents;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��ġ�̺�Ʈ���� ó���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public bool MatchBlockEvent()
    {
        List<List<MatchEvent>> matchEventList = new List<List<MatchEvent>>();

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

        for (int y = 0; y <= mapHeight; y++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                List<MatchEvent> matchEvents = MatchBlock(x, y);
                if(matchEvents.Count > 0)
                {
                    //�̺�Ʈ�� �߻��ߴ�. �߰��Ѵ�.
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

            //��ġ �̺�Ʈ�� �߻��ߴ�.
            return true;
        }
        else
        {
            //�߻����� �ʾҴ�.
            return false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ���ο� ����� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private void CreateNewBlock()
    {
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
        else
        {
            return;
        }

        foreach (Vector2Int pos in createPoints)
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
