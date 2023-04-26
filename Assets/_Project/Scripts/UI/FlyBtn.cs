using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class FlyBtn : MonoBehaviour
{
    private bool _isFlying;
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
    public void DoFly()
    {
        if (!_isFlying)
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
