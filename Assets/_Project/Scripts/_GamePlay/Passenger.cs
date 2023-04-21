using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Pancake;
using TMPro;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("ReadOnlyAttribute")]
    [ReadOnly] public bool isMove;
    [ReadOnly] [SerializeField] private Transform road;
    [ReadOnly] public Transform currentDestination;
    [ReadOnly] [SerializeField] private Transform nextDestination;
    [ReadOnly] public bool isSelected;
    [ReadOnly] public Transform path;
    [ReadOnly] [SerializeField] private List<Transform> pathsToDestination = new List<Transform>();
    [ReadOnly] [SerializeField] private EStateAnim currentStateAnim;
    [Header("Attributes")]
    [SerializeField] private AnimancerComponent animancerComponent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public Material angry;
    public Material pleasure;
    public Material normal;
    [SerializeField] private LayerMask passengerLayerMask;
    public GameObject hint;
    [SerializeField] private GameObject passengerModel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField, Range(0, 100)] private float passengerSpeed;
    [SerializeField] private AnimationClip idleAnim;
    [SerializeField] private AnimationClip walkAnim;
    [SerializeField] private AnimationClip winAnim;
    public int rowDestination;
    public EColumn columnDestination;
    private EStateAnim _previousStateAnim;
    private RaycastHit _raycastHit;
    private int _pathindex;
    private bool _isAdd = true;
    private int indexturn;
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
        SetEmotion(normal);
        SetIdleAnim();
    }
    void Initialation()
    {
        Level.Instance.passengers.Add(this);
        hintText.text = columnDestination + ":" + rowDestination;
    }
    public void SetSelected()
    {
        Level.Instance.SetTheSelectedPassenger(this);
    }
    public void SetEmotion(Material getMaterial)
    {
        Material[] mats = skinnedMeshRenderer.materials;
        mats[1] = getMaterial;
        skinnedMeshRenderer.materials = mats;
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
    void SetIdleAnim() => SetStateAnim(EStateAnim.Idle, idleAnim);
    void SetRunAnim() => SetStateAnim(EStateAnim.Run, walkAnim);
    void SetWinAnim() => SetStateAnim(EStateAnim.Win, winAnim);
    void ClickonGround()
    {
        hint.SetActive(false);
    }
    void SetStateAnim(EStateAnim getEStateAnim, AnimationClip getAnimationclip)
    {
        currentStateAnim = getEStateAnim;
        if (currentStateAnim != _previousStateAnim)
        {
            animancerComponent.Play(getAnimationclip);
            _previousStateAnim = currentStateAnim;
        }
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
                        hint.SetActive(false);
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
            if (path.parent != Level.Instance.paths[0])
            {
                if (!pathsToDestination.Contains(path))
                {
                    pathsToDestination.Add(path);
                }
            }
            else
            {
                pathsToDestination.Add(path);
                nextDestination = pathsToDestination[pathsToDestination.Count - 1].parent;
                SetEmotion(normal);
                SetNewDestination(nextDestination);
                isMove = true;
                _isAdd = false;
                Level.Instance.SetTurn();
                indexturn = Level.Instance.currentTurn;
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
            passengerModel.transform.rotation = Quaternion.Euler(0, 180, 0);
            Level.Instance.CheckTurn(indexturn);
        }
        else
        {
            _pathindex++;
        }
    }
    void SetEndDestination(Transform getNextDestination)
    {
        currentDestination = getNextDestination;
        nextDestination = null;
    }
    void SetNewDestination(Transform newdestination)
    {
        if (Level.Instance.groundSelecteds.Contains(currentDestination))
        {
            Level.Instance.groundSelecteds.Remove(currentDestination);
        }
        Level.Instance.groundSelecteds.Add(newdestination);
        currentDestination.gameObject.GetComponent<Ground>().SetDestination(true);
        newdestination.gameObject.GetComponent<Ground>().SetDestination(false);
    }
    public void DoSwapPosi(Vector3 nextPosi, Transform nextCurrentPosi)
    {
        transform.DOLocalMove(nextPosi, 2).OnUpdate((() =>
        {
            isMove = true;
            Level.Instance.OnProcessing();
            SetEmotion(normal);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            SetNewDestination(nextCurrentPosi);
            SetEndDestination(nextCurrentPosi);
        })).OnComplete((() =>
        {
            isMove = false;
            hint.SetActive(false);
            Level.Instance.EndProcessing();
            Level.Instance.EndDoSwap();
        }));
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
                other.gameObject.GetComponent<Ground>().SetDestination(false);
                currentDestination = other.transform;
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
}
