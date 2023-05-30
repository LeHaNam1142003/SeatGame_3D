using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class SpinBoard : MonoBehaviour
{
    [SerializeField] private List<ItemSpin> itemSpins;
    [SerializeField] AnimationReferenceAsset doSpinAnim;
    [SerializeField] AnimationReferenceAsset idleSpinAnim;
    [SerializeField] AnimationReferenceAsset doneSpinAnim;
    [SerializeField] private SkeletonGraphic spinSkeletonGraphic;
    private int _randomNumber;
    private int _endSpin;
    private float _endAngle;
    private void OnEnable()
    {
        Initialize();
    }
    void Initialize()
    {
        Observer.DoSpin += DoSpin;
        DoSpinAnim(idleSpinAnim, true);
        for (int i = 0; i < itemSpins.Count; i++)
        {
            var a = transform.GetChild(i).GetComponent<Image>();
            a.sprite = itemSpins[i].icon;
            var b = a.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            b.text = itemSpins[i].number.ToString();
        }
    }
    void DoSpinAnim(AnimationReferenceAsset anim, bool isLoop)
    {
        spinSkeletonGraphic.AnimationState.SetAnimation(0, anim, isLoop);
    }
    private void OnDisable()
    {
        Observer.DoSpin -= DoSpin;
        transform.eulerAngles = Vector3.zero;
    }
    void DoSpin(bool isSpinWithTicket)
    {
        Data.SpinWheel += 1;
        SetRamdom();
        DoSpinAnim(doSpinAnim, true);
        transform.DORotate(new Vector3(0, 0, -1080 - (_endAngle * (_endSpin + (_endSpin - 1)))), 5, RotateMode.FastBeyond360).OnUpdate((() =>
        {
            Observer.DoSpin -= DoSpin;
        })).OnComplete((() =>
        {
            Observer.DoSpin += DoSpin;
            SetUpReward s = new SetUpReward();
            s.number = itemSpins[_endSpin - 1].number;
            s.eTypeReward = itemSpins[_endSpin - 1].eTypeReward;
            var getPopupWinHardMode = PopupController.Instance.Get<PopupCongratulation>() as PopupCongratulation;
            if (!getPopupWinHardMode.setupRewards.Contains(s))
            {
                getPopupWinHardMode.setupRewards.Add(s);
            }
            PopupController.Instance.Show<PopupCongratulation>();
            if (isSpinWithTicket)
            {
                if (Data.SpinTicketAmount <= 0) return;
                Data.SpinTicketAmount -= 1;
                Observer.UpdateText?.Invoke();
            }
            DoSpinAnim(doneSpinAnim, true);
        }));
    }
    void SetRamdom()
    {
        _endAngle = 360.0f / 16.0f;
        _randomNumber = Pancake.Random.Range(0, 101);
        for (int i = 0; i < itemSpins.Count; i++)
        {
            if (itemSpins[i].minRatio <= _randomNumber && _randomNumber <= itemSpins[i].maxRatio)
            {
                _endSpin = i + 1;
            }
        }
    }
}
[Serializable]
public class ItemSpin
{
    public float minRatio;
    public float maxRatio;
    public Sprite icon;
    public ETypeReward eTypeReward;
    public int number;
}
