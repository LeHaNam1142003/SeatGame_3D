using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    [SerializeField] private RobotDetect robotDetect;

    private void Start()
    {
        HideRobotDetect();
    }

    void HideRobotDetect()
    {
        robotDetect.gameObject.SetActive(false);
    }

    public void ShowRobotDetect()
    {
        robotDetect.gameObject.SetActive(true);
    }
}