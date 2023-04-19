using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBtn : MonoBehaviour
{
    public void SwitchPosi()
    {
        PopupController.Instance.Show<PopupSwapTool>();
        Level.Instance.SwapTool();
    }
}
