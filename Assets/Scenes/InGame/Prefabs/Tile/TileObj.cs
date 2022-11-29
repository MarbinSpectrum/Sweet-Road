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

    public void InitTile(int pSortOrder)
    {
        if (tileRenderer != null)
        {
            //Ÿ�� Ȱ��ȭ
            tileRenderer.gameObject.SetActive(true);
        }

        tileAnimation.Play();

        tileRenderer.sortingOrder = pSortOrder;
    }

    public void DisableTile()
    {
        if (tileRenderer != null)
        {
            //Ÿ�� ��Ȱ��ȭ
            tileRenderer.gameObject.SetActive(false);
        }
    }
}
