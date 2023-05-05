using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HardModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    private int _getIndex;
    [SerializeField] private TextMeshProUGUI statusText;
    public void SetIndexHardMode(int index)
    {
        _getIndex = index;
        indexText.text = _getIndex.ToString();
    }
    public void PlayHardMode()
    {
        Observer.PlayHardMode?.Invoke(_getIndex);
    }
}
