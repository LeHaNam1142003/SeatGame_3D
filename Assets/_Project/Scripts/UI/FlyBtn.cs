using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyBtn : MonoBehaviour
{
    private bool _isFlying;
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
        textCount.text = Data.FlyToolCount.ToString();
        if (Data.FlyToolCount <= 0)
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
        Observer.CountFly += UpdateTextCount;
    }
    private void OnDisable()
    {
        Observer.OnSwapping -= Swapping;
        Observer.EndSwapping -= EndSwapping;
        Observer.CountFly -= UpdateTextCount;
    }
    public void DoFly()
    {
        Observer.ClickButton?.Invoke();
        if (!_isFlying && Data.FlyToolCount > 0 && !Level.Instance.isGuid)
        {
            Observer.OnSwapping?.Invoke();
            PopupController.Instance.Show<PopupFlyTool>();
            Level.Instance.FlyTool();
        }
    }
    void Swapping()
    {
        _isFlying = true;
    }
    void EndSwapping()
    {
        _isFlying = false;
    }
}
