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
    [ReadOnly] public List<Ground> paths = new List<Ground>();
    [ReadOnly] public List<Ground> groundSelecteds = new List<Ground>();
    [ReadOnly] public int currentTurn;
    [ReadOnly] public List<SetUpSeat> setupSeats = new List<SetUpSeat>();
    [ReadOnly] public bool isWin;
    [ReadOnly] public int bonusMoney;
    [ReadOnly] public List<Passenger> passengers = new List<Passenger>();
    [ReadOnly] public ETool eTool;
    [SerializeField] private int maxTurn;
    public bool isHaveTools;
    [ShowIf("isHaveTools")] [SerializeField] private List<Button> tools;
    [ShowIf("isHaveTools")] [SerializeField] private GameObject toolBar;
    [SerializeField] private TextMeshProUGUI turnText;
    private List<Passenger> _swaps = new List<Passenger>();
    private Passenger _flyPassenger;
    private bool _isCanTouchPlayer;
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
            toolBar.SetActive(true);
        }
        _isCanTouchPlayer = true;
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
                                DoSwapTool(passenger);
                                break;
                            case ETool.Fly:
                                SetFlyTool(passenger);
                                break;
                        }
                    }
                    else
                    {
                        EndDoSwapTool();
                        EndDoFlyTool();
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
        turnText.text = currentTurn.ToString();
    }
    public void CheckTurn(int getindex)
    {
        if (getindex == 0 || getindex != 0 && currentTurn == 0)
        {
            foreach (var checkSeat in setupSeats)
            {
                if (checkSeat.isCorrect == false)
                {
                    int count = 0;
                    foreach (var passenger in passengers)
                    {
                        if (passenger.isMove)
                        {
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                        OnLose();
                    }
                    break;
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
        if (count == 0)
        {
            if (isHaveTools)
            {
                toolBar.SetActive(false);
            }
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
    public void SwapTool()
    {
        _swaps.Clear();
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(true);
        }
        _isCanTouchGround = false;
        _isUseTool = true;
        eTool = ETool.Swap;
    }
    public void FlyTool()
    {
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(true);
        }
        _isCanTouchGround = false;
        _isUseTool = true;
        eTool = ETool.Fly;
    }
    void SetFlyTool(Passenger passenger)
    {
        _flyPassenger = passenger;
        _isCanTouchPlayer = false;
        _isCanTouchGround = true;
    }
    void DoFlyTool(Vector3 flyPosi, Ground nextCurrentPosi)
    {
        Observer.OnSwapping?.Invoke();
        PopupController.Instance.Hide<PopupSwapTool>();
        if (_flyPassenger != null)
        {
            PopupController.Instance.Hide<PopupFlyTool>();
            _flyPassenger.DoFly(flyPosi, nextCurrentPosi);
            if (!Data.IsTesting)
            {
                Data.FlyToolCount -= 1;
                Observer.CountFly?.Invoke();
            }
        }
    }
    void DoSwapTool(Passenger passenger)
    {
        if (!_swaps.Contains(passenger))
        {
            _swaps.Add(passenger);
        }
        if (_swaps.Count == 2)
        {
            Observer.OnSwapping?.Invoke();
            PopupController.Instance.Hide<PopupSwapTool>();
            _swaps[0].DoSwapPosi(_swaps[1].transform.localPosition, _swaps[1].currentDestination);
            _swaps[1].DoSwapPosi(_swaps[0].transform.localPosition, _swaps[0].currentDestination);
            _swaps.Clear();
            if (!Data.IsTesting)
            {
                Data.SwapToolCount -= 1;
                Observer.CountSwap?.Invoke();
            }
        }
    }
    public void EndDoSwapTool()
    {
        eTool = ETool.Non;
        _isUseTool = false;
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(false);
        }
        _swaps.Clear();
        Observer.EndSwapping?.Invoke();
        PopupController.Instance.Hide<PopupSwapTool>();
    }
    public void EndDoFlyTool()
    {
        eTool = ETool.Non;
        _isUseTool = false;
        foreach (var setpassengers in passengers)
        {
            setpassengers.hint.SetActive(false);
        }
        _flyPassenger = null;
        _isCanTouchPlayer = true;
        Observer.EndSwapping?.Invoke();
        PopupController.Instance.Hide<PopupFlyTool>();
    }

    void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (isWin != true && currentTurn != 0 && !_isProcessing)
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
                        var set = hit.collider.gameObject.GetComponent<Ground>();
                        if (_isCanTouchGround)
                        {
                            if (eTool != ETool.Fly)
                            {
                                set.ShowRobotDetect(set);
                                _isCanTouchGround = false;
                            }
                            else
                            {
                                DoFlyTool(set.transform.localPosition, set);
                            }
                        }
                    }
                    else if (hit.collider.gameObject.CompareTag(NameTag.Passenger))
                    {
                        if (_isCanTouchPlayer)
                        {
                            var getPass = hit.collider.gameObject.GetComponent<Passenger>();
                            getPass.SetSelected();
                            ClearPath();
                        }
                    }
                }
            }
        }
    }
    void ClearPath()
    {
        for (int i = 0; i < paths.Count; i++)
        {
            var getGround = paths[i];
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

    private void Start()
    {
        Observer.OnWin += OnWin;
        Observer.OnLost += OnLose;
    }

    private void OnDestroy()
    {
        Observer.OnWin -= OnWin;
        Observer.OnLost -= OnLose;
    }

    private void OnWin()
    {
        GameManager.Instance.OnWinGame();
    }

    private void OnLose()
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
    Fly,
}
