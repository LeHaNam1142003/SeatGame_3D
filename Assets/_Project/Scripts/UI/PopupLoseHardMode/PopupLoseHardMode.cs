using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLoseHardMode : Popup
{
    public void BackHome()
    {
        GameManager.Instance.ReturnHome();
    }
}
