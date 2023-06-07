using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxMission : MonoBehaviour
{
    [SerializeField] private GiftMission giftMission;
    [SerializeField] private List<SetUpGift> setUpGifts;
    [SerializeField] private Image processFill;
    [SerializeField] private TextMeshProUGUI resetTime;
    public void Awake()
    {
        SetDailyTimeReset();
        Initialize();
    }
    private void OnEnable()
    {
        Observer.UpdateStarReward += SetProcess;
        SetDailyTimeReset();
        SetProcess();
    }
    private void OnDisable()
    {
        Observer.UpdateStarReward -= SetProcess;
    }
    private void Update()
    {
        UpdateDailyTimeReset();
    }
    void SetProcess()
    {
        processFill.fillAmount = (float)Data.StarMission / setUpGifts[setUpGifts.Count - 1].needStarToReward;
        if (Data.GiftCanReward == 0) return;
        for (int i = 1; i <= Data.GiftCanReward; i++)
        {
            processFill.transform.GetChild(i - 1).GetComponent<GiftMission>().Rewarded();
        }
    }
    void Arrangement()
    {
        int tmp;
        for (int i = 0; i < setUpGifts.Count; i++)
        {
            for (int j = i + 1; j < setUpGifts.Count; j++)
            {
                if (setUpGifts[i].needStarToReward > setUpGifts[j].needStarToReward)
                {
                    tmp = setUpGifts[i].needStarToReward;
                    setUpGifts[i].needStarToReward = setUpGifts[j].needStarToReward;
                    setUpGifts[j].needStarToReward = tmp;
                }
            }
        }
    }
    void Initialize()
    {
        if (setUpGifts.Count == 0) return;
        Arrangement();
        for (int i = 0; i < setUpGifts.Count; i++)
        {
            var getGift = Instantiate(giftMission, processFill.transform);
            getGift.Init(setUpGifts[i].giftIcon, setUpGifts[i].needStarToReward.ToString(), setUpGifts[i].needStarToReward,setUpGifts[i].setUpReward);
        }
    }
    void SetDailyTimeReset()
    {
        if (Data.DailyQuestDay == 0 && Data.DailyQuestYear == 0 && Data.DailyQuestMonth == 0)
        {
            SetPointTime();
        }
        if (DateTime.Now.Day != Data.DailyQuestDay || DateTime.Now.Month != Data.DailyQuestMonth || DateTime.Now.Year != Data.DailyQuestYear)
        {
            ResetNewDailyQuest();
        }
    }
    void SetPointTime()
    {
        Data.DailyQuestDay = DateTime.Now.Day;
        Data.DailyQuestYear = DateTime.Now.Year;
        Data.DailyQuestMonth = DateTime.Now.Month;
    }
    void UpdateDailyTimeReset()
    {
        var currentHour = DateTime.Now.Hour;
        var currentMinute = DateTime.Now.Minute;
        var currentSecond = DateTime.Now.Second;
        resetTime.text = $" Reset After :{24 - currentHour}h {60 - currentMinute}m {60 - currentSecond}s";
    }
    void ResetNewDailyQuest()
    {
        Data.DailyMissionIndex += 1;
        Data.PlayLevel = 0;
        Data.SpinWheel = 0;
        Data.WatchAds = 0;
        Data.CompletedHardMode = 0;
        Data.Useswapbooster = 0;
        Data.StarMission = 0;
        Data.GiftCanReward = 0;
        Data.MissionRewarded = 0;
        Observer.NewDailyReWard?.Invoke();
        SetPointTime();
    }
}
[Serializable]
public class SetUpGift
{
    public Sprite giftIcon;
    public int needStarToReward;
    public SetUpReward setUpReward;
}
