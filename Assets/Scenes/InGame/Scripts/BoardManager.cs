using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ������ ���¸� �����Ѵ�.
////////////////////////////////////////////////////////////////////////////////
public class BoardManager : FieldObjectSingleton<BoardManager>
{
    public enum MatchEvent
    {
        none = -1,          //�ƹ��ϵ� ���Ͼ
        match3_0 = 0,       //�밢�� ������ �� �������� 3��ġ
        match3_1 = 1,       //�밢�� ���� �� �������� 3��ġ
        match3_2 = 2,       //���� �������� 3��ġ

        gather4_0 = 3,      //�밢�� �� �������� 4���� ����� ����
        gather4_1 = 4,      //�밢�� �Ʒ� �������� 4���� ����� ����
        gather4_2 = 5,      //���� �������� 4���� ����� ����

        size
    }

    //��Ī�� �����ġ
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
    /// : ��ġ�� �Ͼ�� �����Ѵ�.
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
            //�������� ������� ��Ī�Ǵ��� Ȯ���غ���.
            if(CheckMatchEvent(pX,pY,mEvent,blockGroup))
            {
                //�ش� ������� ��Ī�̵ȴ�.
                return mEvent;
            }
        }

        return MatchEvent.none;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش� ��ġ�������� �˻�
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
                //��ϰ�ü�� ����.
                return false;
            }

            if (blockType == BlockType.spin || 
                blockType == BlockType.rocket)
            {
                //Ư������� ��Ī�̾ȵȴ�.
                return false;
            }

            if (blockType == (BlockType)(-1))
            {
                //�˻��� ��� �� ���
                blockType = block.blockType;
            }
            else if(block.blockType != blockType)
            {
                //��� ����� ���� �ʴ�.
                return false;
            }
        }

        //�ش� ������� ��Ī�ȴ�.
        return true;
    }
}
