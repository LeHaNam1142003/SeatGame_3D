using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionContent : MonoBehaviour
{
    [SerializeField] private MissionBoard missionBoard;
    [SerializeField] private List<MissionEachDay> missionEachDays;
    private List<int> _missionRewarded = new List<int>();
    private void Awake()
    {
        Initialize();
    }
    private void OnEnable()
    {
        Observer.NewDailyReWard += Initialize;
        SetProcess();
    }
    private void OnDisable()
    {
        Observer.NewDailyReWard -= Initialize;
    }
    void SetProcess()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int count = 0;
            var getObj = transform.GetChild(i).GetComponent<MissionBoard>();
            if (Data.MissionRewarded == 0)
            {
                getObj.SetTypeMission();
            }
            else
            {
                int tmp = Data.MissionRewarded;
                while (tmp != 0)
                {
                    var g = tmp % 10;
                    _missionRewarded.Add(g);
                    tmp /= 10;
                }
                foreach (var checkMissionRewared in _missionRewarded)
                {
                    if (i + 1 == checkMissionRewared)
                    {
                        getObj.Rewarded();
                        count++;
                    }
                }
                if (count == 0)
                {
                    getObj.SetTypeMission();
                }
            }
        }
    }
    void Initialize()
    {
        if (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        var setIndex = Data.DailyMissionIndex % missionEachDays.Count;
        Debug.Log(setIndex);
        for (int i = 0; i < missionEachDays[setIndex].missions.Count; i++)
        {
            var missionObj = Instantiate(missionBoard, transform);
            missionObj.Init(missionEachDays[setIndex].missions[i].starReward, missionEachDays[setIndex].missions[i].missionTitle, missionEachDays[setIndex].missions[i].requestAmount, missionEachDays[setIndex].missions[i].eMissionQuest, i + 1);
        }
    }
}
[Serializable]
public class MissionEachDay
{
    public List<Mission> missions;
}
[Serializable]
public class Mission
{
    public string missionTitle;
    public int starReward;
    public int requestAmount;
    public EMissionQuest eMissionQuest;
}
