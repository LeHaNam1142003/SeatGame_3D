using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyAmountText;
    [SerializeField] private TextMeshProUGUI currentcySpinTicket;
    [SerializeField] private int stepCount = 10;
    [SerializeField] private float delayTime = .01f;
    [SerializeField] private CurrencyGenerate currencyGenerate;

    private int _currentCoin;
    private int _currentSpinTicket;

    private void Start()
    {
        Observer.SaveCurrencyTotal += SaveCurrency;
        Observer.CurrencyTotalChanged += UpdateCurrencyAmountText;
        currencyAmountText.text = Data.CurrencyTotal.ToString();
        currentcySpinTicket.text = Data.SpinTicketAmount.ToString();
    }

    private void SaveCurrency(bool isMoney)
    {
        if (isMoney)
        {
            _currentCoin = Data.CurrencyTotal;
        }
        else
        {
            _currentSpinTicket = Data.SpinTicketAmount;
        }
    }

    private void UpdateCurrencyAmountText(bool isMoney)
    {
        if (isMoney)
        {
            if (Data.CurrencyTotal > _currentCoin)
            {
                IncreaseCurrency(isMoney);
            }
            else
            {
                DecreaseCurrency(isMoney);
            }
        }
        else
        {
            if (Data.SpinTicketAmount > _currentSpinTicket)
            {
                IncreaseCurrency(isMoney);
            }
            else
            {
                DecreaseCurrency(isMoney);
            }
        }
    }

    private void IncreaseCurrency(bool isMoney)
    {
        bool isPopupUIActive = PopupController.Instance.Get<PopupUI>().isActiveAndEnabled;
        if (!isPopupUIActive) PopupController.Instance.Show<PopupUI>();
        bool isFirstMove = false;
        currencyGenerate.GenerateCoin(() =>
        {
            if (!isFirstMove)
            {
                isFirstMove = true;
                if (isMoney)
                {
                    int currentCurrencyAmount = int.Parse(currencyAmountText.text);
                    int nextAmount = (Data.CurrencyTotal - currentCurrencyAmount) / stepCount;
                    int step = stepCount;
                    CurrencyTextCount(currentCurrencyAmount, nextAmount, step, isMoney);
                }
                else
                {
                    int curentTicketAmount = int.Parse(currentcySpinTicket.text);
                    int nextAmount = (Data.SpinTicketAmount - curentTicketAmount) / stepCount;
                    int step = stepCount;
                    CurrencyTextCount(curentTicketAmount, nextAmount, step, isMoney);
                }
            }
        }, isMoney, () =>
        {
            Observer.CoinMove?.Invoke();
            if (!isPopupUIActive) PopupController.Instance.Hide<PopupUI>();
        });
    }

    private void DecreaseCurrency(bool isMoney)
    {
        if (isMoney)
        {
            int currentCurrencyAmount = int.Parse(currencyAmountText.text);
            int nextAmount = (Data.CurrencyTotal - currentCurrencyAmount) / stepCount;
            int step = stepCount;
            CurrencyTextCount(currentCurrencyAmount, nextAmount, step, isMoney);
        }
        else
        {
            int currentTicket = int.Parse(currentcySpinTicket.text);
            int nextAmount = (Data.SpinTicketAmount - currentTicket) / stepCount;
            int step = stepCount;
            CurrencyTextCount(currentTicket, nextAmount, step, isMoney);
        }
    }

    private void CurrencyTextCount(int currentCurrencyValue, int nextAmountValue, int stepCount, bool isMoney)
    {
        if (stepCount == 0)
        {
            if (isMoney)
            {
                currencyAmountText.text = Data.CurrencyTotal.ToString();
            }
            else
            {
                currentcySpinTicket.text = Data.SpinTicketAmount.ToString();
            }
            return;
        }
        int totalValue = (currentCurrencyValue + nextAmountValue);
        DOTween.Sequence().AppendInterval(delayTime).SetUpdate(isIndependentUpdate: true).AppendCallback(() =>
        {
            if (isMoney)
            {
                currencyAmountText.text = Data.CurrencyTotal.ToString();
            }
            else
            {
                currentcySpinTicket.text = Data.SpinTicketAmount.ToString();
            }
        }).AppendCallback(() =>
        {
            CurrencyTextCount(totalValue, nextAmountValue, stepCount - 1, isMoney);
        });
    }
}
