using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    public RobotDetect robotDetect;

    private void Start()
    {
        HideRobotDetect();
    }

    void HideRobotDetect()
    {
        robotDetect.gameObject.SetActive(false);
    }

    public void ShowRobotDetect(Transform getGround)
    {
        if (!Level.Instance.paths.Contains(getGround.transform))
        {
            Level.Instance.paths.Add(getGround.transform);
        }
        robotDetect.gameObject.SetActive(true);
    }
}