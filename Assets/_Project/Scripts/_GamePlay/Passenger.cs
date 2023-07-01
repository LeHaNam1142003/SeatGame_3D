using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Passenger : MonoBehaviour
{
    [Header("ReadOnlyAttribute")]
    [ReadOnly] public bool isMove;
    public int indexTurn
    {
        get => _defaultTurn;
        set => _defaultTurn = value;
    }
    [ReadOnly] [SerializeField] private Transform road;
    [ReadOnly] public Ground currentDestination;
    [ReadOnly] [SerializeField] private Ground nextDestination;
    [ReadOnly] public bool isSelected;
    [ReadOnly] public Transform path;
    [ReadOnly] [SerializeField] private List<Transform> pathsToDestination = new List<Transform>();
    [ReadOnly] [SerializeField] private EStateAnim currentStateAnim;
    [Header("Attributes")]
    [SerializeField] private AnimancerComponent animancerComponent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererHead;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererBody;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererBot;
    [SerializeField] Material correctHead;
    [SerializeField] Material correctTop;
    [SerializeField] Material correctBot;
    [SerializeField] Material normalHead;
    [SerializeField] Material normalTop;
    [SerializeField] Material normalBot;
    [SerializeField] private Material mainSelected;
    [SerializeField] private Material mainSelectedCorrect;
    [SerializeField] private LayerMask passengerLayerMask;
    [SerializeField] private GameObject passengerModel;
    [SerializeField] private TextMeshProUGUI hintRow;
    [SerializeField] private TextMeshProUGUI hintColumn;
    [SerializeField, Range(0, 100)] private float passengerSpeed;
    [SerializeField] private AnimationClip idleAnim;
    [SerializeField] private AnimationClip walkAnim;
    [SerializeField] private AnimationClip winAnim;
    [SerializeField] private AnimationClip seatAnim;
    [SerializeField] private AnimationClip correctSeatAnim;
    [SerializeField] private AnimationClip wrongSeatAnim;
    [SerializeField] private AnimationClip confusedAnim;
    [SerializeField] private AnimationClip blockAnim;
    [SerializeField] private AnimationClip takePopcorn;
    [SerializeField] private AnimationClip blockSeatAnim;
    [SerializeField] private Image correctSeatEmotion;
    [SerializeField] private Image wrongSeatEmotion;
    [SerializeField] private Emotion passengerEmotion;
    public int rowDestination;
    public EColumn columnDestination;
    [SerializeField] private CapsuleCollider normalCapsu;
    [SerializeField] private CapsuleCollider capsuCheck;
    public bool isGuid;
    public EStateAnim _previousStateAnim;
    private RaycastHit _raycastHit;
    private int _pathindex;
    private bool _isIdle;
    private int _defaultTurn = 1;
    private bool _isDoAnim;
    private bool _isCanCallAction;
    private bool _isAdd = true;
    private void Awake()
    {
        Initialation();
    }
    private void OnEnable()
    {
        Observer.DoneLevel += Win;
    }
    private void OnDisable()
    {
        Observer.DoneLevel -= Win;
    }
    private void Start()
    {
        hintRow.text = rowDestination.ToString();
        hintColumn.text = columnDestination.ToString();
        _previousStateAnim = EStateAnim.Non;
        SetEmotion(Emotion.Normal);
        IdleAnim();
    }
    public void SetCapSuColliderCheck(bool condition)
    {
        capsuCheck.enabled = condition;
        normalCapsu.enabled = !condition;
    }
    void Initialation()
    {
        SetCapSuColliderCheck(true);
        Level.Instance.passengers.Add(this);
        _isIdle = true;
    }
    public void SetSelected()
    {
        Level.Instance.SetTheSelectedPassenger(this);
    }
    public void SetEmotion(Emotion getEmotion)
    {
        switch (getEmotion)
        {
            case Emotion.Normal:
                SetupEmotion(normalHead, normalBot, normalTop, getEmotion);
                break;
            case Emotion.Correct:
                SetupEmotion(correctHead, correctBot, correctTop, getEmotion);
                correctSeatEmotion.gameObject.SetActive(true);
                SetCorrectAnim();
                break;
            case Emotion.Wrong:
                SetupEmotion(normalHead, normalBot, normalTop, getEmotion);
                wrongSeatEmotion.gameObject.SetActive(true);
                SetWrongAnim();
                break;
            case Emotion.Block:
                _isIdle = false;
                if (passengerEmotion == Emotion.Normal)
                {
                    SetBlockAnim();
                }
                else
                {
                    SetBlockSeatAnim();
                }
                break;
        }
    }
    void SetupEmotion(Material mat1, Material mat2, Material mat3, Emotion emotion)
    {
        passengerEmotion = emotion;
        skinnedMeshRendererHead.material = mat1;
        skinnedMeshRendererBot.material = mat2;
        skinnedMeshRendererBody.material = mat3;
    }
    void Win()
    {
        passengerModel.transform.rotation = Quaternion.Euler(0, -90, 0);
        SetWinAnim();
        StartCoroutine(WaitForWin());
    }
    IEnumerator WaitForWin()
    {
        yield return new WaitForSeconds(1f);
        Observer.ShipMove?.Invoke();
    }

    void IdleAnim()
    {
        if (_isIdle)
        {
            switch (passengerEmotion)
            {
                case Emotion.Normal:
                    SetRandomAnim(SetIdleAnim, SetConfusedAnim);
                    break;
                case Emotion.Correct:
                    SetRandomAnim(SetCorrectAnim, SetTakePopcornAnim);
                    break;
                case Emotion.Wrong:
                    SetRandomAnim(SetWrongAnim, SetSeatAnim);
                    break;
            }
        }
    }
    void SetRandomAnim(Action anim1, Action anim2)
    {
        int r = Pancake.Random.Range(1, 3);
        switch (r)
        {
            case 1:
                anim1?.Invoke();
                break;
            case 2:
                anim2?.Invoke();
                break;
        }
    }
    void SetRunAnim() => SetStateAnim(EStateAnim.Run, walkAnim, null);
    void SetWinAnim() => SetStateAnim(EStateAnim.Win, winAnim, IdleAnim);
    void SetCorrectAnim() => SetStateAnim(EStateAnim.Correct, correctSeatAnim, IdleAnim);
    void SetWrongAnim() => SetStateAnim(EStateAnim.Wrong, wrongSeatAnim, IdleAnim);
    void SetSeatAnim() => SetStateAnim(EStateAnim.Seat, seatAnim, IdleAnim);
    void SetConfusedAnim() => SetStateAnim(EStateAnim.Confused, confusedAnim, IdleAnim);
    void SetIdleAnim() => SetStateAnim(EStateAnim.Idle, idleAnim, IdleAnim);
    void SetBlockAnim() => SetStateAnim(EStateAnim.Block, blockAnim, ResetIdle);
    void SetBlockSeatAnim() => SetStateAnim(EStateAnim.BlockSeat, blockSeatAnim, ResetIdle);
    void SetTakePopcornAnim() => SetStateAnim(EStateAnim.TakePopcorn, takePopcorn, IdleAnim);
    void ResetIdle()
    {
        _isIdle = true;
        SetSeatAnim();
    }
    void SetStateAnim(EStateAnim getEStateAnim, AnimationClip getAnimationclip, Action doneAnimEvent)
    {
        currentStateAnim = getEStateAnim;
        if (currentStateAnim != _previousStateAnim)
        {
            _previousStateAnim = currentStateAnim;
            StartCoroutine(Invoke(getAnimationclip, doneAnimEvent));
        }
    }
    IEnumerator Invoke(AnimationClip clip, Action doneAnimEvent)
    {
        var anim = animancerComponent.Play(clip);
        yield return anim;
        _previousStateAnim = EStateAnim.Non;
        doneAnimEvent?.Invoke();
    }

    public void GetSelected(bool isGetSelected)
    {
        if (isMove == false)
        {
            isSelected = isGetSelected;
            if (isGetSelected)
            {
                HightLightSelect(true);
                Observer.StartPoint?.Invoke(currentDestination, this);
            }
            else
            {
                HightLightSelect(false);
            }
        }
    }
    public void HightLightSelect(bool isSelected)
    {
        if (isSelected)
        {
            if (passengerEmotion == Emotion.Normal || passengerEmotion == Emotion.Wrong)
            {
                skinnedMeshRendererHead.material = mainSelected;
            }
            else
            {
                skinnedMeshRendererHead.material = mainSelectedCorrect;
            }
        }
        else
        {
            if (passengerEmotion == Emotion.Normal || passengerEmotion == Emotion.Wrong)
            {
                skinnedMeshRendererHead.material = normalHead;
            }
            else
            {
                skinnedMeshRendererHead.material = correctHead;
            }
        }
    }
    private void Update()
    {
        if (isSelected)
        {
            if (path == null)
            {
                RaycastUp();
                RaycastDown();
                RaycastRight();
                RaycasrLeft();
            }
            else
            {
                AddPathToDestination(path);
                path.GetComponent<RobotDetect>().TakePreviousPath(this);
                if (isMove)
                {
                    if (pathsToDestination.Count != 0)
                    {
                        Level.Instance.CheckPlayMusic();
                        SetCapSuColliderCheck(false);
                        road = pathsToDestination[_pathindex];
                        var dir = road.position - transform.position;
                        transform.Translate(dir.normalized * passengerSpeed * Time.deltaTime);
                        SetRunAnim();
                        _isIdle = false;
                        Quaternion lookRotaion = Quaternion.LookRotation(dir, Vector3.up);
                        passengerModel.transform.rotation = Quaternion.Euler(0, lookRotaion.eulerAngles.y, 0);
                        if (Vector3.Distance(transform.position, road.position) <= 0.05f)
                        {
                            SetNextPath();
                        }
                    }
                }
            }
        }
    }
    public void AddPathToDestination(Transform path)
    {
        if (_isAdd)
        {
            if (path.parent.GetComponent<Ground>() != Level.Instance.paths[0])
            {
                if (!pathsToDestination.Contains(path))
                {
                    pathsToDestination.Add(path);
                }
            }
            else
            {
                pathsToDestination.Add(path);
                nextDestination = pathsToDestination[pathsToDestination.Count - 1].parent.GetComponent<Ground>();
                nextDestination.isTaken = true;
                SetEmotion(Emotion.Normal);
                SetNewDestination(nextDestination);
                isMove = true;
                _isAdd = false;
                Level.Instance.SetTurn();
                indexTurn = Level.Instance.currentTurn;
            }
        }
    }
    void SetNextPath()
    {
        transform.position = new Vector3(road.transform.position.x, transform.position.y, road.transform.position.z);
        if (_pathindex >= pathsToDestination.Count - 1)
        {
            _isIdle = true;
            IdleAnim();
            isMove = false;
            pathsToDestination.Clear();
            SetEndDestination(nextDestination);
            _pathindex = 0;
            path = null;
            isSelected = false;
            _isAdd = true;
            SetCapSuColliderCheck(true);
            passengerModel.transform.rotation = Quaternion.Euler(0, 180, 0);
            if (!currentDestination.GetComponent<Ground>().isHaveSeat)
            {
                Level.Instance.CheckTurn(indexTurn);
            }
        }
        else
        {
            _pathindex++;
        }
    }
    void SetEndDestination(Ground getNextDestination)
    {
        currentDestination = getNextDestination;
        nextDestination = null;
    }
    void SetNewDestination(Ground newdestination)
    {
        if (Level.Instance.groundSelecteds.Contains(currentDestination))
        {
            Level.Instance.groundSelecteds.Remove(currentDestination);
        }
        Level.Instance.groundSelecteds.Add(newdestination);
        currentDestination.gameObject.GetComponent<Ground>().SetDestination(true);
        currentDestination.gameObject.GetComponent<Ground>().isTaken = false;
        newdestination.gameObject.GetComponent<Ground>().SetDestination(false);
        newdestination.gameObject.GetComponent<Ground>().isTaken = true;
    }
    public void DoSwapPosi(Vector3 nextPosi, Ground nextCurrentPosi)
    {
        DoTool(nextPosi, nextCurrentPosi, Level.Instance.EndDoSwapTool);
    }
    void DoTool(Vector3 nextPosi, Ground nextCurrentPosi, Action action)
    {
        transform.DOLocalMove(nextPosi, 2).OnUpdate((() =>
        {
            isMove = true;
            Level.Instance.OnProcessing();
            SetEmotion(Emotion.Normal);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            SetNewDestination(nextCurrentPosi);
            SetEndDestination(nextCurrentPosi);
        })).OnComplete((() =>
        {
            isMove = false;
            Level.Instance.EndProcessing();
            action?.Invoke();
        }));
    }
    public void DoFly(Vector3 flyPosi, Ground nextCurrentPosi)
    {
        DoTool(flyPosi, nextCurrentPosi, Level.Instance.EndDoFlyTool);
    }
    void ShootRaycastCheck(Vector3 direction, Color color)
    {
        Debug.DrawRay(transform.position, direction * 1, color);
        if (Physics.Raycast(transform.position, direction, out _raycastHit, 1, passengerLayerMask))
        {
            if (_raycastHit.collider != null && _raycastHit.collider.gameObject.CompareTag(NameTag.RobotDetect))
            {
                path = _raycastHit.collider.transform;
            }
        }
    }
    void RaycastRight()
    {
        ShootRaycastCheck(Vector3.right, Color.blue);
    }

    void RaycasrLeft()
    {
        ShootRaycastCheck(Vector3.left, Color.red);
    }

    void RaycastDown()
    {
        ShootRaycastCheck(Vector3.back, Color.green);
    }

    void RaycastUp()
    {
        ShootRaycastCheck(Vector3.forward, Color.gray);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(NameTag.GroundCheck))
        {
            if (currentDestination == null)
            {
                var set = other.gameObject.GetComponent<Ground>();
                set.SetDestination(false);
                currentDestination = set;
                set.isTaken = true;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(NameTag.Passenger))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider>());
        }
    }
}
public enum EStateAnim
{
    Non,
    None,
    Idle,
    Run,
    Win,
    Correct,
    Wrong,
    Seat,
    Confused,
    Block,
    BlockSeat,
    TakePopcorn,
}
public enum Emotion
{
    Correct,
    Normal,
    Wrong,
    Block,
}
