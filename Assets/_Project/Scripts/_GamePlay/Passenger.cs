using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Material correctBody;
    public Material correctBot;
    public Material normalBody;
    public Material normalBot;
    [SerializeField] private LayerMask passengerLayerMask;
    public GameObject hint;
    [SerializeField] private GameObject passengerModel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField, Range(0, 100)] private float passengerSpeed;
    [SerializeField] private AnimationClip idleAnim;
    [SerializeField] private AnimationClip walkAnim;
    [SerializeField] private AnimationClip winAnim;
    [SerializeField] private AnimationClip seatAnim;
    [SerializeField] private AnimationClip correctSeatAnim;
    [SerializeField] private AnimationClip wrongSeatAnim;
    [SerializeField] private Image correctSeatEmotion;
    [SerializeField] private Image wrongSeatEmotion;
    public int rowDestination;
    public EColumn columnDestination;
    [SerializeField] private CapsuleCollider normalCapsu;
    [SerializeField] private CapsuleCollider capsuCheck;
    public bool isGuid;
    private EStateAnim _previousStateAnim;
    private RaycastHit _raycastHit;
    private int _pathindex;
    private int _defaultTurn = 1;
    private bool _isAdd = true;
    private void Awake()
    {
        Initialation();
    }
    private void OnEnable()
    {
        Observer.ClickonGround += ClickonGround;
        Observer.DoneLevel += Win;
    }
    private void OnDisable()
    {
        Observer.ClickonGround -= ClickonGround;
        Observer.DoneLevel -= Win;
    }
    private void Start()
    {
        hint.SetActive(false);
        _previousStateAnim = EStateAnim.Non;
        SetEmotion(Emotion.Normal);
        SetIdleAnim();
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
        hintText.text = $"{columnDestination}    {rowDestination}";
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
                SetupEmotion(normalBot, normalBody);
                break;
            case Emotion.Correct:
                SetupEmotion(correctBot, correctBody);
                correctSeatEmotion.gameObject.SetActive(true);
                SetCorrectAnim();
                break;
            case Emotion.Wrong:
                SetupEmotion(normalBot, normalBody);
                wrongSeatEmotion.gameObject.SetActive(true);
                SetWrongAnim();
                break;
        }

    }
    void SetupEmotion(Material mat1, Material mat2)
    {
        skinnedMeshRendererHead.material = mat1;
        skinnedMeshRendererBot.material = mat1;
        skinnedMeshRendererBody.material = mat2;
    }
    void Win()
    {
        hint.SetActive(false);
        passengerModel.transform.rotation = Quaternion.Euler(0, -90, 0);
        SetWinAnim();
        StartCoroutine(WaitForWin());
    }
    IEnumerator WaitForWin()
    {
        yield return new WaitForSeconds(1f);
        Observer.ShipMove?.Invoke();
    }
    void SetIdleAnim() => SetStateAnim(EStateAnim.Idle, idleAnim, null);
    void SetRunAnim() => SetStateAnim(EStateAnim.Run, walkAnim, SetIdleAnim);
    void SetWinAnim() => SetStateAnim(EStateAnim.Win, winAnim, SetIdleAnim);
    void SetCorrectAnim() => SetStateAnim(EStateAnim.Correct, correctSeatAnim, SetSeatAnim);
    void SetWrongAnim() => SetStateAnim(EStateAnim.Wrong, wrongSeatAnim, SetSeatAnim);
    void SetSeatAnim() => SetStateAnim(EStateAnim.Seat, seatAnim, null);
    void ClickonGround()
    {
        hint.SetActive(false);
    }
    void SetStateAnim(EStateAnim getEStateAnim, AnimationClip getAnimationclip, Action doneAnimEvent)
    {
        currentStateAnim = getEStateAnim;
        if (currentStateAnim != _previousStateAnim)
        {
            StartCoroutine(Invoke(getAnimationclip, doneAnimEvent));
            _previousStateAnim = currentStateAnim;
        }
    }
    IEnumerator Invoke(AnimationClip clip, Action doneAnimEvent)
    {
        var anim = animancerComponent.Play(clip);
        yield return anim;
        doneAnimEvent?.Invoke();
    }

    public void GetSelected(bool isGetSelected)
    {
        if (isMove == false)
        {
            isSelected = isGetSelected;
            if (isGetSelected)
            {
                hint.SetActive(true);
            }
            else
            {
                hint.SetActive(false);
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
                        hint.SetActive(false);
                        SetCapSuColliderCheck(false);
                        road = pathsToDestination[_pathindex];
                        var dir = road.position - transform.position;
                        transform.Translate(dir.normalized * passengerSpeed * Time.deltaTime);
                        SetRunAnim();
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
        transform.position = road.transform.position;
        if (_pathindex >= pathsToDestination.Count - 1)
        {
            SetIdleAnim();
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
        newdestination.gameObject.GetComponent<Ground>().SetDestination(false);
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
            hint.SetActive(false);
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
    Idle,
    Run,
    Win,
    Correct,
    Wrong,
    Seat,
}
public enum Emotion
{
    Correct,
    Normal,
    Wrong,
}
