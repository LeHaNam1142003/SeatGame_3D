using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Animancer;
using Lean.Touch;
using Pancake;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public int setIndexRow;
    public EColumn setIndexColumn;
    [SerializeField] private AnimancerComponent animancerComponent;
    [SerializeField] private AnimationClip selectAnim;
    [SerializeField] private AnimationClip idleAnim;
    [SerializeField] private ParticleSystem selectEffect;
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
    public void DoSelectAnim()
    {
        selectEffect.gameObject.SetActive(true);
        selectEffect.Play();
        DoAnim(selectAnim);
    }
    public void StopSelectAnim()
    {
        selectEffect.gameObject.SetActive(false);
        selectEffect.Stop();
        DoAnim(idleAnim);
    }
    void DoAnim(AnimationClip animationClip)
    {
        animancerComponent.Play(animationClip);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(NameTag.Passenger))
        {
            var checkPlayer = other.GetComponent<Passenger>();
            if (checkPlayer.rowDestination == _seatInfor.intdexRow && checkPlayer.columnDestination == _seatInfor.indexColumn)
            {
                Level.Instance.ManageSeat(this, true);
                Level.Instance.CheckTurn(checkPlayer.indexTurn);
                checkPlayer.SetEmotion(Emotion.Correct);
            }
            else
            {
                Level.Instance.ManageSeat(this, false);
                Level.Instance.CheckTurn(checkPlayer.indexTurn);
                checkPlayer.SetEmotion(Emotion.Wrong);
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
