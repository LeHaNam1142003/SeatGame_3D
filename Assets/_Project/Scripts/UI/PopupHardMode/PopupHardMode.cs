using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHardMode : Popup
{
    private void OnEnable()
    {
        Observer.PlayHardMode += OnPLayHardMode;
    }
    private void OnDisable()
    {
        Observer.PlayHardMode -= OnPLayHardMode;
    }
    void OnPLayHardMode(int index)
    {
        GameManager.Instance.StartHardModeGame(index);
    }
    
}
