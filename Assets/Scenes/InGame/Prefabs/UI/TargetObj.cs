using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 제거해야할 오브젝트를 표시합니다.
////////////////////////////////////////////////////////////////////////////////
public class TargetObj : SerializedMonoBehaviour
{
    private BlockType targetBlock;
    private int targetCnt;

    //UI오브젝트
    [SerializeField] private Image targetImg;
    [SerializeField] private TextMeshProUGUI targetNum;

    //블록별 이미지
    [SerializeField] private Dictionary<BlockType, Sprite> blockSprite
        = new Dictionary<BlockType, Sprite>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당 오브젝트 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public void InitObj(BlockType pBlockType,int pNum)
    {
        Sprite sprite = blockSprite[pBlockType];
        if (targetImg == null)
            return;
        targetImg.sprite = sprite;
        targetImg.gameObject.SetActive(true);

        if (targetNum == null)
            return;
        targetCnt = pNum;
        targetNum.text = targetCnt.ToString();
        targetNum.gameObject.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 파괴해야하는 블록을 파괴했을때 처리
    ////////////////////////////////////////////////////////////////////////////////
    public void DestroyTarget()
    {
        targetCnt--;
        targetNum.text = targetCnt.ToString();
        if (targetCnt <= 0)
        {
            targetImg.gameObject.SetActive(false);
            targetNum.gameObject.SetActive(false);
        }
    }
}
