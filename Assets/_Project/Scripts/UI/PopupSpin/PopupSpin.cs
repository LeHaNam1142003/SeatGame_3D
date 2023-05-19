using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSpin : Popup
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI spinAmountText;
    protected override void BeforeShow()
    {
        coinText.text = Data.CurrencyTotal.ToString();
        spinAmountText.text = Data.SpinTicketAmount.ToString();
        base.BeforeShow();
    }
    public void DoSpinWithTicket()
    {
        if (Data.IsTesting)
        {
            Observer.DoSpin?.Invoke();
        }
        else
        {
            if (Data.SpinTicketAmount <= 0) return;
            Observer.DoSpin?.Invoke();
        }
    }
    protected override void AfterHidden()
    {
        PopupController.Instance.Show<PopupUI>();
        base.AfterHidden();
    }
    public void DoSpinWithWatchAds()
    {

    }
}
