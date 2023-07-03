using System;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Pancake;
using Pancake.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : Popup
{
    [SerializeField] private BonusArrowHandler bonusArrowHandler;
    [SerializeField] private GameObject btnRewardAds;
    [SerializeField] private GameObject btnTapToContinue;
    [SerializeField] [ReadOnly] private int totalMoney;
    [SerializeField] private List<Reward> rewards = new List<Reward>();
    [ReadOnly] public List<SetUpReward> setupRewards = new List<SetUpReward>();
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private List<CharacterWin> characterWins = new List<CharacterWin>();
    private Sequence sequence;
    protected override void BeforeShow()
    {
        Observer.PlayWinSound?.Invoke();
        base.BeforeShow();
        var getPopupUI = PopupController.Instance.Get<PopupUI>() as PopupUI;
        getPopupUI.isShowTicket = true;
        getPopupUI.Show();
        Setup();

        sequence = DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { btnTapToContinue.SetActive(true); });
    }
    private void OnEnable()
    {
        foreach (var charWin in characterWins)
        {
            charWin.charAnimacer.Play(charWin.charAnimation);
        }
    }
    public void Setup()
    {
        btnRewardAds.SetActive(true);
        btnTapToContinue.SetActive(false);
        bonusArrowHandler.MoveObject.ResumeMoving();
        if (setupRewards.Count == 0) return;
        foreach (var getReward in rewards)
        {
            foreach (var setupReward in setupRewards)
            {
                if (getReward.eTypeReward == setupReward.eTypeReward)
                {
                    rewardImage.sprite = getReward.iconReward;
                    rewardImage.SetNativeSize();
                    rewardText.text = $"+ {setupReward.number}";
                    if (getReward.eTypeReward == ETypeReward.Money)
                    {
                        totalMoney = setupReward.number;
                    }
                }
            }
        }

    }

    protected override void BeforeHide()
    {
        SoundController.Instance.StopFXSound();
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickAdsReward()
    {
        if (Data.IsTesting)
        {
            GetRewardAds();
            Observer.ClaimReward?.Invoke();
        }
        else
        {
            AdsManager.ShowRewardAds(() =>
            {
                GetRewardAds();
                Observer.ClaimReward?.Invoke();
            }, skipCallback: () =>
            {
                bonusArrowHandler.MoveObject.ResumeMoving();
                btnRewardAds.SetActive(true);
                btnTapToContinue.SetActive(true);
            }, closeCallback: () =>
            {
                bonusArrowHandler.MoveObject.ResumeMoving();
                btnRewardAds.SetActive(true);
                btnTapToContinue.SetActive(true);
            });
        }
    }

    public void GetRewardAds()
    {
        Data.CurrencyTotal += totalMoney * bonusArrowHandler.CurrentAreaItem.MultiBonus;
        bonusArrowHandler.MoveObject.StopMoving();
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);
        sequence?.Kill();

        DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }

    public void OnClickContinue()
    {
        Claim();
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);

        DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }
    void Claim()
    {
        foreach (var setClaim in setupRewards)
        {
            switch (setClaim.eTypeReward)
            {
                case ETypeReward.Money:
                    Data.CurrencyTotal += setClaim.number;
                    break;
                case ETypeReward.Spin:
                    Data.SpinTicketAmount += setClaim.number;
                    break;
            }
        }
        setupRewards.Clear();
    }
}
[Serializable]
public class CharacterWin
{
    public AnimancerComponent charAnimacer;
    public AnimationClip charAnimation;
}
