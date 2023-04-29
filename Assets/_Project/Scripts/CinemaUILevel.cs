using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CinemaUILevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image hightlightSelected;
    public void SetLevelText(int indexLevel)
    {
        levelText.text = indexLevel.ToString();
    }
    public void SetHightLight(bool isActive)
    {
        hightlightSelected.gameObject.SetActive(isActive);
    }
}
