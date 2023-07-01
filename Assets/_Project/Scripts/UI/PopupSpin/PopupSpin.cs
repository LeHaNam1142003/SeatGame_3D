using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSpin : Popup
{
    private bool _isDoSpinWithTicket;
    public void DoSpinWithTicket()
    {
        Observer.ClickButton?.Invoke();
        _isDoSpinWithTicket = true;
        if (Data.IsTesting)
        {
            Observer.DoSpin?.Invoke(_isDoSpinWithTicket);
        }
        else
        {
            if (Data.SpinTicketAmount <= 0) return;
            Observer.DoSpin?.Invoke(_isDoSpinWithTicket);
        }
    }
    protected override void AfterHidden()
    {
        PopupController.Instance.Hide<PopupUI>();
        var getPopupUI = PopupController.Instance.Get<PopupUI>() as PopupUI;
        getPopupUI.isShowTicket = false;
        getPopupUI.Show();
        Observer.ShowTrackingButton?.Invoke();
        base.AfterHidden();
    }
    public void DoSpinWithWatchAds()
    {
        Observer.ClickButton?.Invoke();
        AdsManager.ShowRewardAds(() =>
        {
            Data.SpinTicketAmount += 1;
        });
    }
}
