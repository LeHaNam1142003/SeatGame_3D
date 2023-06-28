using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : Popup 
{
    public bool isShowTicket { get; set; }
    [SerializeField] private GameObject ticketIcon;
    protected override void BeforeShow()
    {
        ticketIcon.gameObject.SetActive(isShowTicket);
        base.BeforeShow();
    }
    public void Debugging()
    {
        Data.CurrencyTotal += 100;
    }
}
