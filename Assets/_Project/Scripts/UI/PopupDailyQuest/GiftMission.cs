using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftMission : MonoBehaviour
{
    [SerializeField] private Image icon;
    private int _needStarToReward;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private Image giftFx;
    [SerializeField] private Image tickIcon;
    private SetUpReward _setUpReward;

    private bool _isCanReward;
    private bool _isCanraiseEvent;
    private void OnEnable()
    {
        Observer.UpdateStarReward += UpdateStar;
    }
    
    public void Init(Sprite sprite, string starText, int starToReward, SetUpReward getsetUpReward)
    {
        _isCanReward = false;
        _isCanraiseEvent = true;
        icon.sprite = sprite;
        startText.text = starText;
        _needStarToReward = starToReward;
        _setUpReward = getsetUpReward;
        UpdateStar();
    }
    void UpdateStar()
    {
        if (_isCanraiseEvent)
        {
            if (Data.StarMission >= _needStarToReward)
            {
                giftFx.gameObject.SetActive(true);
                tickIcon.gameObject.SetActive(false);
                _isCanReward = true;
            }
            else
            {
                giftFx.gameObject.SetActive(false);
                tickIcon.gameObject.SetActive(false);
            }
        }
    }
    public void Reward()
    {
        if (_isCanReward)
        {
            var getPopupWinHardMode = PopupController.Instance.Get<PopupCongratulation>() as PopupCongratulation;
            if (!getPopupWinHardMode.setupRewards.Contains(_setUpReward))
            {
                getPopupWinHardMode.setupRewards.Add(_setUpReward);
            }
            PopupController.Instance.Show<PopupCongratulation>();
            Data.GiftCanReward += 1;
            tickIcon.gameObject.SetActive(true);
            _isCanReward = false;
            _isCanraiseEvent = false;
        }
    }
    public void Rewarded()
    {
        _isCanraiseEvent = false;
        _isCanReward = false;
        giftFx.gameObject.SetActive(true);
        tickIcon.gameObject.SetActive(true);
    }
}
