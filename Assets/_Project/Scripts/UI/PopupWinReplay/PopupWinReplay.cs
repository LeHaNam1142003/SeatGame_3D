using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWinReplay : Popup
{
    public void Continue()
    {
        GameManager.Instance.ReturnHome();
        Hide();
    }
}
