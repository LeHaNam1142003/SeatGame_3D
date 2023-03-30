using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Pancake;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance;
    public List<Transform> paths = new List<Transform>();
    public List<Passenger> passengers = new List<Passenger>();
    public List<SetUpSeat> setupSeats = new List<SetUpSeat>();
    public bool isCanTouchGround;
    public Passenger theChoosenOne;
    private int count;
    [ReadOnly] public int bonusMoney;

    private bool _isFingerDown;
    private bool _isFingerDrag;

    private Camera Camera => GetComponentInChildren<Camera>(true);

#if UNITY_EDITOR
    [Button]
    private void StartLevel()
    {
        Data.CurrentLevel = Utility.GetNumberInAString(gameObject.name);

        EditorApplication.isPlaying = true;
    }
#endif
    private void Awake()
    {
        Instance = this;
    }
    public void SetTheSelectedPassenger(Passenger passenger)
    {
        if (theChoosenOne == null)
        {
            foreach (var checkPassenger in passengers)
            {
                if (checkPassenger == passenger)
                {
                    checkPassenger.GetSelected(true);
                    isCanTouchGround = true;
                }
                else
                {
                    checkPassenger.GetSelected(false);
                }
            }
        }
    }
    public void ManageSeat(Seat getSeat, bool isCorrectPassenger)
    {
        count = 0;
        foreach (var setupSeat in setupSeats)
        {
            if (setupSeat.seat == getSeat)
            {
                setupSeat.isCorrect = isCorrectPassenger;
            }
        }
        for (int i = 0; i < setupSeats.Count; i++)
        {
            if (setupSeats[i].isCorrect == false)
            {
                count++;
            }
        }
        if (count == 0)
        {
           GameManager.Instance.OnWinGame();
        }
    }


    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp += HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (GameManager.Instance.gameState != GameState.WinGame)
        {
            if (!finger.IsOverGui)
            {
                _isFingerDown = true;

                //Get Object raycast hit
                var ray = finger.GetRay(Camera);
                var hit = default(RaycastHit);

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                {
                    //ADDED LAYER SELECTION
                    if (hit.collider.gameObject.CompareTag(NameTag.GroundCheck))
                    {
                        if (isCanTouchGround)
                        {
                            hit.collider.gameObject.GetComponent<Ground>().ShowRobotDetect(hit.collider.transform);
                            isCanTouchGround = false;
                        }
                    }
                    else if (hit.collider.gameObject.CompareTag(NameTag.Passenger))
                    {
                        hit.collider.gameObject.GetComponent<Passenger>().SetSelected();
                    }
                }
            }
        }
    }

    void HandleFingerUp(Lean.Touch.LeanFinger finger)
    {
        _isFingerDown = false;
    }

    void HandleFingerUpdate(Lean.Touch.LeanFinger finger)
    {
        if (_isFingerDown)
        {
            _isFingerDrag = true;
        }
    }

    // private void Start()
    // {
    //     Observer.WinLevel += OnWin;
    //     Observer.LoseLevel += OnLose;
    // }
    //
    // private void OnDestroy()
    // {
    //     Observer.WinLevel -= OnWin;
    //     Observer.LoseLevel -= OnLose;
    // }
    
    // public void OnWin(Level level)
    // {
    //     GameManager.Instance.OnWinGame();
    // }
    //
    // public void OnLose(Level level)
    // {
    // }
}
[Serializable]
public class SetUpSeat
{
    public Seat seat;
    public bool isCorrect;
}
