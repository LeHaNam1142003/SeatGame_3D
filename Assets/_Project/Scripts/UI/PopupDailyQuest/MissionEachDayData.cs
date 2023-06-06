using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/MissionEachDay")]
public class MissionEachDayData : ScriptableObject
{
    public List<MissionEachDay> missionEachDays;
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
