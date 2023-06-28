using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCongratulation : Popup
{
    [SerializeField] private List<Reward> rewards = new List<Reward>();
    [SerializeField] private HorizontalLayoutGroup itemClaim;
    [ReadOnly] public List<SetUpReward> setupRewards = new List<SetUpReward>();
    private GameObject _setIcon;
    public bool isBackHome { get; set; }
    [SerializeField] private Image itemReward;
    [SerializeField] private TextMeshProUGUI itemText;
    protected override void BeforeShow()
    {
        SetReward();
        base.BeforeShow();
    }
    void SetReward()
    {
        foreach (var getReward in rewards)
        {
            foreach (var setupReward in setupRewards)
            {
                if (getReward.eTypeReward == setupReward.eTypeReward)
                {
                    itemReward.sprite = getReward.iconReward;
                    var showIcon = Instantiate(itemReward, itemClaim.transform);
                    itemClaim.childAlignment = TextAnchor.MiddleCenter;
                    itemText.text = $"X {setupReward.number}";
                    var showNumber = Instantiate(itemText, showIcon.transform);
                    showNumber.color = Color.cyan;
                    showNumber.alignment = TextAlignmentOptions.Center;
                    showNumber.alignment = TextAlignmentOptions.Midline;
                    showNumber.rectTransform.anchorMax = new Vector2(0.5f, 0);
                    showNumber.rectTransform.anchorMin = new Vector2(0.5f, 0);
                    showNumber.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    showNumber.rectTransform.anchoredPosition3D = new Vector3(0, -40, 0);
                }
            }
        }
    }
    public void Claim()
    {
        Observer.CongratSound?.Invoke();
        foreach (var setClaim in setupRewards)
        {
            switch (setClaim.eTypeReward)
            {
                case ETypeReward.Tele:
                    Data.SwapToolCount += setClaim.number;
                    break;
                case ETypeReward.Swap:
                    Data.FlyToolCount += setClaim.number;
                    break;
                case ETypeReward.Money:
                    Data.CurrencyTotal += setClaim.number;
                    break;
                case ETypeReward.Spin:
                    Data.SpinTicketAmount += setClaim.number;
                    break;
            }
        }
        setupRewards.Clear();
        for (int i = 0; i < itemClaim.transform.childCount; i++)
        {
            Destroy(itemClaim.transform.GetChild(i).gameObject);
        }
        if (isBackHome)
        {
            GameManager.Instance.ReturnHome();
            isBackHome = false;
        }
        else
        {
            Hide();
        }
    }
}
[Serializable]
public class Reward
{
    public ETypeReward eTypeReward;
    public Sprite iconReward;
}
public enum ETypeReward
{
    Non,
    Tele,
    Swap,
    Money,
    Spin,
}
