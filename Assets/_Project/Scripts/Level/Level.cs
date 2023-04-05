using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Pancake;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int maxTurn;
    [ReadOnly] [SerializeField] private int currentTurn;
    [SerializeField] private TextMeshProUGUI turnText;
    public static Level Instance;
    [ReadOnly] public List<Transform> paths = new List<Transform>();
    [ReadOnly] public List<Transform> groundSelecteds = new List<Transform>();
    public List<Passenger> passengers = new List<Passenger>();
    [ReadOnly] public List<SetUpSeat> setupSeats = new List<SetUpSeat>();
    private bool _isCanTouchGround;
    private bool isDecreaseTurn;
    private int count;
    [ReadOnly] public bool IsWin;
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
        currentTurn = maxTurn;
        UpdateTurn();
    }
    public void SetTheSelectedPassenger(Passenger passenger)
    {
        foreach (var checkPassenger in passengers)
        {
            if (checkPassenger == passenger)
            {
                checkPassenger.GetSelected(true);
                _isCanTouchGround = true;
            }
            else
            {
                checkPassenger.GetSelected(false);
            }
        }
    }
    public void SetTurn()
    {
        currentTurn--;
        UpdateTurn();
    }
    void UpdateTurn()
    {
        turnText.text = "Turn: " + currentTurn + "/" + maxTurn;
    }
    public void CheckTurn()
    {
        if (currentTurn == 0)
        {
            foreach (var checkSeat in setupSeats)
            {
                if (checkSeat.isCorrect == false)
                {
                    OnLose();
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
            Observer.IntroWinGame?.Invoke();
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
        if (IsWin != true && currentTurn != 0)
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
                        if (_isCanTouchGround)
                        {
                            hit.collider.gameObject.GetComponent<Ground>().ShowRobotDetect(hit.collider.transform);
                            _isCanTouchGround = false;
                        }
                    }
                    else if (hit.collider.gameObject.CompareTag(NameTag.Passenger))
                    {
                        hit.collider.gameObject.GetComponent<Passenger>().SetSelected();
                        ClearPath();
                    }
                }
            }
        }
    }
    void ClearPath()
    {
        for (int i = 0; i < paths.Count; i++)
        {
            var getGround = paths[i].GetComponent<Ground>();
            if (groundSelecteds.Count != 0)
            {
                for (int j = 0; j < groundSelecteds.Count; j++)
                {
                    if (paths[i] != groundSelecteds[j])
                    {
                        getGround.SetGroundBox(true);
                    }
                    else
                    {
                        getGround.SetGroundBox(false);
                        break;
                    }
                }
            }
            else
            {
                getGround.SetGroundBox(true);
            }
            getGround.robotDetect.gameObject.SetActive(false);
        }
        paths.Clear();
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

    public void OnWin()
    {
        GameManager.Instance.OnWinGame();
    }

    public void OnLose()
    {
        GameManager.Instance.OnLoseGame();
    }
}
[Serializable]
public class SetUpSeat
{
    public Seat seat;
    public bool isCorrect;
}
