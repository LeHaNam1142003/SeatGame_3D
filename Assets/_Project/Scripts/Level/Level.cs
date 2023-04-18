using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Pancake;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public static Level Instance;
    [ReadOnly] public List<Transform> paths = new List<Transform>();
    [ReadOnly] public List<Transform> groundSelecteds = new List<Transform>();
    [ReadOnly] [SerializeField] public int currentTurn;
    [ReadOnly] public List<SetUpSeat> setupSeats = new List<SetUpSeat>();
    [ReadOnly] public bool IsWin;
    [ReadOnly] public int bonusMoney;
    [ReadOnly] public List<Passenger> passengers = new List<Passenger>();
    [ReadOnly] public ETool eTool;
    [SerializeField] private int maxTurn;
    [SerializeField] private bool isHaveTools;
    [ShowIf("isHaveTools")] [SerializeField] private List<Button> tools;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI arrangeText;
    private List<Passenger> swaps = new List<Passenger>();
    private bool _isCanTouchGround;
    private bool isDecreaseTurn;
    private int count;
    private bool _isUseTool;
    private bool _isFingerDown;
    private bool _isFingerDrag;
    private bool _isProcessing;

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
        Initialization();
    }
    void Initialization()
    {
        if (isHaveTools)
        {
            foreach (var setTool in tools)
            {
                setTool.gameObject.SetActive(true);
            }
        }
        Instance = this;
        currentTurn = maxTurn;
        UpdateTurn();
    }
    public void SetTheSelectedPassenger(Passenger passenger)
    {
        // Set the Selected Passenger is only one and the others are unselectable
        foreach (var checkPassenger in passengers)
        {
            if (checkPassenger == passenger)
            {
                if (!_isUseTool)
                {
                    checkPassenger.GetSelected(true);
                    _isCanTouchGround = true;
                }
                else
                {
                    if (!passenger.isMove)
                    {
                        switch (eTool)
                        {
                            case ETool.Swap:
                                DoSwap(passenger);
                                break;
                        }
                    }
                    else
                    {
                        EndDoSwap();
                    }
                }
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
    void UpdateArrrangement(int wrongSeat, int maxSeat)
    {
        arrangeText.text = "WrongSeat :" + wrongSeat + "/" + maxSeat;
    }
    public void CheckTurn(int getindex)
    {
        if (getindex == 0)
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
    public void OnProcessing() => _isProcessing = true;
    public void EndProcessing() => _isProcessing = false;
    public void ManageSeat(Seat getSeat, bool isCorrectPassenger)
    {
        // Check all of Seats are correct or not
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
        UpdateArrrangement(count, setupSeats.Count);
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
        Observer.SwapTool += SwapTool;
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        Observer.SwapTool -= SwapTool;
    }
    public void SwapTool()
    {
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(true);
        }
        _isCanTouchGround = false;
        _isUseTool = true;
        eTool = ETool.Swap;
    }
    void DoSwap(Passenger passenger)
    {
        swaps.Add(passenger);
        if (swaps.Count == 2)
        {
            swaps[0].DoSwapPosi(swaps[1].transform.position, swaps[1].currentDestination);
            swaps[1].DoSwapPosi(swaps[0].transform.position, swaps[0].currentDestination);
            swaps.Clear();
        }
    }
    public void EndDoSwap()
    {
        eTool = ETool.Non;
        _isUseTool = false;
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(false);
        }
        swaps.Clear();
    }

    void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (IsWin != true && currentTurn != 0 && !_isProcessing)
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
                        var getPass = hit.collider.gameObject.GetComponent<Passenger>();
                        getPass.SetSelected();
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
public enum ETool
{
    Non,
    Swap,
    Tele,
}
