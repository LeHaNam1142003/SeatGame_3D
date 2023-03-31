using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    public RobotDetect robotDetect;
    [SerializeField] private BoxCollider groundBox;
    [SerializeField] private GameObject seat;
    [SerializeField] private Seat seatSurface;
    [SerializeField] private bool isHaveSeat;

    private void Start()
    {
        SetSeat();
        HideRobotDetect();
    }
    void SetSeat()
    {
        if (isHaveSeat)
        {
            seat.gameObject.SetActive(true);
        }
        else
        {
            seat.gameObject.SetActive(false);
        }
    }
    void SeatCheck(bool isEnable)
    {
        if (isHaveSeat)
        {
            seatSurface.seatBox.enabled = isEnable;
            if (isEnable==false)
            {
                Level.Instance.ManageSeat(seatSurface, false);
            }
        }
    }

    void HideRobotDetect()
    {
        robotDetect.gameObject.SetActive(false);
        SetGroundBox(true);
    }

    public void ShowRobotDetect(Transform getGround)
    {
        if (!Level.Instance.paths.Contains(getGround.transform))
        {
            Level.Instance.paths.Add(getGround.transform);
        }
        SetGroundBox(false);
        robotDetect.gameObject.SetActive(true);
        Observer.ClickonGround?.Invoke();
    }
    public void SetGroundBox(bool isEnable)
    {
        groundBox.enabled = isEnable;
    }
    public void SetDestination(bool isEnable)
    {
        switch (isEnable)
        {
            case true:
                SetGroundBox(true);
                SeatCheck(false);
                break;
            case false:
                SetGroundBox(false);
                SeatCheck(true);
                break;
        }
    }
}
