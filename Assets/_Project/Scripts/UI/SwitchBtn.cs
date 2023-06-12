using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SwitchBtn : MonoBehaviour
{
    private bool _isSwapping;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private Image cantUseImage;
    private void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        UpdateTextCount();
    }
    void UpdateTextCount()
    {
        textCount.text = Data.SwapToolCount.ToString();
        if (Data.SwapToolCount <= 0)
        {
            cantUseImage.enabled = true;
        }
        else
        {
            cantUseImage.enabled = false;
        }
    }
    private void OnEnable()
    {
        Observer.OnSwapping += Swapping;
        Observer.EndSwapping += EndSwapping;
        Observer.CountSwap += UpdateTextCount;
    }
    private void OnDisable()
    {
        Observer.OnSwapping -= Swapping;
        Observer.EndSwapping -= EndSwapping;
        Observer.CountSwap -= UpdateTextCount;
    }
    public void SwitchPosi()
    {
        Observer.ClickButton?.Invoke();
        if (!_isSwapping && Data.SwapToolCount > 0)
        {
            Observer.OnSwapping?.Invoke();
            PopupController.Instance.Show<PopupSwapTool>();
            Level.Instance.SwapTool();
        }
    }
    void Swapping()
    {
        _isSwapping = true;
    }
    void EndSwapping()
    {
        _isSwapping = false;
    }
}
