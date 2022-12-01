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
    public bool boardLock;

    //����� ��ĭ�� �̵��ϴµ� �ɸ��� �ð�
    #region[moveTime]
    [SerializeField] [MinValue(0)] private float MoveTime;
    public float moveTime
    {
        get { return MoveTime; }
    }
    #endregion

    //����� �������� ������
    #region[matchDelay]
    [SerializeField] [MinValue(0)] private float MatchDelay;
    public float matchDelay
    {
        get { return MatchDelay; }
    }
    #endregion

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
            //����� �������� �ʴ´�.
            boardLock = false;
            yield break;
        }

        bool canSwap = block0.CanSwap() && block1.CanSwap();
        if(canSwap == false)
        {
            //���� �ϳ��� ��ü�� �ȵǴ� ����̴�.
            boardLock = false;
            yield break;
        }

        yield return blockGroup.SwapBlockEvent(pPos0, pPos1);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : pos1�� pos0�� �ֺ� ������� Ȯ���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private bool IsAroundBlock(Vector2Int pPos0, Vector2Int pPos1)
    {
        int by = pPos0.y % 2; //y��ǥ�� ���� �ֺ� ����� ��ġ�� �ٸ���.

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
