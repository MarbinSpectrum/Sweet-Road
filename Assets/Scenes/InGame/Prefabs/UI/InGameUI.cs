using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : �ΰ��� UI�� �����ϴ� ���Դϴ�.
////////////////////////////////////////////////////////////////////////////////
public class InGameUI : FieldObjectSingleton<InGameUI>
{
    //��ǥ ����� ǥ�����ִ� UI ������Ʈ
    [SerializeField] private Transform tObjContext;
    [SerializeField] private TargetObj tObjPrefab;
    private Dictionary<BlockType, TargetObj> targetList = new Dictionary<BlockType, TargetObj>();

    //�̵����� Ƚ���� ǥ�����ִ� UI ������Ʈ
    [SerializeField] private MoveCnt moveCnt;

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ΰ��� UI�� �ʱ�ȭ�Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public void InitGameUI(int pMoveCnt, List<TargetData> pETargetDatas)
    {
        //��ǥ����� ǥ�����ش�.
        foreach(TargetData targetData in pETargetDatas)
        {
            TargetObj newTarget = null;

            BlockType blockType = targetData.blockType;
            SpecialType specialType = targetData.specialType;

            if (newTarget == null)
            {
                newTarget = Instantiate(tObjPrefab);
                targetList[blockType] = newTarget;
            }

            newTarget.gameObject.SetActive(true);
            newTarget.transform.parent = tObjContext;
            newTarget.transform.localScale = Vector3.one;

            int cnt = targetData.targetNum;

            newTarget.InitObj(blockType, specialType, cnt);
        }

        //�̵�����Ƚ���� ǥ�����ش�.
        moveCnt.InitObj(pMoveCnt);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ����� �ı��Ȱ��� ó���Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    public void DestroyBlock(BlockType pBlockType)
    {
        if(targetList.ContainsKey(pBlockType))
        {
            TargetObj targetObj = targetList[pBlockType];
            targetObj.DestroyTarget();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �̵�Ƚ�� ���
    ////////////////////////////////////////////////////////////////////////////////
    public bool UseMoveCnt()
    {
        if (moveCnt != null)
        {
            return moveCnt.UseMoveCnt();
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �̵�Ƚ���� �����ִ�.
    ////////////////////////////////////////////////////////////////////////////////
    public bool HasMoveCnt()
    {
        if (moveCnt != null)
        {
            return moveCnt.HasMoveCnt();
        }
        return false;
    }
}
