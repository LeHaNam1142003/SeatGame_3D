using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardModeList : ScrollBoard
{
    [SerializeField] private StateModeData stateModeData;
    public void SetStateMode(int levelAvailable)
    {
        for (int i = 0; i < elements; i++)
        {
            var setup = contentUIs[i].GetComponent<HardModeUI>();
            if (i < levelAvailable)
            {
                if (stateModeData.setStateModes.Count != 0)
                {
                    if (i < stateModeData.setStateModes.Count)
                    {
                        var getState = stateModeData.setStateModes[i].eStateMode;
                        setup.ShowState(getState);
                    }
                    else
                    {
                        setup.ShowState(EStateMode.UnlockItem);
                    }
                }
                else
                {
                    setup.ShowState(EStateMode.UnlockItem);
                }
            }
            else
            {
                setup.ShowState(EStateMode.Lock);
            }
        }
    }
    public void Clear()
    {
        if (content.transform.childCount == 0) return;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
    protected override void SetIndexText()
    {
        getObj.GetComponent<HardModeUI>().SetIndexHardMode(index);
    }
}
