using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDailyQuest : Popup
{
    public void OnclickWatchAds()
    {
        Observer.ClickButton?.Invoke();
        AdsManager.ShowRewardAds(() =>
        {
            Data.WatchAds += 1;
            Observer.UpdateProcressDaily?.Invoke();
        });
    }
}
