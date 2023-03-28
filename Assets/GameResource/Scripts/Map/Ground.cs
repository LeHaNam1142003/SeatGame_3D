
using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour,IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowRobotDetect();
    }

    [SerializeField] private RobotDetect robotdetect;

    private void Start()
    {
        HideRobotDetect();
    }

    void HideRobotDetect()
    {
        robotdetect.gameObject.SetActive(false);
    }
    private void OnMouseDown()
    {
        ShowRobotDetect();
    }

    public void ShowRobotDetect()
    {
        robotdetect.gameObject.SetActive(true);
    }
}