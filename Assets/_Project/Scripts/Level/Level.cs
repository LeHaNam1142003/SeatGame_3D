using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Pancake;
using Spine.Unity;
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
    [ReadOnly] public List<Passenger> passengers = new List<Passenger>();
    [ReadOnly] public ETool eTool;
    [SerializeField] private int maxTurn;
    [SerializeField] private bool isHardMode;
    [ShowIf("isHardMode")] [SerializeField] private StateModeData stateModeData;
    public bool isHaveTools;
    [SerializeField] List<SetUpReward> setupRewards;
    [ShowIf("isHaveTools")] [SerializeField] private List<Button> tools;
    [ShowIf("isHaveTools")] [SerializeField] private GameObject toolBar;
    [SerializeField] private TextMeshProUGUI turnText;
    public bool isGuid;
    [ShowIf("isGuid")] [SerializeField] private SkeletonAnimation fingerSkeletonAnimation;
    [ShowIf("isGuid")] [SerializeField] private AnimationReferenceAsset fingerAnim;
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
    private bool _isSetupStateHardMode;
    private Seat _seatGuid;
    private int _countForPLayMusic;

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
        _isSetupStateHardMode = true;
        isWin = false;
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
                if (isGuid)
                {
                    foreach (var seat in setupSeats)
                    {
                        if (seat.seat.setIndexColumn == checkPassenger.columnDestination && seat.seat.setIndexRow == checkPassenger.rowDestination)
                        {
                            var p = seat.seat.transform.position;
                            DoGuid(new Vector3(p.x, p.y + 2, p.z - 0.5f));
                            _seatGuid = seat.seat;
                        }
                    }
                    checkPassenger.GetSelected(true);
                    _isCanTouchGround = true;
                }
                else
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
        SoundController.Instance.StopFXSound();
        _countForPLayMusic = 0;
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
    public void CheckPlayMusic()
    {
        if (_countForPLayMusic == 0)
        {
            Observer.PlayRunMusic?.Invoke();
            _countForPLayMusic++;
        }
    }
    public void OnProcessing() => _isProcessing = true;
    public void EndProcessing() => _isProcessing = false;
    public void ManageSeat(Seat getSeat, bool isCorrectPassenger)
    {
        // Check all of Seats are correct or not
        if (!isWin)
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
                if (isHaveTools)
                {
                    toolBar.SetActive(false);
                }
                isWin = true;
                OnWin();
                if (fingerSkeletonAnimation != null)
                {
                    fingerSkeletonAnimation.gameObject.SetActive(false);
                }
            }
        }

    }
    void OnEnable()
    {
        if (isGuid)
        {
            foreach (var getPassenger in passengers)
            {
                if (getPassenger.isGuid)
                {
                    var p = getPassenger.transform.position;
                    DoGuid(new Vector3(p.x, p.y + 2, p.z - 0.5f));
                }
            }
        }
        if (isHardMode)
        {
            Observer.LoadTrackingMission?.Invoke(EMissionQuest.CompletedHardMode);
        }
        else
        {
            Observer.LoadTrackingMission?.Invoke(EMissionQuest.PlayLevel);
        }
        Lean.Touch.LeanTouch.OnFingerDown += HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp += HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }
    void DoGuid(Vector3 position)
    {
        fingerSkeletonAnimation.gameObject.SetActive(isGuid);
        fingerSkeletonAnimation.AnimationState.SetAnimation(0, fingerAnim, true);
        fingerSkeletonAnimation.transform.position = position;
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
    public void SwapTool()
    {
        StopHightSeat();
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
        StopHightSeat();
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
            Observer.PlayFlySound?.Invoke();
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
            StopHightSeat();
            Observer.PlaySwapSound?.Invoke();
            _swaps[0].DoSwapPosi(_swaps[1].transform.localPosition, _swaps[1].currentDestination);
            _swaps[1].DoSwapPosi(_swaps[0].transform.localPosition, _swaps[0].currentDestination);
            _swaps.Clear();
            if (!Data.IsTesting)
            {
                Data.SwapToolCount -= 1;
                Data.Useswapbooster += 1;
                Observer.CountSwap?.Invoke();
            }
        }
    }
    public void EndDoSwapTool()
    {
        StopHightSeat();
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
        StopHightSeat();
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
                            foreach (var seat in setupSeats)
                            {
                                seat.seat.StopSelectAnim();
                            }
                            if (eTool != ETool.Fly)
                            {
                                if (isGuid)
                                {
                                    var s = set.seatSurface;
                                    if (s == _seatGuid)
                                    {
                                        set.ShowRobotDetect(set);
                                        _isCanTouchGround = false;
                                    }
                                }
                                else
                                {
                                    set.ShowRobotDetect(set);
                                    _isCanTouchGround = false;
                                }
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
                            Observer.ClickButton?.Invoke();
                            Handheld.Vibrate();
                            var getPass = hit.collider.gameObject.GetComponent<Passenger>();
                            if (isGuid)
                            {
                                if (getPass.isGuid)
                                {
                                    HighLightSeat(getPass);
                                    getPass.SetSelected();
                                    ClearPath();
                                }
                            }
                            else
                            {
                                HighLightSeat(getPass);
                                getPass.SetSelected();
                                ClearPath();
                            }
                        }
                    }
                }
            }
        }
    }
    void HighLightSeat(Passenger passenger)
    {
        foreach (var seat in setupSeats)
        {
            if (seat.seat.setIndexRow == passenger.rowDestination && seat.seat.setIndexColumn == passenger.columnDestination)
            {
                seat.seat.DoSelectAnim();
            }
            else
            {
                seat.seat.StopSelectAnim();
            }
        }
    }
    void StopHightSeat()
    {
        foreach (var seat in setupSeats)
        {
            seat.seat.StopSelectAnim();
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
        if (isHardMode)
        {
            if (Data.IndexHardMode < Data.HardModeUnlock)
            {
                GameManager.Instance.WinReplay();
                SetStateHardMode(EStateMode.Completed);
            }
            else
            {
                GameManager.Instance.WinHardMode(setupRewards);
                SetStateHardMode(EStateMode.Completed);
            }
        }
        else
        {
            GameManager.Instance.OnWinGame(setupRewards);
        }
    }
    void SetStateHardMode(EStateMode stateMode)
    {
        if (_isSetupStateHardMode)
        {
            _isSetupStateHardMode = false;
            SetupStateMode setupStateMode = new SetupStateMode();
            setupStateMode.modeIndex = Data.IndexHardMode;
            setupStateMode.eStateMode = stateMode;
            if (Data.IndexHardMode <= stateModeData.setStateModes.Count)
            {
                if (stateMode != EStateMode.Lost)
                {
                    stateModeData.setStateModes[Data.IndexHardMode - 1].eStateMode = EStateMode.Completed;
                }
            }
            else
            {
                stateModeData.setStateModes.Add(setupStateMode);
            }
        }
    }

    private void OnLose()
    {
        if (fingerSkeletonAnimation != null)
        {
            fingerSkeletonAnimation.gameObject.SetActive(false);
        }
        if (isHardMode)
        {
            GameManager.Instance.LoseHardMode();
            SetStateHardMode(EStateMode.Lost);
        }
        else
        {
            GameManager.Instance.OnLoseGame();
        }
    }
}
[Serializable]
public class SetUpSeat
{
    public Seat seat;
    public bool isCorrect;
}
[Serializable]
public class SetUpReward
{
    public ETypeReward eTypeReward;
    public int number;
}
public enum ETool
{
    Non,
    Swap,
    Fly,
}
