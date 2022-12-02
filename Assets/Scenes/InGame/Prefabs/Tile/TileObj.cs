using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : Ÿ���� ǥ���մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class TileObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileRenderer;
    [SerializeField] private Animation tileAnimation;
    [SerializeField] private GameObject tileEffect;

    ////////////////////////////////////////////////////////////////////////////////
    /// : Ÿ�� �ʱ�ȭ
    ////////////////////////////////////////////////////////////////////////////////
    public void InitTile(int pSortOrder)
    {
        if (tileRenderer != null)
        {
            //Ÿ�� Ȱ��ȭ
            tileRenderer.gameObject.SetActive(true);
        }

        ActTileEffect(false);

        tileAnimation.Play();

        tileRenderer.sortingOrder = pSortOrder;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : Ÿ�� ��Ȱ��ȭ
    ////////////////////////////////////////////////////////////////////////////////

    public void DisableTile()
    {
        if (tileRenderer != null)
        {
            //Ÿ�� ��Ȱ��ȭ
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
