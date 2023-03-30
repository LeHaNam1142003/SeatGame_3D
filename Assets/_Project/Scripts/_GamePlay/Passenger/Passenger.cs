using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private LayerMask passengerLayerMask;
    private bool _isMove;
    [ReadOnly] [SerializeField] private Transform road;
    [ReadOnly] [SerializeField] private Transform currentdestination;
    [ReadOnly] [SerializeField] private Transform nextdestination;
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject passengerModel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField, Range(0, 100)] private float passengerSpeed;
    public int rowDestination;
    public EColumn columnDestination;
    private RaycastHit _raycastHit;
    private int _pathindex = 0;
    public bool isclear;
    public bool isAdd;
    private float _currentx;
    private float _currentz;
    [ReadOnly] public bool isSelected;
    [ReadOnly] public Transform path;
    [ReadOnly] [SerializeField] private List<Transform> pathsToDestination = new List<Transform>();
    private void Awake()
    {
        Level.Instance.passengers.Add(this);
    }
    private void OnEnable()
    {
        Observer.ClickonGround += ClickonGround;
    }
    private void OnDisable()
    {
        Observer.ClickonGround -= ClickonGround;
    }
    private void Start()
    {
        hint.SetActive(false);
    }
    void SetCurrentPosi()
    {
        _currentx = transform.position.x;
        _currentz = transform.position.z;
    }
    public void SetSelected()
    {
        Level.Instance.SetTheSelectedPassenger(this);
        isAdd = true;
    }
    void ClickonGround()
    {
        hint.SetActive(false);
    }
    public void GetSelected(bool isGetSelected)
    {
        if (_isMove == false)
        {
            isSelected = isGetSelected;
            if (isGetSelected)
            {
                hint.SetActive(true);
                hintText.text = columnDestination + ":" + rowDestination;
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
                if (isAdd)
                {
                    AddPathToDestination(path);
                    path.GetComponent<RobotDetect>().TakePreviousPath(this);
                    isAdd = false;
                }
                else if (_isMove)
                {
                    Level.Instance.theChoosenOne = this;
                    if (pathsToDestination.Count != 0)
                    {
                        hint.SetActive(false);
                        road = pathsToDestination[_pathindex];
                        var dir = road.position - transform.position;
                        transform.Translate(dir.normalized * passengerSpeed * Time.deltaTime);
                        Quaternion lookRotaion = Quaternion.LookRotation(dir, Vector3.up);
                        passengerModel.transform.rotation = Quaternion.Euler(0, lookRotaion.eulerAngles.y, 0);
                        if (Vector3.Distance(transform.position, road.position) <= 0.1f)
                        {
                            SetNextPath();
                        }
                    }
                }
            }
        }
    }
    // void Move(Vector3 getDir)
    // {
    //     transform.Translate(getDir.normalized * passengerSpeed * Time.deltaTime);
    // }
    public void AddPathToDestination(Transform path)
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
            nextdestination = pathsToDestination[pathsToDestination.Count - 1].parent;
            SetNewDestination(nextdestination);
            _isMove = true;
        }
    }
    void SetNextPath()
    {
        transform.position = road.transform.position;
        _pathindex++;
        if (_pathindex >= pathsToDestination.Count)
        {
            _isMove = false;
            Level.Instance.theChoosenOne = null;
            pathsToDestination.Clear();
            currentdestination = nextdestination;
            nextdestination = null;
            _pathindex = 0;
            path = null;
            isSelected = false;
            passengerModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    void SetNewDestination(Transform newdestination)
    {
        if (Level.Instance.groundSelecteds.Contains(currentdestination))
        {
            Level.Instance.groundSelecteds.Remove(currentdestination);
        }
        Level.Instance.groundSelecteds.Add(newdestination);
        currentdestination.gameObject.GetComponent<Ground>().SetDestination(true);
        newdestination.gameObject.GetComponent<Ground>().SetDestination(false);
    }
    void ShootRaycastCheck(Vector3 direction, Color color)
    {
        Debug.DrawRay(transform.position, direction * 10, color);
        if (Physics.Raycast(transform.position, direction, out _raycastHit, 10, passengerLayerMask))
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
            if (currentdestination == null)
            {
                other.gameObject.GetComponent<Ground>().SetGroundBox(false);
                currentdestination = other.transform;
            }
        }
    }
}
