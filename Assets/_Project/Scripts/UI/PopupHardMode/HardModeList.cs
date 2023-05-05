using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardModeList : ScrollBoard
{
    public override void SetIndexText()
    {
        getObj.GetComponent<HardModeUI>().SetIndexHardMode(index);
    }
}
