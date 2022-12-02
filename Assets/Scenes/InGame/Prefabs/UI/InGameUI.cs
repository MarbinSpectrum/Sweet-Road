using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 인게임 UI를 관리하는 곳입니다.
////////////////////////////////////////////////////////////////////////////////
public class InGameUI : FieldObjectSingleton<InGameUI>
{
    //목표 블록을 표시해주는 UI 오브젝트
    [SerializeField] private Transform tObjContext;
    [SerializeField] private TargetObj tObjPrefab;
    private Dictionary<BlockType, TargetObj> targetList = new Dictionary<BlockType, TargetObj>();

    //이동가능 횟수를 표시해주는 UI 오브젝트
    [SerializeField] private MoveCnt moveCnt;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 인게임 UI를 초기화한다.
    ////////////////////////////////////////////////////////////////////////////////
    public void InitGameUI(int pMoveCnt, List<SaveTargetData> pETargetDatas)
    {
        //목표블록을 표시해준다.
        foreach(SaveTargetData saveTargetData in pETargetDatas)
        {
            TargetObj newTarget = null;

            BlockType blockType = saveTargetData.blockType;
            SpecialType specialType = saveTargetData.specialType;

            if (newTarget == null)
            {
                newTarget = Instantiate(tObjPrefab);
                targetList[blockType] = newTarget;
            }

            newTarget.gameObject.SetActive(true);
            newTarget.transform.parent = tObjContext;
            newTarget.transform.localScale = Vector3.one;

            int cnt = saveTargetData.targetNum;

            newTarget.InitObj(blockType, specialType, cnt);
        }

        //이동가능횟수를 표시해준다.
        moveCnt.InitObj(pMoveCnt);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 블록이 파괴된것을 처리한다.
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
    /// : 이동횟수 사용
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
    /// : 이동횟수가 남아있다.
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
