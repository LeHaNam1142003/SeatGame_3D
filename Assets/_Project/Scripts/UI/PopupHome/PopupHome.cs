using System;
using System.Reflection;
using UnityEngine;

public class PopupHome : Popup
{
    [SerializeField] private GameObject trackingButton;
    protected override void BeforeShow()
    {
        base.BeforeShow();
        ShowTracking();
        var getPopupUI = PopupController.Instance.Get<PopupUI>() as PopupUI;
        getPopupUI.isShowTicket = false;
        getPopupUI.Show();
        Observer.LoadTrackingMission?.Invoke(EMissionQuest.PlayLevel);
    }
    private void OnEnable()
    {
        Observer.ShowTrackingButton += ShowTracking;
    }
    private void OnDisable()
    {
        Observer.ShowTrackingButton -= ShowTracking;
    }
    void ShowTracking() => trackingButton.gameObject.SetActive(true);

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickStart()
    {
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        GameManager.Instance.StartGame(false);
    }

    public void OnClickDebug()
    {
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupDebug>();
    }
    public void OnClickSpin()
    {
        trackingButton.gameObject.SetActive(false);
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        Observer.LoadTrackingMission?.Invoke(EMissionQuest.SpinWheel);
        var getPopupUI = PopupController.Instance.Get<PopupUI>() as PopupUI;
        getPopupUI.isShowTicket = true;
        getPopupUI.Show();
        PopupController.Instance.Show<PopupSpin>();
    }

    public void OnClickSetting()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        Observer.ClickButton?.Invoke();

        PopupController.Instance.Show<PopupSetting>();
    }
    public void OnclickDailyQuest()
    {
        trackingButton.gameObject.SetActive(false);
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupDailyQuest>();
    }

    public void OnClickDailyReward()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupDailyReward>();
    }

    public void OnClickShop()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupShop>();
    }

    public void OnClickTest()
    {
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupTest>();
    }
    public void OnClickHardMode()
    {
        trackingButton.gameObject.SetActive(false);
        Observer.ClickButton?.Invoke();
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        Observer.LoadTrackingMission?.Invoke(EMissionQuest.CompletedHardMode);

        PopupController.Instance.Show<PopupHardMode>();
    }


}
