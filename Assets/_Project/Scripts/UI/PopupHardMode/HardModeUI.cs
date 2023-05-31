using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HardModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI playText;
    private int _getIndex;
    [SerializeField] List<GetReward> getRewards;
    [SerializeField] private HorizontalLayoutGroup iconContent;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image lockImage;
    [SerializeField] private Image unlockImage;
    [SerializeField] private Image unlockReward;
    [SerializeField] private Image cup;
    [SerializeField] private UIButton playBtn;
    [SerializeField] private Image iconRewards;
    public void SetIndexHardMode(int index)
    {
        _getIndex = index;
        indexText.text = index.ToString();
        SetIconReward(index);
    }
    public void PlayHardMode()
    {
        Observer.PlayHardMode?.Invoke(_getIndex);
        Data.IndexHardMode = _getIndex;
    }
    public void ShowState(EStateMode eStateMode)
    {
        switch (eStateMode)
        {
            case EStateMode.Lock:
                SetLock(true);
                break;
            case EStateMode.UnlockItem:
                SetLock(false);
                break;
            case EStateMode.Completed:
                SetComplete(true);
                break;
            case EStateMode.Lost:
                SetComplete(false);
                break;
        }
    }
    void SetLock(bool isLock)
    {
        playText.text = "Play";
        lockImage.gameObject.SetActive(isLock);
        unlockImage.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(!isLock);
        unlockReward.gameObject.SetActive(!isLock);
        statusText.gameObject.SetActive(!isLock);
        if (!isLock)
        {
            statusText.text = "Can Reward :";
        }
    }
    void SetComplete(bool isComplete)
    {
        playText.text = "RePlay";
        lockImage.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(true);
        unlockReward.gameObject.SetActive(false);
        unlockImage.gameObject.SetActive(true);
        iconContent.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        cup.gameObject.SetActive(isComplete);
        if (isComplete)
        {
            statusText.text = "Completed!";
        }
        else
        {
            statusText.text = "Failed!";
        }
    }
    void SetIconReward(int getIndex)
    {
        if (getIndex % 10 == 0)
        {
            ShowIconReward(EShowReward.TeleandSwap);
        }
        else if (getIndex % 2 == 0)
        {
            ShowIconReward(EShowReward.TeleTool);
        }
        else if (getIndex % 2 == 1)
        {
            ShowIconReward(EShowReward.SwapTool);
        }
    }
    void ShowIconReward(EShowReward eShowReward)
    {
        foreach (var getReward in getRewards)
        {
            if (getReward.eShowReward == eShowReward)
            {
                for (int i = 0; i < getReward.iconReward.Count; i++)
                {
                    if (iconContent.gameObject.activeInHierarchy)
                    {
                        var ins = Instantiate(iconRewards, iconContent.transform);
                        ins.sprite = getReward.iconReward[i];
                        iconContent.childAlignment = TextAnchor.MiddleCenter;
                    }
                }
            }
        }
    }
}
[Serializable]
public class GetReward
{
    public EShowReward eShowReward;
    public List<Sprite> iconReward;
}
public enum EShowReward
{
    TeleTool,
    SwapTool,
    TeleandSwap,
}
public enum EStateMode
{
    Lock,
    UnlockItem,
    Completed,
    Lost,
}
