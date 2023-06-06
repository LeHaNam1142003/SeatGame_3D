using System.Reflection;

public class PopupHome : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickStart()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        GameManager.Instance.StartGame(false);
    }

    public void OnClickDebug()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupDebug>();
    }
    public void OnClickSpin()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        Observer.LoadTrackingMission?.Invoke(EMissionQuest.SpinWheel);

        PopupController.Instance.Show<PopupSpin>();
    }

    public void OnClickSetting()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupSetting>();
    }
    public void OnclickDailyQuest()
    {
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
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        PopupController.Instance.Show<PopupTest>();
    }
    public void OnClickHardMode()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        Observer.LoadTrackingMission?.Invoke(EMissionQuest.CompletedHardMode);

        PopupController.Instance.Show<PopupHardMode>();
    }


}
