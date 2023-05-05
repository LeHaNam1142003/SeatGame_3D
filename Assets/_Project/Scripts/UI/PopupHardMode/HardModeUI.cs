using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HardModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI statusText;
    public void SetIndexHardMode(int index)
    {
        indexText.text = index.ToString();
    }
}
