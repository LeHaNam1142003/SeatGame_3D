using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private LayerMask passengerLayerMask;
    [SerializeField] private bool isMove;
    [SerializeField] private Transform road;
    [SerializeField] private Transform currentdestination;
    [SerializeField] private Transform nextdestination;
    [SerializeField, Range(0, 100)] private float passengerSpeed;
    private RaycastHit _raycastHit;
    private int _pathindex = 0;
    public bool isSelected;
    public Transform path;
    [SerializeField] private List<Transform> pathsToDestination = new List<Transform>();
    public void SetSelected()
    {
        Level.Instance.SetTheSelectedPassenger(this);
        isMove = true;
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
                nextdestination = pathsToDestination[pathsToDestination.Count - 1].parent;
                SetNewDestination(nextdestination);
                if (isMove)
                {
                    Level.Instance.theChoosenOne = this;
                    if (pathsToDestination.Count != 0)
                    {
                        road = pathsToDestination[_pathindex];
                        var dir = road.position - transform.position;
                        transform.Translate(dir.normalized * passengerSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, road.position) <= 0.1f)
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
        if (!pathsToDestination.Contains(path))
        {
            pathsToDestination.Add(path);
        }
    }
    void SetNextPath()
    {
        transform.position = road.transform.position;
        _pathindex++;
        if (_pathindex >= pathsToDestination.Count)
        {
            isMove = false;
            Level.Instance.theChoosenOne = null;
            pathsToDestination.Clear();
            foreach (var clearRobot in Level.Instance.paths)
            {
                var ground = clearRobot.gameObject.GetComponent<Ground>();
                if (clearRobot != nextdestination)
                {
                    ground.SetGroundBox(true);
                }
                ground.robotDetect.gameObject.SetActive(false);
            }
            Level.Instance.paths.Clear();
            _pathindex = 0;
            path = null;
            isSelected = false;
        }
    }
    void SetNewDestination(Transform newdestination)
    {
        if (currentdestination != newdestination)
        {
            currentdestination.gameObject.GetComponent<Ground>().SetGroundBox(true);
            newdestination.gameObject.GetComponent<Ground>().SetGroundBox(false);
            currentdestination = newdestination;
        }
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
