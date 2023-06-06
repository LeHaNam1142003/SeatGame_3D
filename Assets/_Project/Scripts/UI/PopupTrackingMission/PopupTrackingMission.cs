using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrackingMission : Popup
{
    [SerializeField] private MissionBoard missionBoard;
    [SerializeField] private MissionEachDayData missionEachDayData;
    private List<int> _missionRewarded = new List<int>();
    private void OnEnable()
    {
        Observer.LoadTrackingMission += Init;
    }
    protected override void BeforeShow()
    {
        LoadProcess();
    }
    void LoadProcess()
    {
        if (missionBoard.indexChild==null) return;
        int count = 0;
        int tmp = Data.MissionRewarded;
        while (tmp != 0)
        {
            var g = tmp % 10;
            _missionRewarded.Add(g);
            tmp /= 10;
        }
        foreach (var checkMissionRewared in _missionRewarded)
        {
            if (missionBoard.indexChild == checkMissionRewared)
            {
                missionBoard.Rewarded();
                Observer.ShowNoticeIcon?.Invoke(false);
                count++;
            }
        }
        if (count == 0)
        {
            missionBoard.SetTypeMission();
            Observer.ShowNoticeIcon?.Invoke(true);
        }
    }
    void Init(EMissionQuest getEMissionQuest)
    {
        var setIndex = Data.DailyMissionIndex % missionEachDayData.missionEachDays.Count;
        var m = missionEachDayData.missionEachDays[setIndex].missions;
        for (int i = 0; i < missionEachDayData.missionEachDays[setIndex].missions.Count; i++)
        {
            if (m[i].eMissionQuest == getEMissionQuest)
            {
                missionBoard.Init(m[i].starReward, m[i].missionTitle, m[i].requestAmount, m[i].eMissionQuest, i + 1);
            }
        }
        LoadProcess();
    }
}
