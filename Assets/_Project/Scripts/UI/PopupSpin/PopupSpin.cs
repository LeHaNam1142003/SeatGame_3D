using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSpin : Popup
{
    [SerializeField] private TextMeshProUGUI spinAmountText;
    private bool _isDoSpinWithTicket;
    protected override void BeforeShow()
    {
        UpdateText();
        base.BeforeShow();
    }
    private void OnEnable()
    {
        Observer.UpdateText += UpdateText;
    }
    private void OnDisable()
    {
        Observer.UpdateText -= UpdateText;
    }
    void UpdateText()
    {
        spinAmountText.text = Data.SpinTicketAmount.ToString();
    }
    public void DoSpinWithTicket()
    {
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
        PopupController.Instance.Show<PopupUI>();
        base.AfterHidden();
    }
    public void DoSpinWithWatchAds()
    {
        AdsManager.ShowRewardAds(() =>
        {
            Data.SpinTicketAmount += 1;
            UpdateText();
        });
    }
    public void ShowTrackingMission()
    {
        PopupController.Instance.Show<PopupTrackingMission>();
    }
}
