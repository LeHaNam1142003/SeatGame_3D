using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDetect : MonoBehaviour
{
    [SerializeField] private LayerMask layerMaskTarget;
    public RobotDetect previousRobotDetect;
    private RaycastHit hitRight;
    private RaycastHit hitLeft;
    private RaycastHit hitDown;
    private RaycastHit hitUp;
    public bool isStopRaycastLeft;
    public bool isStopRaycastRight;
    public bool isStopRaycastDown;
    public bool isStopRaycastUp;

    private void Update()
    {
        ShootRaycastDown();
        ShootRaycastLeft();
        ShootRaycastRight();
        ShootRaycastUp();
    }

    public void Reset()
    {
        isStopRaycastDown = false;
        isStopRaycastLeft = false;
        isStopRaycastRight = false;
        isStopRaycastUp = false;
        previousRobotDetect = null;
    }

    public void TakePreviousPath(Passenger passenger)
    {
        if (previousRobotDetect != null)
        {
            passenger.AddPathToDestination(previousRobotDetect.transform);
            previousRobotDetect.TakePreviousPath(passenger);
        }
    }
    private void OnDisable()
    {
        Reset();
    }

    void ShootRaycastRight()
    {
        if (isStopRaycastRight != true)
        {
            Doraycast(Vector3.right, Color.blue, (() => { isStopRaycastRight = true; }), hitRight, EDirect.Left);
        }
    }

    void ShootRaycastLeft()
    {
        if (isStopRaycastLeft != true)
        {
            Doraycast(Vector3.left, Color.red, (() => { isStopRaycastLeft = true; }), hitLeft, EDirect.Right);
        }
    }

    void ShootRaycastDown()
    {
        if (isStopRaycastDown != true)
        {
            Doraycast(Vector3.back, Color.green, (() => { isStopRaycastDown = true; }), hitDown, EDirect.Up);
        }
    }

    void ShootRaycastUp()
    {
        if (isStopRaycastUp != true)
        {
            Doraycast(Vector3.forward, Color.gray, (() => { isStopRaycastUp = true; }), hitUp, EDirect.Down);
        }
    }

    void Doraycast(Vector3 direction, Color color, Action action, RaycastHit gethit, EDirect eStopdirect)
    {
        Debug.DrawRay(transform.position, direction * 10, color);
        if (Physics.Raycast(transform.position, direction, out gethit, 10, layerMaskTarget))
        {
            action?.Invoke();
            if (gethit.collider.gameObject.CompareTag(NameTag.GroundCheck))
            {
                var set = gethit.transform.gameObject.GetComponent<Ground>();
                set.ShowRobotDetect(set.transform);
                set.robotDetect.previousRobotDetect = this;

                switch (eStopdirect)
                {
                    case EDirect.Left:
                        set.robotDetect.isStopRaycastLeft = true;
                        break;
                    case EDirect.Right:
                        set.robotDetect.isStopRaycastRight = true;
                        break;
                    case EDirect.Up:
                        set.robotDetect.isStopRaycastUp = true;
                        break;
                    case EDirect.Down:
                        set.robotDetect.isStopRaycastDown = true;
                        break;
                }
            }
        }
    }
}

public enum EDirect
{
    Right,
    Left,
    Up,
    Down,
}
