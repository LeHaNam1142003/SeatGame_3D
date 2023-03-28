using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RobotDetect : MonoBehaviour
{
    [SerializeField] public LayerMask LayerMaskTarget;
    [SerializeField] public RobotDetect PreviouPath;
    private RaycastHit hitright;
    private RaycastHit hitleft;
    private RaycastHit hitdown;
    private RaycastHit hitup;
    public bool stopraycastleft;
    public bool stopraycastright;
    public bool stopraycastdown;
    public bool stopraycastup;

    private void Update()
    {
        ShootRaycastDown();
        ShootRaycastLeft();
        ShootRaycastRight();
        ShootRaycastUp();
    }

    public void Reset()
    {
        stopraycastdown = false;
        stopraycastleft = false;
        stopraycastright = false;
        stopraycastup = false;
        PreviouPath = null;
    }

    // public void TakePreviousPath(Player player)
    // {
    //     if (PreviouPath != null)
    //     {
    //         player.AddPathToDestination(PreviouPath.transform);
    //         PreviouPath.TakePreviousPath(player);
    //     }
    // }
    private void OnDisable()
    {
        Reset();
    }

    void ShootRaycastRight()
    {
        if (stopraycastright != true)
        {
            Doraycast(Vector3.right, Color.blue, (() => { stopraycastright = true; }), hitright, EDirect.Left);
        }
    }

    void ShootRaycastLeft()
    {
        if (stopraycastleft != true)
        {
            Doraycast(Vector3.left, Color.red, (() => { stopraycastleft = true; }), hitleft, EDirect.Right);
        }
    }

    void ShootRaycastDown()
    {
        if (stopraycastdown != true)
        {
            Doraycast(Vector3.back, Color.green, (() => { stopraycastdown = true; }), hitdown, EDirect.Up);
        }
    }

    void ShootRaycastUp()
    {
        if (stopraycastup != true)
        {
            Doraycast(Vector3.forward, Color.gray, (() => { stopraycastup = true; }), hitup, EDirect.Down);
        }
    }

    void Doraycast(Vector3 direction, Color color, Action action, RaycastHit gethit, EDirect estopdirect)
    {
        Debug.DrawRay(transform.position, direction * 10, color);
        if (Physics.Raycast(transform.position, direction, out gethit, 10, LayerMaskTarget))
        {
            action?.Invoke();
            // if (gethit.collider.gameObject.CompareTag("Check"))
            // {
            //     var set = gethit.transform.gameObject.GetComponent<Ground>();
            //     set.ShowRobotDetect();
            //     if (!set.MarkController.Marks.Contains(gethit.transform))
            //     {
            //         set.MarkController.Marks.Add(gethit.transform);
            //     }
            //
            //     set.RobotDetect.PreviouPath = this;
            //
            //     switch (estopdirect)
            //     {
            //         case EDirect.Left:
            //             set.RobotDetect.stopraycastleft = true;
            //             break;
            //         case EDirect.Right:
            //             set.RobotDetect.stopraycastright = true;
            //             break;
            //         case EDirect.Up:
            //             set.RobotDetect.stopraycastup = true;
            //             break;
            //         case EDirect.Down:
            //             set.RobotDetect.stopraycastdown = true;
            //             break;
            //     }
            // }
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
