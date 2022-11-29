using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 보드의 상태를 관리한다.
////////////////////////////////////////////////////////////////////////////////
public class BoardManager : FieldObjectSingleton<BoardManager>
{
    public enum MatchEvent
    {
        none = -1,          //아무일도 안일어남
        match3_0 = 0,       //대각선 오른쪽 위 방향으로 3매치
        match3_1 = 1,       //대각선 왼쪽 위 방향으로 3매치
        match3_2 = 2,       //가로 방향으로 3매치

        gather4_0 = 3,      //대각선 위 방향으로 4개의 블록이 모임
        gather4_1 = 4,      //대각선 아래 방향으로 4개의 블록이 모임
        gather4_2 = 5,      //가로 방향으로 4개의 블록이 모임

        size
    }

    //매칭할 블록위치
    private int[][,] matchDic = new int[][,]
    {
        new int[,]{ { 0, 0 }, { 1, 1 }, { 2, 1 } }, //match3_0
        new int[,]{ { 0, 0 }, { 0, 1 }, { 0, 2 } }, //match3_1
        new int[,]{ { 0, 0 }, { 1, 0 }, { 2, 0 } }, //match3_2

        new int[,]{ { 0, 0 }, { 1, 0 }, { 1, 1 }, { 2, 1 } },   //gather4_0
        new int[,]{ { 0, 0 }, { -1, 0 }, { -1, 1 }, { 0, 1 } }, //gather4_1
        new int[,]{ { 0, 0 }, { 0, 1 }, { 1, 1 }, { 0, 2 } },   //gather4_2
    };

    ////////////////////////////////////////////////////////////////////////////////
    /// : 매치가 일어나지 판정한다.
    ////////////////////////////////////////////////////////////////////////////////
    private MatchEvent MatchBlock(int pX,int pY)
    {
        BlockGroup blockGroup = BlockGroup.instance;
        if(blockGroup == null)
        {
            return MatchEvent.none;
        }

        for(MatchEvent mEvent = MatchEvent.match3_0; 
            mEvent < MatchEvent.size; mEvent++)
        {
            //여러가지 방식으로 매칭되는지 확인해본다.
            if(CheckMatchEvent(pX,pY,mEvent,blockGroup))
            {
                //해당 방식으로 매칭이된다.
                return mEvent;
            }
        }

        return MatchEvent.none;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당 매치조건인지 검사
    ////////////////////////////////////////////////////////////////////////////////
    private bool CheckMatchEvent(int pX, int pY, MatchEvent pMEvent, 
        BlockGroup pBlockGroup)
    {
        List<BlockObj> blockObjs = new List<BlockObj>();
        int matchDicIdx = (int)pMEvent;

        for (int idx = 0; idx < matchDic[matchDicIdx].Length; idx++)
        {
            int newX = pX + matchDic[matchDicIdx][idx, 0];
            int newY = pY + matchDic[matchDicIdx][idx, 1];
            blockObjs.Add(pBlockGroup.GetBlock(newX, newY));
        }

        BlockType blockType = (BlockType)(-1);
        foreach(BlockObj block in blockObjs)
        {
            if (block == null)
            {
                //블록객체가 없다.
                return false;
            }

            if (blockType == BlockType.spin || 
                blockType == BlockType.rocket)
            {
                //특수블록은 매칭이안된다.
                return false;
            }

            if (blockType == (BlockType)(-1))
            {
                //검사할 블록 값 등록
                blockType = block.blockType;
            }
            else if(block.blockType != blockType)
            {
                //모든 블록이 같지 않다.
                return false;
            }
        }

        //해당 방식으로 매칭된다.
        return true;
    }
}
