using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : �����ؾ��� ������Ʈ�� ǥ���մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TargetObj : SerializedMonoBehaviour
{
    private BlockType targetBlock;
    private SpecialType specialType;
    private int targetCnt;

    //UI������Ʈ
    [SerializeField] private Image targetImg;
    [SerializeField] private TextMeshProUGUI targetNum;

    //��ϸŴ���
    [SerializeField] private BlockManager blockManager;

    ////////////////////////////////////////////////////////////////////////////////
    /// : �ش� ������Ʈ �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitObj(BlockType pBlockType, SpecialType pSpecialType, int pNum)
    {
        targetBlock = pBlockType;
        specialType = pSpecialType;

        Sprite sprite = blockManager.GetSprite(targetBlock, specialType);
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
    /// : �ı��ؾ��ϴ� ����� �ı������� ó��
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
