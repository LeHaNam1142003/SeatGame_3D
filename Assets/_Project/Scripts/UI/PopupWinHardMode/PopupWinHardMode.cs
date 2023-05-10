using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWinHardMode : Popup
{
    [SerializeField] private List<Reward> rewards = new List<Reward>();
    [SerializeField] private GameObject itemClaim;
    [ReadOnly] public List<SetUpReward> setupRewards = new List<SetUpReward>();
    private Image _setIcon;
    private GameObject a;
    private TextMeshProUGUI _numberText;
    protected override void BeforeShow()
    {
        SetReward();
        base.BeforeShow();
    }
    void SetReward()
    {
        Debug.Log("2");
        for (int i = 0; i < rewards.Count; i++)
        {
            for (int j = 0; j < setupRewards.Count; j++)
            {
                if (rewards[i].eTypeReward == setupRewards[i].eTypeReward)
                {
                    a = new GameObject();
                    Instantiate(a, itemClaim.transform);
                }
            }
        }
        // foreach (var getReward in rewards)
        // {
        //     foreach (var setupReward in setupRewards)
        //     {
        //         if (getReward.eTypeReward == setupReward.eTypeReward)
        //         {
        //             _setIcon.sprite = getReward.iconReward;
        //             var showIcon = Instantiate(_setIcon, itemClaim.transform);
        //             Debug.Log("sinh");
        //             itemClaim.childAlignment = TextAnchor.MiddleCenter;
        //             _numberText.text = $"X {setupReward.number}";
        //             var showNumber = Instantiate(_numberText, showIcon.transform);
        //             Debug.Log("sinnh");
        //             showNumber.rectTransform.anchorMax = new Vector2(0.5f, 0);
        //             showNumber.rectTransform.anchorMin = new Vector2(0.5f, 0);
        //             showNumber.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        //             showNumber.rectTransform.anchoredPosition3D = new Vector3(0, -40, 0);
        //         }
        //     }
        // }
    }
    public void Claim()
    {

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
}
