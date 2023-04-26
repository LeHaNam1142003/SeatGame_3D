using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBtn : MonoBehaviour
{
    private bool _isSwapping;
    private void OnEnable()
    {
        Observer.OnSwapping += Swapping;
        Observer.EndSwapping += EndSwapping;
    }
    private void OnDisable()
    {
        Observer.OnSwapping -= Swapping;
        Observer.EndSwapping -= EndSwapping;
    }
    public void SwitchPosi()
    {
        if (!_isSwapping)
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
