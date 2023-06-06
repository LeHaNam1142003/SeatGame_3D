using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Laputa.Localization.Components;
using Pancake;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInGame : Popup
{
    public GameObject levelText;
    public TextMeshProUGUI levelTypeText;
    public TypeOfGame typeOfGame;
    [SerializeField] private Button rePlayBtn;
    public bool isHardMode { set; get; }
    private List<UIEffect> UIEffects => GetComponentsInChildren<UIEffect>().ToList();

    public void Start()
    {
        Observer.WinLevel += HideUI;
        Observer.LoseLevel += HideUI;
    }

    public void OnDestroy()
    {
        Observer.WinLevel -= HideUI;
        Observer.LoseLevel -= HideUI;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        if (!Data.IsTesting) AdsManager.ShowBanner();
        rePlayBtn.gameObject.SetActive(!isHardMode);
        Setup();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        AdsManager.HideBanner();
    }

    public void Setup()
    {
        levelText.GetComponent<LocalizedText>()?.SetValue("x", Data.CurrentLevel.ToString());
        levelTypeText.text = $"Level {(Data.UseLevelABTesting == 0 ? "A" : "B")}";
    }

    public void OnClickHome()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        GameManager.Instance.ReturnHome();
    }

    public void OnClickReplay()
    {
        if (Data.IsTesting)
        {
            GameManager.Instance.ReplayGame(isHardMode);
        }
        else
        {
            AdsManager.ShowInterstitial(() =>
            {
                MethodBase function = MethodBase.GetCurrentMethod();
                Observer.TrackClickButton?.Invoke(function.Name);

                GameManager.Instance.ReplayGame(isHardMode);
            });
        }
    }

    public void OnClickPrevious()
    {
        GameManager.Instance.BackLevel(isHardMode);
    }

    public void OnClickSkip()
    {
        if (Data.IsTesting)
        {
            GameManager.Instance.NextLevel(isHardMode);
        }
        else
        {
            AdsManager.ShowRewardAds(() =>
            {
                MethodBase function = MethodBase.GetCurrentMethod();
                Observer.TrackClickButton?.Invoke(function.Name);

                GameManager.Instance.NextLevel(isHardMode);
            });
        }
    }

    public void OnClickLevelA()
    {
        Data.UseLevelABTesting = 0;
        GameManager.Instance.ReplayGame(isHardMode);
    }

    public void OnClickLevelB()
    {
        Data.UseLevelABTesting = 1;
        GameManager.Instance.ReplayGame(isHardMode);
    }

    public void OnClickLose()
    {
        GameManager.Instance.OnLoseGame(1f);
    }

    public void OnClickWin()
    {
        GameManager.Instance.OnWinGame(1f);
    }

    private void HideUI(Level level = null)
    {
        foreach (UIEffect item in UIEffects)
        {
            item.PlayAnim();
        }
    }
}
public enum TypeOfGame
{
    Normal,
    HardMode,
    SuperHardMode,
}
