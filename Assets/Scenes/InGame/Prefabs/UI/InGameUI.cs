using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : FieldObjectSingleton<InGameUI>
{
    [SerializeField] private Transform tObjContext;
    [SerializeField] private TargetObj tObjPrefab;
    private Dictionary<BlockType, TargetObj> targetList = new Dictionary<BlockType, TargetObj>();

    [SerializeField] private MoveCnt moveCnt;

    public void InitGameUI(int pMoveCnt, List<LevelEditor.SaveTargetData> pETargetDatas)
    {
        foreach(LevelEditor.SaveTargetData saveTargetData in pETargetDatas)
        {
            TargetObj newTarget = null;

            LevelEditor.BlockType eblockType = saveTargetData.blockType;
            string str = eblockType.ToString();
            BlockType blockType;
            if (Enum.TryParse(str, out blockType))
            {
                //ÆÄ½Ì ¼º°ø
                if (targetList.ContainsKey(blockType))
                {
                    newTarget = targetList[blockType];
                }
            }

            if (newTarget == null)
            {
                newTarget = Instantiate(tObjPrefab);
                targetList[blockType] = newTarget;
            }

            newTarget.gameObject.SetActive(true);
            newTarget.transform.parent = tObjContext;
            newTarget.transform.localScale = Vector3.one;

            int cnt = saveTargetData.targetNum;

            newTarget.InitObj(blockType, cnt);
        }

        moveCnt.InitObj(pMoveCnt);
    }

    public void DestroyBlock(BlockType pBlockType)
    {
        if(targetList.ContainsKey(pBlockType))
        {
            TargetObj targetObj = targetList[pBlockType];
            targetObj.DestroyTarget();
        }
    }

    public void UseMoveCnt()
    {
        if (moveCnt != null)
        {
            moveCnt.UseMoveCnt();
        }
    }
}
