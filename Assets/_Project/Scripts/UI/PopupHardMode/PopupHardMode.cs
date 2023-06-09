using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupHardMode : Popup
{
    [SerializeField] private HardModeList hardModeList;
    [SerializeField] private Image buttonHardModeOn;
    [SerializeField] private Image buttonHardModeOff;
    [SerializeField] private Image buttonSuperHardModeOn;
    [SerializeField] private Image buttonSuperHardModeOff;
    private bool _isSuperHardMode;
    private void OnEnable()
    {
        if (ConfigController.Game!=null)
        {
            OnClickHardMode();
            if (_isSuperHardMode)
            {
                hardModeList.SetStateMode(Data.SuperHardModeUnlock);
            }
            else
            {
                hardModeList.SetStateMode(Data.HardModeUnlock);
            }
            Observer.PlayHardMode += OnPLayHardMode;
        }
    }
    private void OnDisable()
    {
        Observer.PlayHardMode -= OnPLayHardMode;
        hardModeList.ClearContent();
    }
    void OnPLayHardMode(int index)
    {
        GameManager.Instance.StartHardModeGame(index);
    }
    void OnPlaySuperHardMode()
    {

    }
    public void OnclickSuperHardMode()
    {
        setHardModeOn(false);
        if (Data.HardModeUnlock == 25 && Data.SuperHardModeUnlock == 0)
        {
            Data.SuperHardModeUnlock = 1;
        }
        SetUp(12, Data.SuperHardModeUnlock);
    }
    public void OnClickHardMode()
    {
        setHardModeOn(true);
        SetUp(ConfigController.Game.maxLevel, Data.HardModeUnlock);
    }
    void setHardModeOn(bool isHardMode)
    {
        buttonHardModeOn.gameObject.SetActive(isHardMode);
        buttonHardModeOff.gameObject.SetActive(!isHardMode);
        buttonSuperHardModeOn.gameObject.SetActive(!isHardMode);
        buttonSuperHardModeOff.gameObject.SetActive(isHardMode);
    }
    void SetUp(int elements, int CountUnLock)
    {
        hardModeList.contentUIs.Clear();
        hardModeList.Clear();
        hardModeList.elements = elements;
        hardModeList.ShowContent();
        hardModeList.SetStateMode(CountUnLock);
    }


}
