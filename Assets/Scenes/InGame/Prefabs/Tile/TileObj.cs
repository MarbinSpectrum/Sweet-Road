using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 타일을 표시합니다.
////////////////////////////////////////////////////////////////////////////////
public class TileObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileRenderer;
    [SerializeField] private Animation tileAnimation;
    [SerializeField] private GameObject tileEffect;

    ////////////////////////////////////////////////////////////////////////////////
    /// : 타일 초기화
    ////////////////////////////////////////////////////////////////////////////////
    public void InitTile(int pSortOrder)
    {
        if (tileRenderer != null)
        {
            //타일 활성화
            tileRenderer.gameObject.SetActive(true);
        }

        ActTileEffect(false);

        tileAnimation.Play();

        tileRenderer.sortingOrder = pSortOrder;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 타일 비활성화
    ////////////////////////////////////////////////////////////////////////////////

    public void DisableTile()
    {
        if (tileRenderer != null)
        {
            //타일 비활성화
            tileRenderer.gameObject.SetActive(false);
        }
    }

    public void ActTileEffect(bool pState)
    {
        if (tileEffect != null)
        {
            tileEffect.SetActive(pState);
        }
    }
}
