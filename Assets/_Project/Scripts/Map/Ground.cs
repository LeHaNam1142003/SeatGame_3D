using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    public RobotDetect robotDetect;
    [SerializeField] private BoxCollider groundBox;
    public int x;
    public int y;
    public int mark;
    [SerializeField] private GameObject seat;
    public Seat seatSurface;
    public bool isHaveSeat;
    [SerializeField] private MeshRenderer groundModel;
    public bool isTaken;

    private void Awake()
    {
        SetSeat();
        HideRobotDetect();
    }
    void SetSeat()
    {
        seat.gameObject.SetActive(isHaveSeat);
    }
    void SeatCheck(bool isEnable)
    {
        if (isHaveSeat)
        {
            seatSurface.seatBox.enabled = isEnable;
            if (isEnable == false)
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

    public void ShowRobotDetect(Ground getGround)
    {
        if (!Level.Instance.paths.Contains(getGround))
        {
            Level.Instance.paths.Add(getGround);
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
