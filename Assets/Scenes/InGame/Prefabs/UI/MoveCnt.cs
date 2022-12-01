using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 이동횟수를 표시해주는 UI입니다.
////////////////////////////////////////////////////////////////////////////////

public class MoveCnt : MonoBehaviour
{
    private int moveCnt;
    [SerializeField] private TextMeshProUGUI moveNum;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 해당 오브젝트 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public void InitObj(int pMoveCnt)
    {
        moveCnt = pMoveCnt;
        moveNum.text = moveCnt.ToString();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 이동횟수 사용
    ////////////////////////////////////////////////////////////////////////////////
    public void UseMoveCnt()
    {
        moveCnt--;
        moveNum.text = moveCnt.ToString();
    }
}