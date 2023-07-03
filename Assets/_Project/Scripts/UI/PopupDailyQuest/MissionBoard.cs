using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionBoard : MonoBehaviour
{
    public TextMeshProUGUI starRewardText;
    public TextMeshProUGUI misstionContent;
    [SerializeField] private TextMeshProUGUI textProcess;
    [SerializeField] private Image processFill;
    [SerializeField] private Image claimBtnOff;
    [SerializeField] private Image claimBtnOn;
    [SerializeField] private Image tickIcon;
    [ReadOnly] public EMissionQuest eMissionQuest;
    public int process { get; set; }
    public int startReward { private get; set; }
    public int indexChild { get; set; }
    public void Init(int getStartReward, string getMisstionContent, int getProcess, EMissionQuest getEMissionQuest, int getIndexChild)
    {
        indexChild = getIndexChild;
        starRewardText.text = getStartReward.ToString();
        misstionContent.text = getMisstionContent;
        startReward = getStartReward;
        process = getProcess;
        eMissionQuest = getEMissionQuest;
        SetTypeMission();
    }
    public void SetTypeMission()
    {
        switch (eMissionQuest)
        {
            case EMissionQuest.PlayLevel:
                SetProcess(Data.PlayLevel);
                break;
            case EMissionQuest.SpinWheel:
                SetProcess(Data.SpinWheel);
                break;
            case EMissionQuest.WatchAds:
                SetProcess(Data.WatchAds);
                break;
            case EMissionQuest.CompletedHardMode:
                SetProcess(Data.CompletedHardMode);
                break;
            case EMissionQuest.Useswapbooster:
                SetProcess(Data.Useswapbooster);
                break;
        }
    }
    void SetProcess(int currentProcess)
    {
        if (currentProcess < process)
        {
            CanClaim(false);
        }
        else
        {
            CanClaim(true);
            currentProcess = process;
        }
        processFill.fillAmount = (float)currentProcess / process;
        textProcess.text = $"{currentProcess + "/" + process}";
    }
    public void Rewarded()
    {
        tickIcon.gameObject.SetActive(true);
        claimBtnOff.gameObject.SetActive(false);
        claimBtnOn.gameObject.SetActive(false);
        processFill.fillAmount = (float)process / process;
        textProcess.text = $"{process + "/" + process}";
    }
    void CanClaim(bool iscanClaim)
    {
        tickIcon.gameObject.SetActive(false);
        claimBtnOff.gameObject.SetActive(!iscanClaim);
        claimBtnOn.gameObject.SetActive(iscanClaim);
    }
    public void Claim()
    {
        Observer.MissionSound?.Invoke();
        Observer.ShowNoticeIcon?.Invoke(false);
        Rewarded();
        Data.StarMission += startReward;
        Observer.UpdateStarReward?.Invoke();
        Data.MissionRewarded = Data.MissionRewarded * 10 + indexChild;
    }
}
public enum EMissionQuest
{
    PlayLevel,
    SpinWheel,
    WatchAds,
    CompletedHardMode,
    Useswapbooster,
}
