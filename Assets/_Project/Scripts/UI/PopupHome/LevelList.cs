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
    protected override void SetIndexText()
    {
        getObj.GetComponent<CinemaUILevel>().SetLevelText(index);
    }
    private void OnEnable()
    {
        if (ConfigController.Game != null)
        {
            elements = ConfigController.Game.maxLevel;
            ShowContent();
            for (int i = 1; i <= elements; i++)
            {
                if (i % 5 == 0)
                {
                    content.transform.GetChild(i - 1).GetComponent<CinemaUILevel>().SetNormal(false);
                }
                else
                {
                    content.transform.GetChild(i - 1).GetComponent<CinemaUILevel>().SetNormal(true);
                }
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
    }
    private void OnDisable()
    {
        ClearContent();
    }
    void SetPosi()
    {
        var s = content.rectTransform().sizeDelta;
        var posiIndex = (Data.CurrentLevel - 1) * (content.cellSize.y + content.spacing.y);
        content.rectTransform().anchoredPosition3D = new Vector3(0, (s.y + content.padding.bottom) / 2 - posiIndex);
    }
}
