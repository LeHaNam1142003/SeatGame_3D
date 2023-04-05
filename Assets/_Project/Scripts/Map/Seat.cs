using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Lean.Touch;
using Pancake;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private int setIndexRow;
    [SerializeField] private EColumn setIndexColumn;
    private SeatInfor _seatInfor;
    public BoxCollider seatBox;
    [ReadOnly] public SetUpSeat _setUpSeat;
    private void OnEnable()
    {
        if (isActiveAndEnabled)
        {
            _setUpSeat.seat = this;
            _setUpSeat.isCorrect = false;
            if (!Level.Instance.setupSeats.Contains(_setUpSeat))
            {
                Level.Instance.setupSeats.Add(_setUpSeat);
            }
        }
        _seatInfor = new SeatInfor(setIndexRow, setIndexColumn);
        seatBox.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(NameTag.Passenger))
        {
            var checkPlayer = other.GetComponent<Passenger>();
            if (checkPlayer.rowDestination == _seatInfor.intdexRow && checkPlayer.columnDestination == _seatInfor.indexColumn)
            {
                Level.Instance.ManageSeat(this, true);
            }
            else
            {
                Level.Instance.ManageSeat(this, false);
            }
        }
    }
}
struct SeatInfor
{
    public int intdexRow;
    public EColumn indexColumn;
    public SeatInfor(int getIndexRow, EColumn getIndexColmn)
    {
        intdexRow = getIndexRow;
        indexColumn = getIndexColmn;
    }
}
public enum EColumn
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
}
