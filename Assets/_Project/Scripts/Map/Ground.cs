using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    public RobotDetect robotDetect;
    public BoxCollider groundBox;

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
        SetGroundBox(false);
        robotDetect.gameObject.SetActive(true);
    }
    public void SetGroundBox(bool isEnable)
    {
        groundBox.enabled = isEnable;
    }
}
