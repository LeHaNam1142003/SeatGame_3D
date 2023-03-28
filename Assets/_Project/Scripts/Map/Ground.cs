using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    [SerializeField] private RobotDetect robotdetect;

    private void Start()
    {
        HideRobotDetect();
    }

    void HideRobotDetect()
    {
        robotdetect.gameObject.SetActive(false);
    }

    public void ShowRobotDetect()
    {
        robotdetect.gameObject.SetActive(true);
    }
}