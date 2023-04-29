using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class LevelList : MonoBehaviour
{
    [SerializeField] private int levels;
    [SerializeField] private GridLayoutGroup content;
    [SerializeField] private CinemaUILevel cinemaUI;
    private void Awake()
    {
        for (int i = 0; i < levels; i++)
        {
            Instantiate(cinemaUI.gameObject, content.transform);
        }
    }
}
