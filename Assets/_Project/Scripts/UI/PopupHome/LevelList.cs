using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Pancake;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LevelList : ScrollBoard
{
    private int maxGroup = 3;
    private void Awake()
    {
        ShowContent();
    }
    protected override void SetIndexText()
    {
        getObj.GetComponent<CinemaUILevel>().SetLevelText(index);
    }
    private void OnEnable()
    {
        for (int i = 1; i <= elements; i++)
        {
            if (i == Data.CurrentLevel)
            {
                content.transform.GetChild(i - 1).GetComponent<CinemaUILevel>().SetHightLight(true);
            }
            else
            {
                content.transform.GetChild(i - 1).GetComponent<CinemaUILevel>().SetHightLight(false);
            }
        }
        SetPosi();
    }
    void SetPosi()
    {
        var setSizeGroup = (content.cellSize.y + content.spacing.y) * 3;
        var calculateLevel = Data.CurrentLevel % maxGroup == 0 ? Data.CurrentLevel / maxGroup - 1 : Data.CurrentLevel / maxGroup;
        var getRect = content.GetComponent<RectTransform>();
        content.rectTransform().anchoredPosition3D = new Vector3(0, Mathf.Ceil(getRect.rect.height / 2) - setSizeGroup * calculateLevel, 0);
    }
}
