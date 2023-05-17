using System;
using Unity.Collections;
using UnityEngine;

public class PopupHardMode : Popup
{
    [SerializeField] private HardModeList hardModeList;
    private void Awake()
    {
        Initialization();
    }
    void Initialization()
    {
        hardModeList.ShowContent();
    }
    private void OnEnable()
    {
        hardModeList.SetStateMode(Data.HardModeUnlock);
        Observer.PlayHardMode += OnPLayHardMode;
    }
    private void OnDisable()
    {
        Observer.PlayHardMode -= OnPLayHardMode;
    }
    void OnPLayHardMode(int index)
    {
        GameManager.Instance.StartHardModeGame(index);
    }

}
