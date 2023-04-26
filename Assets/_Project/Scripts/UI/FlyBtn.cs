using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;

public class FlyBtn : MonoBehaviour
{
    private bool _isFlying;
    [SerializeField] private TextMeshProUGUI textCount;
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
        if (!_isFlying && Data.FlyToolCount > 0)
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
