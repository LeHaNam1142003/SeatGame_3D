using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Pancake;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LevelList : MonoBehaviour
{
    [SerializeField] private int levels;
    [SerializeField] private GridLayoutGroup content;
    [SerializeField] private CinemaUILevel cinemaUI;
    [SerializeField] private CinemaUILevel cinemaUILevelTop;
    private GameObject bottom;
    private int maxGroup = 3;
    private void Awake()
    {
        bottom = new GameObject();
        for (int i = 1; i <= levels + 2; i++)
        {
            if (i <= levels)
            {
                if (i == levels)
                {
                    InstainCinemaUI(cinemaUILevelTop, i);
                }
                else
                {
                    InstainCinemaUI(cinemaUI, i);
                }
            }
            else
            {
                Debug.Log(i);
                var emptyObj = Instantiate(bottom, content.transform);
                emptyObj.AddComponent<RectTransform>();
                content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
            }
        }
        SetPosi();
    }
    void InstainCinemaUI(CinemaUILevel getCinemaUILevel, int index)
    {
        var cinema = Instantiate(getCinemaUILevel, content.transform);
        content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
        cinema.SetLevelText(index);
    }
    private void OnEnable()
    {
        for (int i = 1; i <= levels; i++)
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
        // transform.DOLocalMoveY()
    }
    void SetPosi()
    {
        var setSizeGroup = (content.cellSize.y + content.spacing.y) * 3;
        var calculateLevel = Data.CurrentLevel % maxGroup == 0 ? Data.CurrentLevel / maxGroup - 1 : Data.CurrentLevel / maxGroup;
        var getRect = content.GetComponent<RectTransform>();
        Debug.Log(Mathf.Ceil(getRect.rect.height / 2));
        content.rectTransform().anchoredPosition3D = new Vector3(0, Mathf.Ceil(getRect.rect.height / 2) - setSizeGroup * calculateLevel, 0);
    }
}
