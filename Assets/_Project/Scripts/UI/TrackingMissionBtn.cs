using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingMissionBtn : MonoBehaviour
{
    [SerializeField] private GameObject notice;
    private void OnEnable()
    {
        Observer.ShowNoticeIcon += ShowNoticeIcon;
    }
    public void ShowTrackingMission()
    {
        Observer.ClickButton?.Invoke();
        PopupController.Instance.Show<PopupTrackingMission>();
    }
    void ShowNoticeIcon(bool isShowNotice)
    {
        if (notice!=null)
        {
             notice.gameObject.SetActive(isShowNotice);
        }
    }

}
